using System.Linq;
using TalismanSqlForum.Models;
using TalismanSqlForum.Models.Notification;

namespace TalismanSqlForum.Code
{
    public static class Notify
    {
        public static void NewUser(string id)
        {
            using (var db = new ApplicationDbContext())
            {
                var t = new tNotification
                {
                    tNotificationType = db.tNotificationType.Find(1),
                    tNotification_date = System.DateTime.Now,
                    tNotification_IsRead = false,
                    tNotification_message = "В системе появился новый пользователь - " + db.Users.Find(id).NickName
                };
                foreach (var item2 in db.Roles.Where(a=> a.Name == "admin").SelectMany(item => item.Users))
                {
                    t.tUsers = db.Users.Find(item2.UserId);
                    db.tNotification.Add(t);
                    db.SaveChanges();
                }
                db.Dispose();
            }
        }
        public static void NewThemes(int id, string href, string userId)
        {
            using(var db = new ApplicationDbContext())
            {
                var ft = db.tForumThemes.Find(id);
                var t = new tNotification
                {
                    tNotificationType = db.tNotificationType.Find(2),
                    tNotification_date = System.DateTime.Now,
                    tNotification_IsRead = false,
                    tNotification_message =
                        "Новая тема на форуме " + ft.tForumList.tForumList_name + "\n" + "\"" + ft.tForumThemes_name +
                        "\"",
                    tNotification_href = href
                };
                //Отсылаем модераторам
                foreach (var item2 in db.Roles.Where(a => a.Name == "moderator").SelectMany(item => item.Users.Where(a => a.UserId != userId)))
                {
                    t.tUsers = db.Users.Find(item2.UserId);
                    db.tNotification.Add(t);
                    db.SaveChanges();
                }
                //Отсылаем администраторам.. Только я конечо я незнаю зачем все это?????!!!!!!
                foreach (var item2 in db.Roles.Where(a => a.Name == "admin").SelectMany(item => item.Users.Where(a => a.UserId != userId)))
                {
                    t.tUsers = db.Users.Find(item2.UserId);
                    db.tNotification.Add(t);
                    db.SaveChanges();
                }
                db.Dispose();
            }
        }
        public static void NewMessage(int id, string href, string userId)
        {
            using (var db = new ApplicationDbContext())
            {
                var ft = db.tForumMessages.Find(id);
                var t = new tNotification
                {
                    tNotificationType = db.tNotificationType.Find(3),
                    tNotification_date = System.DateTime.Now,
                    tNotification_IsRead = false,
                    tNotification_message =
                        "Новое сообщение на форуме " + ft.tForumThemes.tForumList.tForumList_name + " в разделе " + "\"" +
                        ft.tForumThemes.tForumThemes_name + "\"",
                    tNotification_href = href
                };
                //Отсылаем модераторам
                foreach (var item2 in db.Roles.Where(a => a.Name == "moderator").SelectMany(item => item.Users.Where(a => a.UserId != userId)))
                {
                    t.tUsers = db.Users.Find(item2.UserId);
                    db.tNotification.Add(t);
                    db.SaveChanges();
                }
                //Отсылаем администраторам
                foreach (var item2 in db.Roles.Where(a => a.Name == "admin").SelectMany(item => item.Users.Where(a => a.UserId != userId)))
                {
                    t.tUsers = db.Users.Find(item2.UserId);
                    db.tNotification.Add(t);
                    db.SaveChanges();
                }
                foreach (var item in ft.tForumThemes.tForumMessages.Select(a => a.tUsers).Distinct().Where(a => a.Id != userId).Where(item => !db.tNotification.Where(a => a.tUsers.Id == item.Id)
                    .Any(a => a.tNotification_href == t.tNotification_href)))
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
            using (var db = new ApplicationDbContext())
            {
                var c = db.tNotification.Where(a => a.tUsers.UserName == username).Count(a => !a.tNotification_IsRead);
                return c;
            }
        }
    }
}