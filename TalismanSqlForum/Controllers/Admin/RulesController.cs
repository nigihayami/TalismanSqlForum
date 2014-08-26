using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TalismanSqlForum.Models;
using TalismanSqlForum.Models.Admin;

namespace TalismanSqlForum.Controllers.Admin
{
    [Authorize(Roles="admin")]
    public class RulesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Rules
        [AllowAnonymous]
        public ActionResult Rules()
        {
            return View(db.tRules.First());
        }

        public ActionResult Edit()
        {
            var t = db.tRules.First();
            return View(t);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,tRules_rules")] tRules trules)
        {
            if (ModelState.IsValid)
            {
                db.Entry(trules).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                RedirectToAction("Index");
            }
            return View(trules);
        }
    }
}
