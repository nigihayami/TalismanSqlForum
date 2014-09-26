using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TalismanSqlForum.Models;
using TalismanSqlForum.Models.Notification;

namespace TalismanSqlForum.Controllers.Notify
{
    public class NotificationsController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        // GET: Notifications
        public ActionResult Index(string username)
        {
            var d = _db.tNotification.Where(a => a.tUsers.UserName == username).Where(a => !a.tNotification_IsRead).ToList();
            ViewData["notif"] = d;
            return View();
        }
        [Authorize]
        public ActionResult New()
        {
            var username = User.Identity.Name;
            var d = new List<tNotification>();
            foreach (var item in _db.tNotification.Where(a => a.tUsers.UserName == username).Where(a => !a.tNotification_IsRead))
            {
                item.tNotification_IsRead = true;
                _db.Entry(item).State = EntityState.Modified;
                _db.SaveChanges();
                d.Add(item);
            }
            ViewData["notif"] = d;
            return View();
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
