using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using TalismanSqlForum.Models;
using TalismanSqlForum.Models.Admin;

namespace TalismanSqlForum.Controllers.Admin
{
    [Authorize(Roles="admin")]
    public class RulesController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        // GET: Rules
        [AllowAnonymous]
        public ActionResult Rules()
        {
            return View(_db.tRules.First());
        }

        public ActionResult Edit()
        {
            var t = _db.tRules.First();
            return View(t);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,tRules_rules")] tRules trules)
        {
            if (!ModelState.IsValid) return View(trules);
            _db.Entry(trules).State = EntityState.Modified;
            _db.SaveChanges();
            return RedirectToAction("Rules");
        }
    }
}
