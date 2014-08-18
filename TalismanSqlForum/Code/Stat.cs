using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TalismanSqlForum.Models;

namespace TalismanSqlForum.Code
{
    public class Stat
    {
        public static int count_theme(string username, int id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var d = db.Users.Where(a => a.UserName == username).First().LastIn;
                var t = db.tForumMessages.Where(b => b.tForumThemes.tForumList.Id == id).Where(a => a.tForumMessages_datetime >= d).Count();
                return t;
            }
        }
        public static int count_mess(string username, int id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var d = db.Users.Where(a => a.UserName == username).First().LastIn;
                var t = db.tForumMessages.Where(b => b.tForumThemes.Id == id).Where(a => a.tForumMessages_datetime >= d).Count();
                return t;
            }
        }

        public class Stat_Forum
        {
            public int tForumThemes_Count { get; set; }
            public int tForumMessages_Count { get; set; }
            public int ApplicationUser_Count { get; set; }
            public string LastUser { get; set; }
            public int Max_User_login_count { get; set; }
            public string Date_User_login_count { get; set; }
        }
        public static Stat_Forum get_stat()
        {
            var t = new Stat_Forum();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                t.tForumMessages_Count = db.tForumMessages.Count();
                t.tForumThemes_Count = db.tForumThemes.Count();
                t.ApplicationUser_Count = db.Users.Count();
                t.LastUser = db.Users.OrderByDescending(a => a.DateReg).First().UserName;
                //t.Max_User_login_count = db.Users.GroupBy(a => a.LastIn.ToString("ddMMY")).Max(a => a.Count());
                //t.Date_User_login_count = db.Users.GroupBy(a => a.LastIn.ToString("ddMMY")).Max(a => a.Count()).ToString();
                db.Dispose();
            }
            return t;
        }
    }
}