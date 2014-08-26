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
        public ActionResult Index(string _searchVal, string _searchUser)
        {
            ViewData["val"] = _searchVal;
            var t1 = db.tForumThemes.Where(a => a.tForumThemes_name.Contains(_searchVal));
            var t2 = db.tForumThemes.Where(a => a.tForumThemes_desc.Contains(_searchVal));
            var t3 = db.tForumMessages.Where(a => a.tForumMessages_messages.Contains(_searchVal));
            if (_searchUser != null && _searchUser != "")
            {
                t1 = t1.Where(a => a.tUsers.UserName.ToUpper() == _searchUser);
                t2 = t2.Where(a => a.tUsers.UserName.ToUpper() == _searchUser);
                t3 = t3.Where(a => a.tUsers.UserName.ToUpper() == _searchUser);
            }

            ViewData["user"] = _searchUser;
            ViewData["tForumThemes"] = t1;
            ViewData["tForumThemes_desc"] = t2;
            ViewData["tForumMessages"] = t3;
            return View("Index");
        }
    }
}
