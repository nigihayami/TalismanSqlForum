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

namespace TalismanSqlForum.Controllers
{
    [Authorize(Roles = "admin,moderator,user")]
    public class ForumMessagesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [AllowAnonymous]
        // GET: ForumMessages
        public ActionResult Index(int? id)
        {
            var t = db.tForumThemes.Find(id);
            ViewData["ForumThemes_DateTime"] = t.tForumThemes_datetime;
            ViewData["ForumThemes_Name"] = t.tForumThemes_name;
            ViewData["ForumThemes_Desc"] = t.tForumThemes_desc;
            ViewData["ForumThemes_Id"] = t.Id;
            ViewData["tForumThemes_top"] = t.tForumThemes_top;
            ViewData["tForumThemes_close"] = t.tForumThemes_close;
            List<int> v = new List<int>();
            foreach(var item in t.toffer)
            {
                v.Add(item.tOffer_docnumber);
            }
            ViewData["tForumThemes_docnumber"] = v;

            ViewData["tForumMessages"] = t.tForumMessages.Where(a => !a.tForumMessages_hide).ToList();
            ViewData["tUsers"] = t.tUsers;
            ViewData["tUsers_NickName"] = t.tUsers.NickName;
            ViewData["tUsers_Org"] = t.tUsers.Name_Org;
            ViewData["roles"] = db.Roles.ToList();
            ViewBag.ReturnUrl = Url.Action("Index", "ForumMessages", new { id = id });
            if (User.Identity.IsAuthenticated)
            {
                ViewData["ForumThemes_Is_Edit"] = false;
                if (!User.IsInRole("admin"))
                {
                    if (User.IsInRole("moderator"))
                    {
                        var r = db.Roles.Where(a => a.Name == "admin").First();
                        if (!(t.tUsers.Roles.Where(a => a.RoleId == r.Id).Count() > 0))
                        {
                            ViewData["ForumThemes_Is_Edit"] = true;
                        }
                    }
                }
                else
                {
                    //admin GRANT
                    ViewData["ForumThemes_Is_Edit"] = true;
                }


                foreach (var tUserNewThemes in db.tUserNewThemes.Where(a => a.tUsers.UserName == User.Identity.Name).Where(b => b.tForumThemes.Id == id))
                {
                    //Удаляем из новых сообщений
                    db.tUserNewThemes.Remove(tUserNewThemes);
                    db.SaveChanges();
                }
                foreach (var tUserNewMessages in db.tUserNewMessages.Where(a => a.tUsers.UserName == User.Identity.Name).Where(b => b.tForumMessages.tForumThemes.Id == id))
                {
                    //Удаляем из новых сообщений
                    db.tUserNewMessages.Remove(tUserNewMessages);
                    db.SaveChanges();
                }
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "tForumMessages_messages")] int? id, tForumMessages tForumMessages)
        {
            tForumMessages.tForumThemes = db.tForumThemes.Find(id);
            tForumMessages.tUsers = db.Users.Where(a => a.UserName == User.Identity.Name).First();
            tForumMessages.tForumMessages_datetime = DateTime.Now;
            tForumMessages.tForumMessages_hide = false;
            var UserId = tForumMessages.tUsers.Id;
            if (tForumMessages.tForumMessages_messages != null)
            {
                tForumMessages.tForumMessages_messages = WebUtility.HtmlDecode(tForumMessages.tForumMessages_messages);
            }
            if (ModelState.IsValid)
            {
                db.tForumMessages.Add(tForumMessages);
                db.SaveChanges();
                var r = db.Roles.ToList();
                foreach (var item in r)
                {
                    //по ролям
                    foreach (var item2 in db.Users.Where(a => a.Roles.Where(b => b.RoleId == item.Id).Count() > 0).Where(a => a.Id != UserId))
                    {
                        //по пользователям в роли
                        if (db.tUserNewThemes.Where(a => a.tUsers.Id == item2.Id).Where(b => b.tForumThemes.Id == tForumMessages.tForumThemes.Id).Count() == 0)
                        {
                            var n = new tUserNewThemes();
                            n.tForumThemes = tForumMessages.tForumThemes;
                            n.tUsers = item2;
                            db.tUserNewThemes.Add(n);
                            db.SaveChanges();
                        }
                        //Также новое сообщение
                        if (db.tUserNewMessages.Where(a => a.tUsers.Id == item2.Id).Where(b => b.tForumMessages.Id == tForumMessages.Id).Count() == 0)
                        {
                            var n = new tUserNewMessages();
                            n.tForumMessages = tForumMessages;
                            n.tUsers = item2;
                            db.tUserNewMessages.Add(n);
                            db.SaveChanges();
                        }
                    }
                }
                var val = this.Url.RequestContext.HttpContext.Request.Url.Scheme;
                var href = Url.Action("Index", "ForumMessages", new { id = tForumMessages.tForumThemes.Id, id_list = tForumMessages.tForumThemes.tForumList.Id }, val);
                TalismanSqlForum.Code.Notify.NewMessage(tForumMessages.Id, href, UserId);
                return RedirectToAction("Index", new { id = id, id_list = tForumMessages.tForumThemes.tForumList.Id });
            }

            return View(tForumMessages);
        }
        [Authorize(Roles = "admin,moderator")]
        public ActionResult Hide(int? id)
        {
            var t = db.tForumMessages.Find(id);
            if (t != null)
            {
                t.tForumMessages_hide = true;
                db.Entry(t).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { id = t.tForumThemes.Id, id_list = t.tForumThemes.tForumList.Id });
            }
            return HttpNotFound();
        }
        [Authorize(Roles="admin,moderator")]
        public ActionResult Edit(int? id)
        {
            var t = db.tForumMessages.Find(id);
            if (t != null)
            {                
                return View(t);
            }
            return HttpNotFound();
        }
        [Authorize(Roles = "admin,moderator")]
        [HttpPost]
        public ActionResult Edit([Bind(Include = "tForumMessages_messages")] int? id, tForumMessages tForumMessages)
        {
            var t = db.tForumMessages.Find(id);
            if (t != null)
            {
                t.tForumMessages_messages = WebUtility.HtmlDecode(tForumMessages.tForumMessages_messages);
                t.tUsers_Edit_name = db.Users.Where(a => a.UserName == User.Identity.Name).First();
                t.tUsers_Edit_datetime = DateTime.Now;
                db.Entry(t).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { id = t.tForumThemes.Id, id_list = t.tForumThemes.tForumList.Id });
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
