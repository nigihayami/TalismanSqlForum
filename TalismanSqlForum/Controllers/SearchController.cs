using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TalismanSqlForum.Models;

namespace TalismanSqlForum.Controllers
{
    public class SearchController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Search
        public ActionResult Index(string val)
        {
            ViewData["val"] = val;
            ViewData["tForumThemes"] = db.tForumThemes.Where(a => a.tForumThemes_name.Contains(val));
            ViewData["tForumThemes_desc"] = db.tForumThemes.Where(a => a.tForumThemes_desc.Contains(val));
            ViewData["tForumMessages"] = db.tForumMessages.Where(a => a.tForumMessages_messages.Contains(val));
            return View();
        }
    }
}
