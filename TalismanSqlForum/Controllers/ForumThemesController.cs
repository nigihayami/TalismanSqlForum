using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using TalismanSqlForum.Models;
using TalismanSqlForum.Models.Forum;
using TalismanSqlForum.Models.Users;
using TalismanSqlForum.Models.ViewModel;

namespace TalismanSqlForum.Controllers
{
    [Authorize(Roles = "admin,user,moderator")]
    public class ForumThemesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [AllowAnonymous]
        public ActionResult Index(int? id)
        {
            var t = db.tForumLists.Find(id);
            if (t == null)
            {
                return HttpNotFound();
            }
            ViewBag.Title = t.tForumList_name;
            ViewData["tForumList_id"] = t.Id;
            ViewData["tForumList_icon"] = t.tForumList_icon;
            ViewData["tForumThemes_top"] = t.tForumThemes.Where(a => a.tForumThemes_top == true).OrderByDescending(a => a.tForumThemes_datetime).ToList();
            ViewData["tForumThemes"] = t.tForumThemes.Where(a => a.tForumThemes_top == false).OrderByDescending(a => a.tForumThemes_datetime).ToList();
            return View();
        }

        // GET: ForumThemes/Create
        public ActionResult Create(int? id)
        {
            var t = db.tForumLists.Find(id);
            if (t == null)
            {
                return HttpNotFound();
            }
            ViewData["tForumList_id"] = t.Id;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,tForumThemes_name,tForumThemes_datetime,tForumThemes_desc,tForumThemes_top,tForumThemes_close")] int? id, tForumThemes tForumThemes)
        {           
            tForumThemes.tForumList = db.tForumLists.Find(id);
            tForumThemes.tForumThemes_datetime = DateTime.Now;
            tForumThemes.tUsers = db.Users.Where(a => a.UserName == User.Identity.Name).First();

            var UserId = tForumThemes.tUsers.Id;
            if (tForumThemes.tForumThemes_desc != null)
            {
                tForumThemes.tForumThemes_desc = WebUtility.HtmlDecode(tForumThemes.tForumThemes_desc);
            }
            if (ModelState.IsValid)
            {
                if (User.IsInRole("user"))
                {
                    //это я поставлю на всякий случай - защита от самого себя
                    //пользователь не может создавать закрепленные темы
                    tForumThemes.tForumThemes_top = false;
                    tForumThemes.tForumThemes_close = false;
                }
                db.tForumThemes.Add(tForumThemes);
                db.SaveChanges();                
                var r = db.Roles.ToList();
                foreach (var item in r)
                {
                    //по ролям
                    foreach (var item2 in db.Users.Where(a => a.Roles.Where(b => b.RoleId == item.Id).Count() > 0).Where(a => a.Id != UserId))
                    {
                        //по пользователям в роли
                        if (db.tUserNewThemes.Where(a=> a.tUsers.Id == item2.Id).Where(b => b.tForumThemes.Id == tForumThemes.Id).Count() == 0)
                        {
                            var n = new tUserNewThemes();
                            n.tForumThemes = tForumThemes;
                            n.tUsers = item2;
                            db.tUserNewThemes.Add(n);
                            db.SaveChanges();
                        }
                    }
                }
                //отсылаем сообщение всем модераторам
                var val = this.Url.RequestContext.HttpContext.Request.Url.Scheme;
                var href = Url.Action("Index", "ForumMessages", new { id = tForumThemes.Id, id_list = tForumThemes.tForumList.Id }, val);
                TalismanSqlForum.Code.Notify.NewThemes(tForumThemes.Id, href, UserId);
                
                
                return RedirectToAction("Index","ForumMessages", new { id = tForumThemes.Id });
            }

            return View(tForumThemes);
        }

        [Authorize(Roles="admin,moderator")]
        public ActionResult Edit(int? id)
        {
            var t = db.tForumThemes.Find(id);
            if (t != null)
            {
                var m = new tForumThemes_Edit();
                m.tForumThemes_name = t.tForumThemes_name;
                m.tForumThemes_desc = t.tForumThemes_desc;
                ViewData["tForumThemes_name"] = t.tForumThemes_name;
                ViewData["tForumThemes_id"] = t.Id;
                return View(m);
            }
            return HttpNotFound();
        }
        [Authorize(Roles = "admin,moderator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "tForumThemes_desc,tForumThemes_Name")] int id, tForumThemes_Edit t)
        {
            var d = db.tForumThemes.Find(id);
            if (d != null)
            {
                d.tForumThemes_name = t.tForumThemes_name;
                d.tForumThemes_desc = t.tForumThemes_desc;
                db.Entry(d).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "ForumThemes", new { id = d.tForumList.Id });
            }
            return HttpNotFound();
        }
        
        
        
        [Authorize(Roles = "admin,moderator")]
        public ActionResult Top(int? id)
        {
            var t = db.tForumThemes.Find(id);
            if (t != null)
            {
                t.tForumThemes_top = !t.tForumThemes_top;
                db.Entry(t).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "ForumMessages", new { id = id });
            }
            return HttpNotFound();
        }
        [Authorize(Roles = "admin,moderator")]
        public ActionResult Close(int? id)
        {
            var t = db.tForumThemes.Find(id);
            if (t != null)
            {
                t.tForumThemes_close = !t.tForumThemes_close;
                db.Entry(t).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "ForumMessages", new { id = id });
            }
            return HttpNotFound();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
