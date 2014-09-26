using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using TalismanSqlForum.Models;
using TalismanSqlForum.Models.Forum;
using TalismanSqlForum.Models.Users;

namespace TalismanSqlForum.Controllers
{
    [Authorize(Roles = "admin,moderator,user")]
    public class ForumMessagesController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        [AllowAnonymous]
        // GET: ForumMessages
        public ActionResult Index(int? id)
        {
            var t = _db.tForumThemes.Find(id);
            if (t == null) return HttpNotFound();
            ViewData["ForumThemes_DateTime"] = t.tForumThemes_datetime;
            ViewData["ForumThemes_Name"] = t.tForumThemes_name;
            ViewData["ForumThemes_Desc"] = t.tForumThemes_desc;
            ViewData["ForumThemes_Id"] = t.Id;
            ViewData["tForumThemes_top"] = t.tForumThemes_top;
            ViewData["tForumThemes_close"] = t.tForumThemes_close;

            var v = t.toffer.Select(item => item.tOffer_docnumber).ToList();
            ViewData["tForumThemes_docnumber"] = v;

            ViewData["tForumMessages"] = t.tForumMessages.Where(a => !a.tForumMessages_hide).ToList();
            ViewData["tUsers"] = t.tUsers;
            ViewData["tUsers_NickName"] = t.tUsers.NickName;
            ViewData["tUsers_Org"] = t.tUsers.Name_Org;
            ViewData["roles"] = _db.Roles.ToList();
            ViewBag.ReturnUrl = Url.Action("Index", "ForumMessages", new { id = id });
            
            ViewData["ForumThemes_Is_Edit"] = false;

            if (!User.Identity.IsAuthenticated) return View();

            if (User.IsInRole("admin") || User.IsInRole("moderator"))
            {
                //admin GRANT
                ViewData["ForumThemes_Is_Edit"] = true;
            }


            foreach (var tUserNewThemes in _db.tUserNewThemes.Where(a => a.tUsers.UserName == User.Identity.Name).Where(b => b.tForumThemes.Id == id))
            {
                //Удаляем из новых сообщений
                _db.tUserNewThemes.Remove(tUserNewThemes);
                _db.SaveChanges();
            }
            foreach (var tUserNewMessages in _db.tUserNewMessages.Where(a => a.tUsers.UserName == User.Identity.Name).Where(b => b.tForumMessages.tForumThemes.Id == id))
            {
                //Удаляем из новых сообщений
                _db.tUserNewMessages.Remove(tUserNewMessages);
                _db.SaveChanges();
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "tForumMessages_messages")] int? id, tForumMessages tForumMessages)
        {
            tForumMessages.tForumThemes = _db.tForumThemes.Find(id);
            tForumMessages.tUsers = _db.Users.First(a => a.UserName == User.Identity.Name);
            tForumMessages.tForumMessages_datetime = DateTime.Now;
            tForumMessages.tForumMessages_hide = false;
            var userId = tForumMessages.tUsers.Id;
            if (tForumMessages.tForumMessages_messages != null)
            {
                tForumMessages.tForumMessages_messages = WebUtility.HtmlDecode(tForumMessages.tForumMessages_messages);
            }
            if (!ModelState.IsValid)
                return RedirectToAction("Index", new {id, id_list = tForumMessages.tForumThemes.tForumList.Id});
            
            _db.tForumMessages.Add(tForumMessages);
            _db.SaveChanges();
            var r = _db.Roles.ToList();
            foreach (var item2 in r.SelectMany(item => _db.Users.Where(a => a.Roles.Any(b => b.RoleId == item.Id)).Where(a => a.Id != userId)))
            {
                //по пользователям в роли
                if (!_db.tUserNewThemes.Where(a => a.tUsers.Id == item2.Id).Any(b => b.tForumThemes.Id == tForumMessages.tForumThemes.Id))
                {
                    var n = new tUserNewThemes {tForumThemes = tForumMessages.tForumThemes, tUsers = item2};
                    _db.tUserNewThemes.Add(n);
                    _db.SaveChanges();
                }
                //Также новое сообщение
                if (
                    _db.tUserNewMessages.Where(a => a.tUsers.Id == item2.Id)
                        .Any(b => b.tForumMessages.Id == tForumMessages.Id)) continue;
                var nm = new tUserNewMessages {tForumMessages = tForumMessages, tUsers = item2};
                _db.tUserNewMessages.Add(nm);
                _db.SaveChanges();
            }
            var val = Url.RequestContext.HttpContext.Request.Url.Scheme;
            var href = Url.Action("Index", "ForumMessages", new { id = tForumMessages.tForumThemes.Id, id_list = tForumMessages.tForumThemes.tForumList.Id }, val);
            Code.Notify.NewMessage(tForumMessages.Id, href, userId);
            return RedirectToAction("Index", new { id = id, id_list = tForumMessages.tForumThemes.tForumList.Id });
        }
        [Authorize(Roles = "admin,moderator")]
        public ActionResult Hide(int? id)
        {
            var t = _db.tForumMessages.Find(id);
            if (t == null) return HttpNotFound();
            t.tForumMessages_hide = true;
            _db.Entry(t).State = EntityState.Modified;
            _db.SaveChanges();
            return RedirectToAction("Index", new { id = t.tForumThemes.Id, id_list = t.tForumThemes.tForumList.Id });
        }
        [Authorize(Roles="admin,moderator")]
        public ActionResult Edit(int? id)
        {
            var t = _db.tForumMessages.Find(id);
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
            var t = _db.tForumMessages.Find(id);
            if (t == null) return HttpNotFound();
            t.tForumMessages_messages = WebUtility.HtmlDecode(tForumMessages.tForumMessages_messages);
            t.tUsers_Edit_name = _db.Users.First(a => a.UserName == User.Identity.Name);
            t.tUsers_Edit_datetime = DateTime.Now;
            _db.Entry(t).State = EntityState.Modified;
            _db.SaveChanges();
            return RedirectToAction("Index", new { id = t.tForumThemes.Id, id_list = t.tForumThemes.tForumList.Id });
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
