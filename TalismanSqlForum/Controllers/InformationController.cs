using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TalismanSqlForum.Controllers
{
    public class InformationController : Controller
    {
        // GET: Information
        public ActionResult Rules()
        {
            return View();
        }
    }
}