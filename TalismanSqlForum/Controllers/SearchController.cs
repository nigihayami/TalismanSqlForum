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
        public ActionResult Index(string _searchVal, string _searchUser, string _searchPlace, string _searchFrom, string _searchTo, string _searchOrder, string _searchIn)
        {
            ViewData["val"] = _searchVal;
            var t1 = db.tForumThemes.Where(a => a.tForumThemes_name.Contains(_searchVal));
            var t2 = db.tForumThemes.Where(a => a.tForumThemes_desc.Contains(_searchVal));
            var t3 = db.tForumMessages.Where(a => a.tForumMessages_messages.Contains(_searchVal));
            if (_searchIn != null && _searchIn != "")
            {
                if (!_searchIn.StartsWith("all"))
                {
                    if (_searchIn.StartsWith("fl"))
                    {
                        int i = Convert.ToInt32(_searchIn.Substring(3));
                        t1 = t1.Where(a => a.tForumList.Id == i);
                        t2 = t2.Where(a => a.tForumList.Id == i);
                        t3 = t3.Where(a => a.tForumThemes.tForumList.Id == i);
                    }
                    else if (_searchIn.StartsWith("fm"))
                    {
                        int i = Convert.ToInt32(_searchIn.Substring(3));
                        t1 = t1.Where(a => a.Id == i);
                        t2 = t2.Where(a => a.Id == i);
                        t3 = t3.Where(a => a.tForumThemes.Id == i);
                    }
                }
            }
           
            if (_searchUser != null && _searchUser != "")
            {
                if (db.Users.Where(a => a.Name_Org.ToUpper() == _searchUser.ToUpper()).Count() > 0)
                {
                    t1 = t1.Where(a => a.tUsers.Name_Org.ToUpper() == _searchUser.ToUpper());
                    t2 = t2.Where(a => a.tUsers.Name_Org.ToUpper() == _searchUser.ToUpper());
                    t3 = t3.Where(a => a.tUsers.Name_Org.ToUpper() == _searchUser.ToUpper());
                }
                else
                {
                    t1 = t1.Where(a => a.tUsers.NickName.ToUpper() == _searchUser.ToUpper());
                    t2 = t2.Where(a => a.tUsers.NickName.ToUpper() == _searchUser.ToUpper());
                    t3 = t3.Where(a => a.tUsers.NickName.ToUpper() == _searchUser.ToUpper());
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
            if (_searchFrom != null && _searchFrom != "")
            {
                Convert.ToDateTime(_searchFrom);
                DateTime s = Convert.ToDateTime(_searchFrom);
                t1 = t1.Where(a => a.tForumThemes_datetime >= s);
                t2 = t1.Where(a => a.tForumThemes_datetime >= s);
                t3 = t3.Where(a => a.tForumMessages_datetime >= s);
            }
            if (_searchTo != null && _searchTo != "")
            {
                Convert.ToDateTime(_searchTo);
                DateTime s = Convert.ToDateTime(_searchTo);
                t1 = t1.Where(a => a.tForumThemes_datetime <= s);
                t2 = t1.Where(a => a.tForumThemes_datetime <= s);
                t3 = t3.Where(a => a.tForumMessages_datetime <= s);
            }
            if (_searchOrder != null && _searchOrder != "")
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
