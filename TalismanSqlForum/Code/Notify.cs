using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TalismanSqlForum.Models;
using TalismanSqlForum.Models.Notification;

namespace TalismanSqlForum.Code
{
    public class Notify
    {
        public static void NewUser(string id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var t = new tNotification();
                t.tNotificationType = db.tNotificationType.Find(1); //Новый пользователь
                t.tNotification_date = System.DateTime.Now;
                t.tNotification_IsRead = false;
                t.tNotification_message = "В системе появился новый пользователь - " + db.Users.Find(id).NickName;
                foreach(var item in db.Roles.Where(a=> a.Name == "admin"))
                {
                   foreach(var item2 in item.Users)
                   {
                       t.tUsers = db.Users.Find(item2.UserId);
                       db.tNotification.Add(t);
                       db.SaveChanges();
                   }
                }
                db.Dispose();
            }
        }
        public static void NewThemes(int id, string href)
        {
            using(ApplicationDbContext db = new ApplicationDbContext())
            {
                var ft = db.tForumThemes.Find(id);
                var t = new tNotification();
                t.tNotificationType = db.tNotificationType.Find(2); //Новая тема
                t.tNotification_date = System.DateTime.Now;
                t.tNotification_IsRead = false;
                t.tNotification_message = "Новая тема на форуме " + ft.tForumList.tForumList_name + "\n" + "\"" + ft.tForumThemes_name+ "\"";
                t.tNotification_href = href;
                //Отсылаем модераторам
                foreach (var item in db.Roles.Where(a => a.Name == "moderator"))
                {
                    foreach (var item2 in item.Users)
                    {
                        t.tUsers = db.Users.Find(item2.UserId);
                        db.tNotification.Add(t);
                        db.SaveChanges();
                    }
                }
                db.Dispose();
            }
        }
        public static void NewMessage(int id, string href)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var ft = db.tForumMessages.Find(id);
                var t = new tNotification();
                t.tNotificationType = db.tNotificationType.Find(3); //Новое сообщение
                t.tNotification_date = System.DateTime.Now;
                t.tNotification_IsRead = false;
                t.tNotification_message = "Новое сообщение на форуме " + ft.tForumThemes.tForumList.tForumList_name+ " в разделе " + "\"" + ft.tForumThemes.tForumThemes_name + "\"";
                t.tNotification_href = href;
                //Отсылаем модераторам
                foreach (var item in db.Roles.Where(a => a.Name == "moderator"))
                {
                    foreach (var item2 in item.Users)
                    {
                        t.tUsers = db.Users.Find(item2.UserId);
                        db.tNotification.Add(t);
                        db.SaveChanges();
                    }
                }
                foreach(var item in ft.tForumThemes.tForumMessages.Select(a => a.tUsers).Distinct())
                {
                    t.tUsers = db.Users.Find(item.Id);
                    db.tNotification.Add(t);
                    db.SaveChanges();
                }
                db.Dispose();
            }
        }

        public static int Count(string username)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                int c = db.tNotification.Where(a => a.tUsers.UserName == username).Where(a => !a.tNotification_IsRead).Count();
                return c;
            }
        }
    }
}