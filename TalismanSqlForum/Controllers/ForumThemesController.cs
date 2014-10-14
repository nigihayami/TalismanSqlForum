using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
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
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        [AllowAnonymous]
        public ActionResult Index(int? id)
        {
            if (id != null)
            {
                Code.Stat.SetView(Request.UserHostAddress);
                Code.Stat.SetViewList(Request.UserHostAddress, id.Value);
            }
            var t = _db.tForumLists.Find(id);
            if (t == null) return HttpNotFound();
            ViewBag.Title = t.tForumList_name;
            ViewData["tForumList_id"] = t.Id;
            ViewData["tForumList_icon"] = t.tForumList_icon;
            ViewData["tForumThemes_top"] = t.tForumThemes.Where(a => !a.tForumThemes_hide).Where(a => a.tForumThemes_top).OrderByDescending(a => a.tForumThemes_datetime).ToList();
            ViewData["tForumThemes"] = t.tForumThemes.Where(a => !a.tForumThemes_hide).Where(a => !a.tForumThemes_top).OrderByDescending(a => a.tForumThemes_datetime).ToList();
            return View();
        }

        // GET: ForumThemes/Create
        public ActionResult Create(int? id)
        {
            var t = _db.tForumLists.Find(id);
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
            tForumThemes.tForumList = _db.tForumLists.Find(id);
            tForumThemes.tForumThemes_datetime = DateTime.Now;
            tForumThemes.tUsers = _db.Users.First(a => a.UserName == User.Identity.Name);
            tForumThemes.tForumThemes_hide = false;

            var userId = tForumThemes.tUsers.Id;
            if (tForumThemes.tForumThemes_desc != null)
            {
                tForumThemes.tForumThemes_desc = WebUtility.HtmlDecode(tForumThemes.tForumThemes_desc);
            }
            if (!ModelState.IsValid) return View(tForumThemes);
            if (User.IsInRole("user"))
            {
                //это я поставлю на всякий случай - защита от самого себя
                //пользователь не может создавать закрепленные темы
                tForumThemes.tForumThemes_top = false;
                tForumThemes.tForumThemes_close = false;
            }
            _db.tForumThemes.Add(tForumThemes);
            _db.SaveChanges();
            var r = _db.Roles.ToList();
            foreach (var item in r)
            {
                //по ролям
                foreach (var item2 in _db.Users.Where(a => a.Roles.Any(b => b.RoleId == item.Id)).Where(a => a.Id != userId))
                {
                    //по пользователям в роли
                    if (
                        _db.tUserNewThemes.Where(a => a.tUsers.Id == item2.Id)
                            .Any(b => b.tForumThemes.Id == tForumThemes.Id)) continue;
                    var n = new tUserNewThemes { tForumThemes = tForumThemes, tUsers = item2 };
                    _db.tUserNewThemes.Add(n);
                    _db.SaveChanges();
                }
            }
            //отсылаем сообщение всем модераторам
            var val = Url.RequestContext.HttpContext.Request.Url.Scheme;
            var href = Url.Action("Index", "ForumMessages", new { id = tForumThemes.Id, id_list = tForumThemes.tForumList.Id }, val);
            Code.Notify.NewThemes(tForumThemes.Id, href, userId);


            return RedirectToAction("Index", "ForumMessages", new { id = tForumThemes.Id });
        }

        [Authorize(Roles = "admin,moderator")]
        public ActionResult Edit(int? id)
        {
            var t = _db.tForumThemes.Find(id);
            if (t == null) return HttpNotFound();
            var m = new tForumThemes_Edit
            {
                tForumThemes_name = t.tForumThemes_name,
                tForumThemes_desc = t.tForumThemes_desc
            };
            ViewData["tForumThemes_name"] = t.tForumThemes_name;
            ViewData["tForumThemes_id"] = t.Id;
            return View(m);
        }
        [Authorize(Roles = "admin,moderator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "tForumThemes_desc,tForumThemes_Name")] int id, tForumThemes_Edit t)
        {
            var d = _db.tForumThemes.Find(id);
            if (d == null) return HttpNotFound();
            d.tForumThemes_name = t.tForumThemes_name;
            d.tForumThemes_desc = t.tForumThemes_desc;
            _db.Entry(d).State = EntityState.Modified;
            _db.SaveChanges();
            return RedirectToAction("Index", "ForumThemes", new { id = d.tForumList.Id });
        }

        [Authorize(Roles = "admin,moderator")]
        public ActionResult Top(int? id)
        {
            var t = _db.tForumThemes.Find(id);
            if (t == null) return HttpNotFound();
            t.tForumThemes_top = !t.tForumThemes_top;
            _db.Entry(t).State = EntityState.Modified;
            _db.SaveChanges();
            return RedirectToAction("Index", "ForumMessages", new { id = id });
        }
        [Authorize(Roles = "admin,moderator")]
        public ActionResult Close(int? id)
        {
            var t = _db.tForumThemes.Find(id);
            if (t == null) return HttpNotFound();
            t.tForumThemes_close = !t.tForumThemes_close;
            _db.Entry(t).State = EntityState.Modified;
            _db.SaveChanges();
            return RedirectToAction("Index", "ForumMessages", new { id = id });
        }
        [Authorize(Roles = "admin")]
        public ActionResult Hide(int? id)
        {
            var t = _db.tForumThemes.Find(id);
            if (t == null) return HttpNotFound();
            t.tForumThemes_hide = true;
            _db.Entry(t).State = EntityState.Modified;
            _db.SaveChanges();
            return RedirectToAction("Index", new { id = t.tForumList.Id });
        }
        #region Перенос темы
        [Authorize(Roles = "admin,moderator")]
        public ActionResult Transfer(int? id)
        {
            var t = _db.tForumThemes.Find(id);
            if (t == null) return HttpNotFound();
            ViewData["tForumList"] = _db.tForumLists.Where(a => a.Id != t.tForumList.Id).ToList();
            ViewData["tForumThemes_Id"] = id;
            return View();
        }
        [Authorize(Roles = "admin,moderator")]
        public ActionResult TransferGo(int? id, int? id_list)
        {
            var t = _db.tForumThemes.Find(id);
            if (t == null) return HttpNotFound();
            t.tForumList = _db.tForumLists.Find(id_list);
            _db.Entry(t).State = EntityState.Modified;
            _db.SaveChanges();
            return RedirectToAction("Index", new { id = id_list });
        }
        #endregion

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
