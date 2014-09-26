using System.Linq;
using System.Web.Mvc;
using TalismanSqlForum.Models;

namespace TalismanSqlForum.Controllers.Admin
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        readonly ApplicationDbContext _db = new ApplicationDbContext();
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }      

        public ActionResult Forum()
        {
            ViewData["tForumList"] = _db.tForumLists.OrderBy(a => a.tForumList_name).ToList();
            return View();
        }

        public ActionResult Bt(string mes)
        {
            ViewData["mes"] = mes;
            return View();
        }
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}