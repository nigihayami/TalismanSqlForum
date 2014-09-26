using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using TalismanSqlForum.Models;

namespace TalismanSqlForum.Controllers
{
    public class ForumListController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        // GET: ForumList
        public ActionResult Index()
        {
            ViewData["tForumList"] = _db.tForumLists.Where(a=> !a.tForumList_hide).OrderBy(a => a.tForumList_name).ToList();
            return View();
        }

        [Authorize(Roles="admin")]
        public ActionResult HideAllThemes(int? id)
        {
            var t = _db.tForumLists.Find(id);
            if (t == null) return HttpNotFound();
            foreach (var item in t.tForumThemes)
            {
                item.tForumThemes_hide = true;
                _db.Entry(item).State = EntityState.Modified;
                _db.SaveChanges();
            }
            return RedirectToAction("Index", "ForumThemes", new { id = id });
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
