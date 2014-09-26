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
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: Search
        public ActionResult Index(string _searchVal, string _searchUser, string _searchPlace, string _searchFrom, string _searchTo, string _searchOrder, string _searchIn)
        {
            ViewData["val"] = _searchVal;
            var t1 = db.tForumThemes.Where(a => a.tForumThemes_name.Contains(_searchVal));
            var t2 = db.tForumThemes.Where(a => a.tForumThemes_desc.Contains(_searchVal));
            var t3 = db.tForumMessages.Where(a => a.tForumMessages_messages.Contains(_searchVal));
            if (!string.IsNullOrEmpty(_searchIn))
            {
                if (!_searchIn.StartsWith("all"))
                {
                    if (_searchIn.StartsWith("fl"))
                    {
                        var i = Convert.ToInt32(_searchIn.Substring(3));
                        t1 = t1.Where(a => a.tForumList.Id == i);
                        t2 = t2.Where(a => a.tForumList.Id == i);
                        t3 = t3.Where(a => a.tForumThemes.tForumList.Id == i);
                    }
                    else if (_searchIn.StartsWith("fm"))
                    {
                        var i = Convert.ToInt32(_searchIn.Substring(3));
                        t1 = t1.Where(a => a.Id == i);
                        t2 = t2.Where(a => a.Id == i);
                        t3 = t3.Where(a => a.tForumThemes.Id == i);
                    }
                }
            }
           
            if (!string.IsNullOrEmpty(_searchUser))
            {
                if (db.Users.Any(a => String.Equals(a.Name_Org, _searchUser, StringComparison.CurrentCultureIgnoreCase)))
                {
                    t1 = t1.Where(a => String.Equals(a.tUsers.Name_Org, _searchUser, StringComparison.CurrentCultureIgnoreCase));
                    t2 = t2.Where(a => String.Equals(a.tUsers.Name_Org, _searchUser, StringComparison.CurrentCultureIgnoreCase));
                    t3 = t3.Where(a => String.Equals(a.tUsers.Name_Org, _searchUser, StringComparison.CurrentCultureIgnoreCase));
                }
                else
                {
                    t1 = t1.Where(a => String.Equals(a.tUsers.NickName, _searchUser, StringComparison.CurrentCultureIgnoreCase));
                    t2 = t2.Where(a => String.Equals(a.tUsers.NickName, _searchUser, StringComparison.CurrentCultureIgnoreCase));
                    t3 = t3.Where(a => String.Equals(a.tUsers.NickName, _searchUser, StringComparison.CurrentCultureIgnoreCase));
                }
            }
            switch (_searchPlace)
            {
                case "1":
                    t1 = t1.Where(a => a.Id == -1);
                    t2 = t2.Where(a => a.Id == -1); ;
                    break;
                case "2":
                    t3 = t3.Where(a => a.Id == -1); ;
                    break;
            }
            if (!string.IsNullOrEmpty(_searchFrom))
            {
                var s = Convert.ToDateTime(_searchFrom);
                t1 = t1.Where(a => a.tForumThemes_datetime >= s);
                t2 = t1.Where(a => a.tForumThemes_datetime >= s);
                t3 = t3.Where(a => a.tForumMessages_datetime >= s);
            }
            if (!string.IsNullOrEmpty(_searchTo))
            {
                var s = Convert.ToDateTime(_searchTo);
                t1 = t1.Where(a => a.tForumThemes_datetime <= s);
                t2 = t1.Where(a => a.tForumThemes_datetime <= s);
                t3 = t3.Where(a => a.tForumMessages_datetime <= s);
            }
            if (!string.IsNullOrEmpty(_searchOrder))
            {
                switch (_searchOrder)
                {
                    case "1":
                        t1 = t1.OrderBy(a => a.tForumThemes_datetime);
                        t2 = t2.OrderBy(a => a.tForumThemes_datetime);
                        t3 = t3.OrderBy(a => a.tForumMessages_datetime);
                        break;
                    case "2":
                        t1 = t1.OrderByDescending(a => a.tForumThemes_datetime);
                        t2 = t2.OrderByDescending(a => a.tForumThemes_datetime);
                        t3 = t3.OrderByDescending(a => a.tForumMessages_datetime);
                        break;
                    case "3":
                        t1 = t1.OrderBy(a => a.tForumThemes_name);
                        t2 = t2.OrderBy(a => a.tForumThemes_name);
                        t3 = t3.OrderBy(a => a.tForumThemes.tForumThemes_name);
                        break;
                    case "4":
                        t1 = t1.OrderByDescending(a => a.tForumThemes_name);
                        t2 = t2.OrderByDescending(a => a.tForumThemes_name);
                        t3 = t3.OrderByDescending(a => a.tForumThemes.tForumThemes_name);
                        break;
                }
            }
            ViewData["user"] = _searchUser;
            ViewData["tForumThemes"] = t1;
            ViewData["tForumThemes_desc"] = t2;
            ViewData["tForumMessages"] = t3;
            ViewData["tForumList"] = db.tForumLists.ToList();
            return View("Index");
        }
    }
}
