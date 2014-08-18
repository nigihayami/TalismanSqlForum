using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TalismanSqlForum.Models;
using TalismanSqlForum.Models.Admin;

namespace TalismanSqlForum.Controllers.Admin
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Users()
        {
            var t = db.Users.Where(a => a.UserName != "admin@talismansql.ru").ToList();
            ViewData["_users"] = t;
            return View();
        }
        public ActionResult UsersGet(string UserId)
        {
            var t = db.Users.Where(a => a.UserName == UserId).First();
            ViewData["username"] = t.UserName;
            var r = new ViewRole();
            foreach (var item in db.Roles.ToList())
            {
                switch (item.Name)
                {
                    case "admin":
                        if (t.Roles.Where(a => a.RoleId == item.Id).Count() > 0)
                        {
                            r.is_admin = true;
                        }
                        break;
                    case "moderator":
                        if (t.Roles.Where(a => a.RoleId == item.Id).Count() > 0)
                        {
                            r.is_moderator = true;
                        }
                        break;
                    case "user":
                        if (t.Roles.Where(a => a.RoleId == item.Id).Count() > 0)
                        {
                            r.is_user = true;
                        }
                        break;
                }
            }
            ViewData["userrole"] = r;
            return View();
        }
        public JsonResult set_UsersRoles(string UserId, string RoleName)
        {
            var context = new TalismanSqlForum.Models.ApplicationDbContext();
            var store = new UserStore<ApplicationUser>(context);
            var manager = new UserManager<ApplicationUser>(store);

            var userId = db.Users.Where(a => a.UserName == UserId).First();
            var roleId = db.Roles.Where(a => a.Name == RoleName).First();
            if (userId.Roles.Where(a => a.RoleId == roleId.Id).Count() != 0)
            {
                manager.RemoveFromRole(userId.Id, RoleName);
            }
            else
            {
                manager.AddToRole(userId.Id, RoleName);
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}