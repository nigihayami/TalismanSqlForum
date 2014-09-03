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
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Notifications
        public ActionResult Index(string username)
        {
            var d = new List<tNotification>();
            foreach (var item in db.tNotification.Where(a => a.tUsers.UserName == username).Where(a => !a.tNotification_IsRead))
            {
                //item.tNotification_IsRead = true;
                //db.Entry(item).State = EntityState.Modified;
                //db.SaveChanges();
                d.Add(item);
            }
            ViewData["notif"] = d;
            return View();
        }
        [Authorize]
        public ActionResult New()
        {
            var username = User.Identity.Name;
            var d = new List<tNotification>();
            foreach (var item in db.tNotification.Where(a => a.tUsers.UserName == username).Where(a => !a.tNotification_IsRead))
            {
                item.tNotification_IsRead = true;
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
                d.Add(item);
            }
            ViewData["notif"] = d;
            return View();
        }
        [Authorize]
        [HttpPost]
        public ActionResult New(int id)
        {
            var t = db.tNotification.Find(id);
            if (t != null)
            {
                //Помечаем к прочтению
                t.tNotification_IsRead = true;
                db.Entry(t).State = EntityState.Modified;
                db.SaveChanges();
                //Берем адрес
                var url = t.tNotification_href;
                if (url != null && url != "")
                {
                    return Redirect(url);
                }
                else
                {
                    return RedirectToAction("New", "Notifications");
                }
            }
            return HttpNotFound();
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
