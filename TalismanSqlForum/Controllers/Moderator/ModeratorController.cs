using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TalismanSqlForum.Models;

using FirebirdSql.Data.FirebirdClient;
using TalismanSqlForum.Models.ViewModel;
using TalismanSqlForum.Models.Moderator;
using TalismanSqlForum.Models.Forum;

namespace TalismanSqlForum.Controllers.Moderator
{
    [Authorize(Roles = "admin,moderator")]
    public class ModeratorController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        //
        // GET: /Moderator/

        public ActionResult Settings()
        {
            var tu = db.Users.Where(a => a.UserName == User.Identity.Name).First().tModerator;
            if (tu.Count == 0)
            {
                tModerator d = new tModerator();
                d.tUsers = db.Users.Where(a => a.UserName == User.Identity.Name).First();
                db.tModerator.Add(d);
                db.SaveChanges();
                return View(d);
            }
            else
            {
                var d = tu.First();
                return View(d);
            }
        }

        [HttpPost]
        public ActionResult Settings([Bind(Include = "Id,tModerator_database,tModerator_userId,tModerator_password")] tModerator tt)
        {
            tt.tModerator_database = @"85.175.98.196:bt";
            if (ModelState.IsValid)
            {
                
                db.Entry(tt).State = EntityState.Modified;
                db.SaveChanges();
                //первый прогон - попробуем соединится если интеренет
                if (!TryConnect(tt.Id))
                {
                    //если мы попали сюда, то интеренет не прокатил - сделаем лок
                    tt.tModerator_database = @"192.168.1.250:bt";
                    db.Entry(tt).State = EntityState.Modified;
                    db.SaveChanges();
                    if (!TryConnect(tt.Id))
                    {
                        //Ну значит вообще не прокатило
                        return View(tt);
                    }
                }
            }
            else
            {
                return View(tt);
            }

            return RedirectToAction("Index", "ForumList");
        }

        public ActionResult CreateOffer(int id)
        {
            var tu = db.Users.Where(a => a.UserName == User.Identity.Name).First().tModerator;
            if (tu.Count == 0)
            {
                return RedirectToAction("Settings");
            }
            else
                if(!TryConnect(tu.First().Id))
                {
                    return RedirectToAction("Settings");
                }
            tForumMessages tm = db.tForumMessages.Find(id);
            Models.ViewModel.CreateOffer c = new Models.ViewModel.CreateOffer();
            c._message = tm;

            return View(c);
        }
        [HttpPost]
        public ActionResult CreateOffer(CreateOffer t)
        {
            int doc_number = 0;
            var tu = db.Users.Where(a => a.UserName == User.Identity.Name).First().tModerator;
            if (tu.Count != 0)
            {
                FbConnectionStringBuilder fc = new FbConnectionStringBuilder();
                fc.Database = tu.First().tModerator_database;
                fc.UserID = tu.First().tModerator_userId;
                fc.Password = tu.First().tModerator_password;
                fc.Role = "R_ADMIN";
                fc.Pooling = false;
                fc.Charset = "win1251";
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

                                switch (t._iserror)
                                {
                                    case true:
                                        fcon.Parameters.AddWithValue("@IS_ERROR", '1');
                                        break;
                                    default:
                                        fcon.Parameters.AddWithValue("@IS_ERROR", '0');
                                        break;
                                }
                                fcon.Parameters.AddWithValue("@LOCATION", t._location);
                                fcon.Parameters.AddWithValue("@ID_RELEASE_PROJECTS", t.id_release_projects);
                                fcon.Parameters.AddWithValue("@ID_RELEASE_PROJECTS_EXEC", t.id_release_projects_exec);
                                fcon.Parameters.AddWithValue("@ID_SUBSYSTEM", t.id_subsystem);
                                fcon.Parameters.AddWithValue("@ID_BRANCH", t.id_branch);
                                fcon.Parameters.AddWithValue("@comment", t._message.tForumThemes.tForumThemes_name);
                                fcon.Parameters.AddWithValue("@DETAIL_COMMENT",
                                    "<em><a href ='" +
                                    Url.HttpRouteUrl("default",new{id =  t._message.tForumThemes.Id}) +
                                    //string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~")) +
                                    //Url.Action("Index", "ForumMessages", new { id = t._message.tForumThemes.Id }) +
                                    "'> " +
                                    Url.Action("Index", "ForumMessages", new { id = t._message.tForumThemes.Id }) + "</a></em>" +


                                    "<p>" + t._message.tForumMessages_messages + "</p>");
                                fcon.Parameters.AddWithValue("@ID_PROJECTS", t.id_projects);

                                fcon.Parameters.AddWithValue("@DOC_NUMBER", doc_number);
                                try
                                {
                                    fcon.ExecuteNonQuery();
                                    ft.Commit();
                                }
                                catch (FbException ex1)
                                {
                                    ModelState.AddModelError("", ex1.Message);
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
                    catch (FbException ex)
                    {
                        ModelState.AddModelError("", ex.Message);
                    }
                    finally
                    {
                        fb.Close();
                    }
                    fb.Dispose();
                }
            }
            else
            {
                ModelState.AddModelError("", "Настройте соединение с БТ");
            }
            if (ModelState.IsValid)
            {
                tForumMessages tm = db.tForumMessages.Find(t._message.Id);
                tm.tForumMessages_offer += "<p>Поставлено замечание № " + doc_number + " от " + DateTime.Now.ToString("dd MM yyyy") + "</p>";
                db.Entry(tm).State = EntityState.Modified;
                db.SaveChanges();
                //Перенаправим на основу сообщений
                return RedirectToAction("Index", "ForumMessages", new { id = t._message.tForumThemes.Id });
            }
            return View(t);
        }

        public bool TryConnect(int id)
        {
            bool _yes = false;
            try
            {
                var t = db.tModerator.Find(id);
                if (t != null)
                {
                    FbConnectionStringBuilder fc = new FbConnectionStringBuilder();
                    fc.Database = t.tModerator_database;
                    fc.UserID = t.tModerator_userId;
                    fc.Password = t.tModerator_password;
                    fc.Pooling = false;
                    fc.Charset = "win1251";
                    using (FbConnection fb = new FbConnection(fc.ConnectionString))
                    {
                        try
                        {
                            fb.Open();
                            _yes = true;
                        }
                        catch
                        {
                            _yes = false;
                        }
                        finally
                        {
                            fb.Close();
                        }
                        fb.Dispose();
                    }                    
                }
            }
            catch
            {
                _yes = false;
            }
            return _yes;
        }
        public JsonResult get_release_projects(int id, string _table)
        {
            List<val> result = new List<val>();
            var tu = db.Users.Where(a => a.UserName == User.Identity.Name).First().UserName;
            try
            {
                foreach (var i in rep.get(_table, id, tu))
                {
                    result.Add(i);
                }
            }
            catch
            {
                result.Add(new val { id = -1, name = "Не удалось соедениться с БТ" });
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}
