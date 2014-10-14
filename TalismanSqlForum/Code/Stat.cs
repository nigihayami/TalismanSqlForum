using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TalismanSqlForum.Models;
using TalismanSqlForum.Models.Stat;

namespace TalismanSqlForum.Code
{
    public class Stat
    {
        public static int count_theme(string username, int id)
        {
            using (var db = new ApplicationDbContext())
            {
                var t = db.tUserNewThemes.Where(a => a.tUsers.UserName == username).Count(b => b.tForumThemes.tForumList.Id == id);
                return t;
            }
        }
        public static int count_mess(string username, int id)
        {
            using (var db = new ApplicationDbContext())
            {
                var t = db.tUserNewMessages.Where(a => a.tUsers.UserName == username).Count(a => a.tForumMessages.tForumThemes.Id == id);
                return t;
            }
        }
        public static int count_mess_list(string username, int id)
        {
            //id = forumlist_id
            using (var db = new ApplicationDbContext())
            {
                var t = db.tUserNewMessages.Where(a => a.tUsers.UserName == username).Count(a => a.tForumMessages.tForumThemes.tForumList.Id == id);
                return t;
            }
        }
        public class statForumList
        {
            public int count_list { get; set; }
            public int count_NewForumThemes { get; set; }
            public int count_NewForumMessages { get; set; }
            public int count_OldForumThemes { get; set; }
            public int count_OldForumMessages { get; set; }
            public int count_View { get; set; }
            public int count_Users { get; set; }
            public int count_ForumMessages { get; set; }
            public int count_ForumThemes { get; set; }
            public string mega_User { get; set; }
        }
        //Статистика форума
        public static statForumList Stat_Forum()
        {
            var t = new statForumList();
            using (var db = new ApplicationDbContext())
            {
                var d = DateTime.Now.AddDays(-1);
                var d2 = DateTime.Now.AddDays(-2);
                t.count_list = db.tForumLists.Count(a => !a.tForumList_hide);
                t.count_NewForumThemes = db.tForumThemes.Count(a => a.tForumThemes_datetime >= d);
                t.count_NewForumMessages = db.tForumMessages.Count(a => a.tForumMessages_datetime >= d);
                t.count_OldForumThemes = db.tForumThemes.Where(a => a.tForumThemes_datetime >= d2).Count(b => b.tForumThemes_datetime <= d);
                t.count_OldForumMessages = db.tForumMessages.Where(a => a.tForumMessages_datetime >= d2).Count(b => b.tForumMessages_datetime <= d);
                t.count_View = db.StatForum.Count();
                if (HttpRuntime.Cache["LoggedInUsers"] != null)
                {
                    var loggedOnUsers = (List<string>)HttpRuntime.Cache["LoggedInUsers"];
                    t.count_Users = loggedOnUsers.Count();
                }
                else
                {
                    t.count_Users = 0;
                }
                t.count_ForumMessages = db.tForumMessages.Count();
                t.count_ForumThemes = db.tForumThemes.Count();
                var i = 0;
                foreach (var item in db.tForumMessages.GroupBy(a => a.tUsers).OrderByDescending(a => a.Count()))
                {
                    if (!string.IsNullOrEmpty(t.mega_User))
                    {
                        t.mega_User += ", ";
                    }
                    t.mega_User += item.First().tUsers.NickName;
                    i++;
                    if (i == 3)
                        break;
                }
                db.Dispose();
            }
            return t;
        }
        //Статистика форумаID
        public static statForumList Stat_ForumList(int id)
        {
            var t = new statForumList();
            using (var db = new ApplicationDbContext())
            {
                var d = DateTime.Now.AddDays(-1);
                var d2 = DateTime.Now.AddDays(-2);
                t.count_NewForumThemes = db.tForumThemes.Where(a => a.tForumList.Id == id).Count(a => a.tForumThemes_datetime >= d);
                t.count_NewForumMessages = db.tForumMessages.Where(a => a.tForumThemes.tForumList.Id == id).Count(a => a.tForumMessages_datetime >= d);
                t.count_OldForumThemes = db.tForumThemes.Where(a => a.tForumList.Id == id).Where(a => a.tForumThemes_datetime >= d2).Count(b => b.tForumThemes_datetime <= d);
                t.count_OldForumMessages += db.tForumMessages.Where(a => a.tForumThemes.tForumList.Id == id).Where(a => a.tForumMessages_datetime >= d2).Count(b => b.tForumMessages_datetime <= d);
                t.count_View = db.StatForumList.Count(a => a.TForumLists.Id == id);
                t.count_Users = db.tForumMessages.Where(a => a.tForumThemes.tForumList.Id == id).GroupBy(a => a.tUsers).Count();
                t.count_ForumMessages = db.tForumMessages.Count(a => a.tForumThemes.tForumList.Id == id);
                t.count_ForumThemes = db.tForumThemes.Count(a => a.tForumList.Id == id);
                db.Dispose();
            }
            return t;
        }

        public static void SetView(string ip)
        {
            //Общее количество просмотров
            using (var db = new ApplicationDbContext())
            {
                if (!db.StatForum.Any(a => a.StatForumIp == ip))
                {
                    var t = new StatForum { StatForumIp = ip };
                    db.StatForum.Add(t);
                    db.SaveChanges();
                }
                db.Dispose();
            }
        }
        
        public static void SetViewList(string ip, int id)
        {
            //Количество просмотров конкретного форума
            using (var db = new ApplicationDbContext())
            {
                if (!db.StatForumList.Where(a => a.TForumLists.Id == id).Any(a => a.StatForumIp == ip))
                {
                    var t = new StatForumList { StatForumIp = ip, TForumLists = db.tForumLists.Find(id) };
                    db.StatForumList.Add(t);
                    db.SaveChanges();
                }
                db.Dispose();               
            }
        }
    }
}