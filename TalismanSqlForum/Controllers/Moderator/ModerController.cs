using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TalismanSqlForum.Models;
using TalismanSqlForum.Models.Moderator;
using TalismanSqlForum.Models.Offer;

namespace TalismanSqlForum.Controllers.Moderator
{
    [Authorize(Roles = "admin,moderator")]
    public class ModerController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Settings(string returnUrl)
        {
            ViewData["returnUrl"] = returnUrl;

            foreach (var item2 in db.tModerator.Where(a => a.tUsers.UserName == User.Identity.Name))
            {
                //смотрим настройки модератора - если мы здесь, то ясень пень что они есть
                return View(item2);
            }
            //ну а если продолжение - то создадим новые
            var t = new tModerator();
            return View(t);
        }
        [HttpPost]
        public ActionResult Settings (string returnUrl, tModerator tmoderator)
        {
            var username = User.Identity.Name;
            foreach(var item in db.tModerator.Where(a => a.tUsers.UserName == username))
            {
                //у нас уже сужествуют настройки
                item.tModerator_database = @"85.175.98.196:bt";
                item.tModerator_password = tmoderator.tModerator_password;
                item.tModerator_userId = tmoderator.tModerator_userId;
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
                if (!TryConnect(username))
                {
                    item.tModerator_database = @"192.168.1.250:bt";
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();
                    if(!TryConnect(username))
                    {
                        ModelState.AddModelError("", "Невозможно подключиться к базе БТ, проверьте настройки соединения");
                        return View(item);
                    }
                }
                return RedirectToLocal(returnUrl);
            }
            //настроек нет, значит создадим
            var t = new tModerator();
            t.tModerator_database = @"85.175.98.196:bt";
            t.tModerator_password = tmoderator.tModerator_password;
            t.tModerator_userId = tmoderator.tModerator_userId;
            t.tUsers = db.Users.Where(a => a.UserName == username).First();
            db.tModerator.Add(t);
            db.SaveChanges();
            if (!TryConnect(username))
            {
                t.tModerator_database = @"192.168.1.250:bt";
                db.Entry(t).State = EntityState.Modified;
                db.SaveChanges();
                if (!TryConnect(username))
                {
                    ModelState.AddModelError("", "Невозможно подключиться к базе БТ, проверьте настройки соединения");
                    return View(t);
                }
            }
            return RedirectToLocal(returnUrl);
        }

        public ActionResult CreateOffer(int id_mess, int id_theme)
        {
            var returnUrl = Url.Action("CreateOffer", new { id_mess = id_mess, id_theme = id_theme });
            //попробуем для начала подсоедениться к БТ
            if (TryConnect(User.Identity.Name))
            {
                var t = db.tForumMessages.Find(id_mess);
                if (t != null)
                {
                    //создаем замечание на основании сообщения
                    var toffer = new tOffer();
                    toffer.tforummessages = t;
                    //теперь нам нужны данные для комбиков
                    ViewData["tBranch"] = db.tBranch.ToList();
                    ViewData["tProject"] = db.tProject.ToList();
                    ViewData["tReleaseProject"] = db.tReleaseProject.ToList();
                    ViewData["tReleaseProject_exec"] = db.tReleaseProject.ToList();
                    ViewData["tSubsystem"] = db.tSubsystem.ToList();
                    ViewData["id_mess"] = id_mess;
                    ViewData["id_theme"] = id_theme;
                    return View(toffer);
                }
                var tm = db.tForumThemes.Find(id_theme);
                if (tm != null)
                {
                    //создаем замечание на основании сообщения
                    var toffer = new tOffer();
                    toffer.tforumthemes = tm;
                    //теперь нам нужны данные для комбиков
                    ViewData["tBranch"] = db.tBranch.ToList();
                    ViewData["tProject"] = db.tProject.ToList();
                    ViewData["tReleaseProject"] = db.tReleaseProject.ToList();
                    ViewData["tReleaseProject_exec"] = db.tReleaseProject.ToList();
                    ViewData["tSubsystem"] = db.tSubsystem.ToList();
                    ViewData["id_mess"] = id_mess;
                    ViewData["id_theme"] = id_theme;
                    return View(toffer);
                }
                return HttpNotFound();
            }
            return RedirectToAction("Settings", new { returnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOffer([Bind(Include = "tOffer_error,tOffer_place,tOffer_tBranch_id,tOffer_tProject_id,tOffer_tReleaseProject_id,tOffer_tReleaseProject_exec_id,tOffer_tSubsystem_id")] 
                                        int id_mess, 
                                        int id_theme, tOffer toffer)
        {
            var val = this.Url.RequestContext.HttpContext.Request.Url.Scheme;
            var returnUrl = "";
            string href = "";
            if (id_mess != 0)
            {
                var t = db.tForumMessages.Find(id_mess);
                toffer.tforummessages = t;
                db.tOffer.Add(toffer);
                db.SaveChanges();
                returnUrl = Url.Action("Index", "ForumMessages", new { id = t.tForumThemes.Id, id_list = t.tForumThemes.tForumList.Id });
                href = Url.Action("Index", "ForumMessages", new { id = t.tForumThemes.Id, id_list = t.tForumThemes.tForumList.Id }, val);
            }
            else
            {
                var t = db.tForumThemes.Find(id_theme);
                toffer.tforumthemes = t;
                db.tOffer.Add(toffer);
                db.SaveChanges();
                returnUrl = Url.Action("Index", "ForumThemes", new { id = t.tForumList.Id });
                href = Url.Action("Index", "ForumThemes", new { id = t.tForumList.Id }, val);
            }
            //и теперь мы создаем замечание - выделим отдельно, так как там много кода
            toffer.tOffer_docnumber =  create(toffer.Id,User.Identity.Name,href);
            db.Entry(toffer).State = EntityState.Modified;
            db.SaveChanges();
            
            return RedirectToLocal(returnUrl);
        }

        static bool TryConnect(string username)
        {
            bool res = false;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                foreach(var item in db.tModerator.Where(a => a.tUsers.UserName == username))
                {
                    try
                    {
                        FbConnectionStringBuilder fc = new FbConnectionStringBuilder();
                        fc.UserID = item.tModerator_userId;
                        fc.Password = item.tModerator_password;
                        fc.Database = item.tModerator_database;
                        fc.Pooling = false;
                        fc.Charset = "win1251";
                        using (FbConnection fb = new FbConnection(fc.ConnectionString))
                        {
                            try
                            {
                                fb.Open();
                                fb.Close();
                                res = true;
                            }
                            catch { }
                            fb.Dispose();
                        }
                    }
                    catch { }
                }
                db.Dispose();
            }
            return res;
        }
        public JsonResult get_data(int id, string _table)
        {
            List<val> result = new List<val>();
            switch(_table)
            {
                case "RELEASE_PROJECTS":
                    foreach(var item in db.tProject.Find(id).treleaseproject.OrderByDescending(a=> a.tReleaseProject_name))
                    {
                        var v = new val { id = item.Id, name = item.tReleaseProject_name };
                        result.Add(v);
                    }
                    break;
                case "SUBSYSTEMS":
                    foreach (var item in db.tProject.Find(id).tsubsystem.OrderBy(a=>a.tSubsystem_name))
                    {
                        var v = new val { id = item.Id, name = item.tSubsystem_name };
                        result.Add(v);
                    }
                    break;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "ForumList");
            }
        }

        static int create(int id, string username, string href)
        {
            int doc_number = 0;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var t = db.tOffer.Find(id);
                var m = db.tModerator.Where(a => a.tUsers.UserName == username).First();
                var fc = new FbConnectionStringBuilder();
                fc.UserID = m.tModerator_userId;
                fc.Password = m.tModerator_password;
                fc.Database = m.tModerator_database;
                fc.Charset = "win1251";
                fc.Pooling = false;
                fc.Role = "R_ADMIN";
                
                using (FbConnection fb = new FbConnection(fc.ConnectionString))
                {
                    try
                    {
                        fb.Open();
                        using (FbTransaction ft = fb.BeginTransaction())
                        {
                            using (FbCommand fcon = new FbCommand("select G.NUM " +
        "from GET_DC_DOCUMENT_NUMBER(85) G", fb, ft))
                            {
                                using (FbDataReader fr = fcon.ExecuteReader())
                                {
                                    while (fr.Read())
                                    {
                                        doc_number = (int)fr[0];
                                    }
                                    fr.Dispose();
                                }
                                fcon.Dispose();
                            }

                            var _com = "execute procedure IUD_BUGS('I', null, @IS_ERROR, @LOCATION, @ID_RELEASE_PROJECTS, @ID_RELEASE_PROJECTS_EXEC," +
                            " null, null, @ID_SUBSYSTEM," +
                            "@ID_BRANCH, null, 85, 1, (select list_id from sel_filter_budg(2)),@DOC_NUMBER, 'NOW'," +
                            "null, @comment, null, @DETAIL_COMMENT, 1, @ID_PROJECTS, null,null) ";
                            using (FbCommand fcon = new FbCommand(_com, fb, ft))
                            {

                                switch (t.tOffer_error)
                                {
                                    case true:
                                        fcon.Parameters.AddWithValue("@IS_ERROR", '1');
                                        break;
                                    default:
                                        fcon.Parameters.AddWithValue("@IS_ERROR", '0');
                                        break;
                                }
                                fcon.Parameters.AddWithValue("@LOCATION", t.tOffer_place);
                                fcon.Parameters.AddWithValue("@ID_RELEASE_PROJECTS", t.tOffer_tReleaseProject_id);
                                fcon.Parameters.AddWithValue("@ID_RELEASE_PROJECTS_EXEC", t.tOffer_tReleaseProject_exec_id);
                                fcon.Parameters.AddWithValue("@ID_SUBSYSTEM", t.tOffer_tSubsystem_id);
                                fcon.Parameters.AddWithValue("@ID_BRANCH", t.tOffer_tBranch_id);
                                if (t.tforummessages != null)
                                {                                    
                                    fcon.Parameters.AddWithValue("@comment", t.tforummessages.tForumThemes.tForumThemes_name);
                                    fcon.Parameters.AddWithValue("@DETAIL_COMMENT",
                                        "<em><a href ='" + href + "'> " +
                                       href +
                                       "</a></em>" +
                                        "<p>" + t.tforummessages.tForumMessages_messages + "</p>");
                                }
                                else
                                {
                                    fcon.Parameters.AddWithValue("@comment", t.tforumthemes.tForumThemes_name);
                                    fcon.Parameters.AddWithValue("@DETAIL_COMMENT",
                                        "<em><a href ='" +
                                        href + 
                                        "'> " +
                                        href + 
                                        "</a></em>" +
                                        "<p>" + t.tforumthemes.tForumThemes_desc + "</p>");
                                }

                                fcon.Parameters.AddWithValue("@ID_PROJECTS", t.tOffer_tProject_id);

                                fcon.Parameters.AddWithValue("@DOC_NUMBER", doc_number);
                                try
                                {
                                    fcon.ExecuteNonQuery();
                                    ft.Commit();
                                }
                                catch (FbException ex1)
                                {
                                    ft.Rollback();
                                }
                                finally
                                {
                                    fcon.Dispose();
                                }
                            }
                            ft.Dispose();
                        }
                    }
                    catch { }
                    finally
                    {
                        fb.Close();
                    }
                    fb.Dispose();
                }
                db.Dispose();
            }
            return doc_number;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
