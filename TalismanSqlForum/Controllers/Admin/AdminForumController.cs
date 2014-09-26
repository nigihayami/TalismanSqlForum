using System.Data.Entity;
using System.Web.Mvc;
using TalismanSqlForum.Models;
using TalismanSqlForum.Models.Forum;

namespace TalismanSqlForum.Controllers.Admin
{
    [Authorize(Roles = "admin")]
    public class AdminForumController : Controller
    {
        readonly ApplicationDbContext _db = new ApplicationDbContext();

        public void Hide(int id)
        {
            var t = _db.tForumLists.Find(id);
            if (t == null) return;
            t.tForumList_hide = !t.tForumList_hide;
            _db.Entry(t).State = EntityState.Modified;
            _db.SaveChanges();
        }
        #region Edit
        public ActionResult Edit(int? id)
        {
            var t = _db.tForumLists.Find(id);
            return View(t);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,tForumList_name,tForumList_description,tForumList_hide,tForumList_icon")] tForumList tForumList)
        {
            if (!ModelState.IsValid) return View(tForumList);
            _db.Entry(tForumList).State = EntityState.Modified;
            _db.SaveChanges();
            return RedirectToAction("Forum", "Admin");
        }
        #endregion
        #region Create
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,tForumList_name,tForumList_description,tForumList_hide,tForumList_icon")] tForumList tForumList)
        {
            if (!ModelState.IsValid) return View(tForumList);
            _db.Entry(tForumList).State = EntityState.Added;
            _db.SaveChanges();
            return RedirectToAction("Forum", "Admin");
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
