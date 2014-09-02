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
    [Authorize(Roles="admin,moderator,user")]
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
            ViewData["tForumMessages"] = t.tForumMessages;
            ViewData["tUsers"] = t.tUsers;
            ViewData["tUsers_NickName"] = t.tUsers.NickName;
            ViewData["tUsers_Org"] = t.tUsers.Name_Org;
            ViewData["roles"] = db.Roles.ToList();
            ViewBag.ReturnUrl = Url.Action("Index", "ForumMessages", new { id = id });
            if (User.Identity.IsAuthenticated)
            {
                foreach (var tUserNewThemes in db.tUserNewThemes.Where(a => a.tUsers.UserName == User.Identity.Name).Where(b => b.tForumThemes.Id == id))
                {
                    //Удаляем из новых сообщений
                    db.tUserNewThemes.Remove(tUserNewThemes);
                    db.SaveChanges();
                }
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "tForumMessages_messages")] int? id , tForumMessages tForumMessages)
        {
            tForumMessages.tForumThemes = db.tForumThemes.Find(id);
            tForumMessages.tUsers = db.Users.Where(a => a.UserName == User.Identity.Name).First();
            tForumMessages.tForumMessages_datetime = DateTime.Now;
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
                    foreach (var item2 in db.Users.Where(a => a.Roles.Where(b => b.RoleId == item.Id).Count() > 0))
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
                    }
                }
                var val = this.Url.RequestContext.HttpContext.Request.Url.Scheme;
                var href = Url.Action("Index", "ForumMessages", new { id = tForumMessages.tForumThemes.Id, id_list = tForumMessages.tForumThemes.tForumList.Id }, val);
                TalismanSqlForum.Code.Notify.NewMessage(tForumMessages.Id,href);                
                return RedirectToAction("Index", new { id = id, id_list = tForumMessages.tForumThemes.tForumList.Id });
            }

            return View(tForumMessages);
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
