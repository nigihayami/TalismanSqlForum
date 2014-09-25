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
                var t = db.tUserNewThemes.Where(a => a.tUsers.UserName == username).Where(b => b.tForumThemes.tForumList.Id == id).Count();
                return t;
            }
        }
        public static int count_mess(string username, int id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var t = db.tUserNewThemes.Where(a => a.tUsers.UserName == username).Where(b => b.tForumThemes.Id == id).Count();
                return t;
            }
        }
        public static int count_mess_list(string username, int id)
        {
            //id = forumlist_id
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var t = db.tUserNewMessages.Where(a => a.tUsers.UserName == username).Where(a => a.tForumMessages.tForumThemes.tForumList.Id == id).Count() ;
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
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var d = DateTime.Now.AddDays(-1);
                var d2 = DateTime.Now.AddDays(-2);
                t.count_list = db.tForumLists.Where(a => !a.tForumList_hide).Count();
                t.count_NewForumThemes = db.tForumThemes.Where(a => a.tForumThemes_datetime >= d).Count();
                t.count_NewForumMessages = db.tForumMessages.Where(a => a.tForumMessages_datetime >= d).Count();
                t.count_OldForumThemes = db.tForumThemes.Where(a => a.tForumThemes_datetime >= d2).Where(b => b.tForumThemes_datetime <= d).Count();
                t.count_OldForumMessages = db.tForumMessages.Where(a => a.tForumMessages_datetime >= d2).Where(b => b.tForumMessages_datetime <= d).Count();
                t.count_View = 0; //я пока не знаю как это сделать
                t.count_Users = db.Users.Count();
                t.count_ForumMessages = db.tForumMessages.Count();
                t.count_ForumThemes = db.tForumThemes.Count();
                var i = 0;
                foreach (var item in db.tForumMessages.GroupBy(a => a.tUsers).OrderByDescending(a => a.Count()))
                {
                    if (t.mega_User != "" && t.mega_User != null)
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
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var d = DateTime.Now.AddDays(-1);
                var d2 = DateTime.Now.AddDays(-2);
                t.count_NewForumThemes = db.tForumThemes.Where(a => a.tForumList.Id == id).Where(a => a.tForumThemes_datetime >= d).Count();
                t.count_NewForumMessages = db.tForumMessages.Where(a => a.tForumThemes.tForumList.Id == id).Where(a => a.tForumMessages_datetime >= d).Count();
                t.count_OldForumThemes = db.tForumThemes.Where(a => a.tForumList.Id == id).Where(a => a.tForumThemes_datetime >= d2).Where(b => b.tForumThemes_datetime <= d).Count();
                t.count_OldForumMessages += db.tForumMessages.Where(a => a.tForumThemes.tForumList.Id == id).Where(a => a.tForumMessages_datetime >= d2).Where(b => b.tForumMessages_datetime <= d).Count();
                t.count_View = 0; //я пока не знаю как это сделать
                t.count_Users = db.tForumMessages.Where(a => a.tForumThemes.tForumList.Id == id).GroupBy(a => a.tUsers).Count();
                t.count_ForumMessages = db.tForumMessages.Where(a => a.tForumThemes.tForumList.Id == id).Count();
                t.count_ForumThemes = db.tForumThemes.Where(a => a.tForumList.Id == id).Count();
                db.Dispose();
            }
            return t;
        }
    }
}