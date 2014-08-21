using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TalismanSqlForum.Models;

namespace TalismanSqlForum.Controllers.Admin
{
    public class AdminForumController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public void Hide(int id)
        {
            var t = db.tForumLists.Find(id);
            if (t != null)
            {
                t.tForumList_hide = !t.tForumList_hide;
                db.Entry(t).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
