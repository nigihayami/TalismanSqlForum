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
    public class ForumListController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ForumList
        public ActionResult Index()
        {
            ViewData["tForumList"] = db.tForumLists.Where(a=> !a.tForumList_hide).OrderBy(a => a.tForumList_name).ToList();
            return View();
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
