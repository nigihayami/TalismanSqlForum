using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TalismanSqlForum.Models;
using TalismanSqlForum.Models.Forum;

namespace TalismanSqlForum.Controllers.Admin
{
    [Authorize(Roles = "admin")]
    public class AdminForumController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public void Hide(int id)
        {
            var t = db.tForumLists.Find(id);
            if (t != null)
            {
                t.tForumList_hide = !t.tForumList_hide;
                db.Entry(t).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        #region Edit
        public ActionResult Edit(int? id)
        {
            var t = db.tForumLists.Find(id);
            return View(t);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,tForumList_name,tForumList_description,tForumList_hide,tForumList_icon")] tForumList tForumList)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tForumList).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Forum", "Admin");
            }
            return View(tForumList);
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
            if (ModelState.IsValid)
            {
                db.Entry(tForumList).State = EntityState.Added;
                db.SaveChanges();
                return RedirectToAction("Forum", "Admin");
            }
            return View(tForumList);
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
