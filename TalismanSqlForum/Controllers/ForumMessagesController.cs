using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TalismanSqlForum.Models;
using TalismanSqlForum.Models.Forum;

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
            ViewData["tForumMessages"] = t.tForumMessages;
            ViewData["tUsers"] = t.tUsers;
            ViewData["tUsers_NickName"] = t.tUsers.NickName;
            ViewData["tUsers_Org"] = t.tUsers.Name_Org;
            ViewData["roles"] = db.Roles.ToList();
            ViewBag.ReturnUrl = Url.Action("Index", "ForumMessages", new { id = id });
            return View();
        }

        // POST: ForumMessages/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
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
                return RedirectToAction("Index", new { id = id, id_fl = tForumMessages.tForumThemes.tForumList.Id });
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
