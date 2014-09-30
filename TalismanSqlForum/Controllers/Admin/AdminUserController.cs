using System.Data;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;
using TalismanSqlForum.Models;

namespace TalismanSqlForum.Controllers.Admin
{
    [Authorize(Roles = "admin")]
    public class AdminUserController : Controller
    {
        readonly ApplicationDbContext _db = new ApplicationDbContext();
        public ActionResult Users(string val)
        {
            ViewData["users_new"] = _db.Users.Where(a => a.IsNew).ToList();
            ViewData["users_wait"] = _db.Users.Where(a => !a.IsNew).Where(a => !a.Roles.Any()).ToList();
            var um = new List<ApplicationUser>();
            var uo = new List<ApplicationUser>();
            if (!string.IsNullOrEmpty(val))
            {
                foreach (var item in _db.Roles.Where(a => a.Name == "moderator"))
                {
                    um.AddRange(_db.Users.Where(a => a.UserName == val).Where(a => a.Roles.Count(b => b.RoleId == item.Id) > 0));
                }
                foreach (var item in _db.Roles.Where(a => a.Name == "user"))
                {
                    uo.AddRange(_db.Users.Where(a=> a.UserName == val).Where(a => a.Roles.Count(b => b.RoleId == item.Id) > 0));
                }
            }
            else
            {
                foreach (var item in _db.Roles.Where(a => a.Name == "moderator"))
                {
                    um.AddRange(_db.Users.Where(a => a.Roles.Count(b => b.RoleId == item.Id) > 0));
                }
                foreach (var item in _db.Roles.Where(a => a.Name == "user"))
                {
                    uo.AddRange(_db.Users.Where(a => a.Roles.Count(b => b.RoleId == item.Id) > 0));
                }
            }


            ViewData["users_moderator"] = um;
            ViewData["users_other"] = uo;
            return View();
        }
        [HttpPost]
        public ActionResult AuthUser(string id, int auth)
        {
            var t = _db.Users.Find(id);
            if (t == null) return RedirectToAction("Users", "AdminUser");
            //Ставим, что пользователь просмотрен администратором
            t.IsNew = false;
            _db.Entry(t).State = System.Data.Entity.EntityState.Modified;
            _db.SaveChanges();
            //А теперь назначим роль пользователь
            if (auth == 1)
            {
                setRole(id, "user");
            }
            return RedirectToAction("Users", "AdminUser");
        }
        [HttpPost]
        public ActionResult DelUser(string id)
        {
            foreach (var roleName in _db.Users.Find(id).Roles.Select(item => _db.Roles.Find(item.RoleId).Name))
            {
                setRole(id,roleName);
            }
            return RedirectToAction("Users","AdminUser");
        }

        [HttpPost]
        public ActionResult ModUser(string id)
        {
            foreach (var roleName in _db.Users.Find(id).Roles.Select(item => _db.Roles.Find(item.RoleId).Name))
            {
                setRole(id, roleName);
            }
            setRole(id, "moderator");
            return RedirectToAction("Users", "AdminUser");
        }

        public void setRole(string id, string RoleName)
        {
            var context = new ApplicationDbContext();
            var store = new UserStore<ApplicationUser>(context);
            var manager = new UserManager<ApplicationUser>(store);
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            var userId = _db.Users.Find(id);
            var roleId = _db.Roles.First(a => a.Name == RoleName);
            if (userId.Roles.Count(a => a.RoleId == roleId.Id) != 0)
            {
                manager.RemoveFromRole(userId.Id, RoleName);
            }
            else
            {
                var roleresult = manager.AddToRole(userId.Id, RoleName);
                //Отправляем письмо, о том, что пользователя авторизовали
                var mail = new MailMessage
                {
                    Subject = "авторизация пройдена",
                    Body = "<p>Уважаемы пользователь " + userId.NickName + "</p>" + "<p>Вы авторизированы на форуме программы Талисман-SQL</p>"
                };

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
                Code.Mail.SendEmail(mail);
            }
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
