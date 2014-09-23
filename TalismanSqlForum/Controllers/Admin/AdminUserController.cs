using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using TalismanSqlForum.Models;

namespace TalismanSqlForum.Controllers.Admin
{
    [Authorize(Roles = "admin")]
    public class AdminUserController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Users(string val)
        {
            ViewData["users_new"] = db.Users.Where(a => a.IsNew).ToList();
            ViewData["users_wait"] = db.Users.Where(a => !a.IsNew).Where(a => a.Roles.Count() == 0).ToList();
            List<ApplicationUser> um = new List<ApplicationUser> { };
            List<ApplicationUser> uo = new List<ApplicationUser> { };
            if (val != null && val != "")
            {
                string _val = val;
                foreach (var item in db.Roles.Where(a => a.Name == "moderator"))
                {
                    foreach (var item2 in item.Users)
                    {
                        if (db.Users.Find(item2.UserId).Email.Contains(_val) || db.Users.Find(item2.UserId).NickName.Contains(_val))
                        {
                            um.Add(db.Users.Find(item2.UserId));
                        }
                    }
                }
                foreach (var item in db.Roles.Where(a => a.Name == "user"))
                {
                    foreach (var item2 in item.Users)
                    {
                        if (db.Users.Find(item2.UserId).Email.Contains(_val) || db.Users.Find(item2.UserId).NickName.Contains(_val))
                        {
                            uo.Add(db.Users.Find(item2.UserId));
                        }
                    }
                }
            }
            else
            {
                //moderator
                foreach (var item in db.Roles.Where(a => a.Name == "moderator"))
                {
                    foreach (var item2 in item.Users)
                    {
                        um.Add(db.Users.Find(item2.UserId));
                    }
                }
                //user
                foreach (var item in db.Roles.Where(a => a.Name == "user"))
                {
                    foreach (var item2 in item.Users)
                    {
                        uo.Add(db.Users.Find(item2.UserId));
                    }
                }
            }
            
            
            ViewData["users_moderator"] = um;
            ViewData["users_other"] = uo;
            return View();
        }
        [HttpPost]
        public ActionResult AuthUser(string id, int auth)
        {
            var t = db.Users.Find(id);
            if (t != null)
            {
                //Ставим, что пользователь просмотрен администратором
                t.IsNew = false;
                db.Entry(t).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                //А теперь назначим роль пользователь
                if (auth == 1)
                {
                    setRole(id, "user");
                }
            }
            return RedirectToAction("Users", "AdminUser");
        }
        [HttpPost]
        public ActionResult DelUser(string id)
        {
            foreach(var item in db.Users.Find(id).Roles)
            {
                var RoleName = db.Roles.Find(item.RoleId).Name;
                setRole(id,RoleName);
            }
            return RedirectToAction("Users","AdminUser");
        }
        [HttpPost]
        public ActionResult ModUser(string id)
        {
            foreach (var item in db.Users.Find(id).Roles)
            {
                var RoleName = db.Roles.Find(item.RoleId).Name;
                setRole(id, RoleName);
            }
            setRole(id, "moderator");
            return RedirectToAction("Users", "AdminUser");
        }

        public void setRole(string id, string RoleName)
        {
            var context = new TalismanSqlForum.Models.ApplicationDbContext();
            var store = new UserStore<ApplicationUser>(context);
            var manager = new UserManager<ApplicationUser>(store);
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            var userId = db.Users.Find(id);
            var roleId = db.Roles.Where(a => a.Name == RoleName).First();
            if (userId.Roles.Where(a => a.RoleId == roleId.Id).Count() != 0)
            {
                manager.RemoveFromRole(userId.Id, RoleName);
            }
            else
            {
                var roleresult = manager.AddToRole(userId.Id, RoleName);
                //Отправляем письмо, о том, что пользователя авторизовали
                MailMessage mail = new MailMessage();
                mail.Subject = "авторизация пройдена";
                mail.Body = "<p>Уважаемы пользователь " + userId.NickName + "</p>";
                mail.Body = "<p>Вы авторизированы на форуме программы Талисман-SQL</p>";

                switch (RoleName)
                {
                    case "user":
                        mail.Body += "<p>Теперь вы можете создавать темы в разделах форума, а также писать сообщения</p>";
                        break;
                    case "moderator":
                        mail.Body += "<p>Теперь вы можете создавать темы в разделах форума, создавать сообщения, закреплять и закрывать темы</p>";
                        break;

                }
                mail.IsBodyHtml = true;
                mail.To.Add(userId.Email);
                TalismanSqlForum.Code.Mail.SendEmail(mail);
            }
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
