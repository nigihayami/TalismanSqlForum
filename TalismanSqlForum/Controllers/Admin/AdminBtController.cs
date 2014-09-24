using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TalismanSqlForum.Models;
using TalismanSqlForum.Models.Offer;

namespace TalismanSqlForum.Controllers.Admin
{
    [Authorize(Roles="admin")]
    public class AdminBtController : Controller
    {
        public ActionResult Sync()
        {
            //синхронизация спарвочника БТ
            if(TryConnect(User.Identity.Name))
            {
                //начинаем формирование справочников
                var username = User.Identity.Name;
                SyncBranch(username);
                SyncProject(username);

                return RedirectToAction("Bt", "Admin", new { mes = "Синхронизация завершена"});
            }
            return RedirectToAction("Settings", "Moder", new {returnUrl = Url.Action("Sync","AdminBt") });
        }


        static bool TryConnect(string username)
        {
            bool res = false;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                foreach (var item in db.tModerator.Where(a => a.tUsers.UserName == username))
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
        #region Branch
        static void SyncBranch(string username)
        {
            using(ApplicationDbContext db = new ApplicationDbContext())
            {
                var t = db.tModerator.Where(a => a.tUsers.UserName == username).First();
                FbConnectionStringBuilder fc = new FbConnectionStringBuilder();
                fc.UserID = t.tModerator_userId;
                fc.Password = t.tModerator_password;
                fc.Database = t.tModerator_database;
                fc.Charset = "win1251";
                fc.Pooling = false;
                fc.Role = "R_ADMIN";

                using(FbConnection fb = new FbConnection(fc.ConnectionString))
                {
                    try
                    {
                        fb.Open();
                        using (FbTransaction ft = fb.BeginTransaction())
                        {
                            using (FbCommand fcon = new FbCommand("select b.id_branch, b.mnemo from branch b",fb,ft))
                            {
                                using (FbDataReader fr = fcon.ExecuteReader())
                                {
                                    while(fr.Read())
                                    {
                                        if (db.tBranch.Find(fr[0]) != null)
                                        {
                                            var m = db.tBranch.Find(fr[0]);
                                            m.tBranch_name = fr[1].ToString();
                                            db.Entry(m).State = System.Data.Entity.EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                        else
                                        {
                                            var m = new tBranch { Id = (int)fr[0], tBranch_name = fr[1].ToString() };
                                            db.tBranch.Add(m);
                                            db.SaveChanges();
                                        }
                                    }
                                    fr.Dispose();
                                }
                                fcon.Dispose();
                            }
                            ft.Commit();
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
        }
        #endregion
        #region Project
        static void SyncProject(string username)
        {
            //вывожу ее отдельно, так как здесь немного другая логика работы
            using (ApplicationDbContext db = new ApplicationDbContext())
            {

                var t = db.tModerator.Where(a => a.tUsers.UserName == username).First();
                FbConnectionStringBuilder fc = new FbConnectionStringBuilder();
                fc.UserID = t.tModerator_userId;
                fc.Password = t.tModerator_password;
                fc.Database = t.tModerator_database;
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
                            using (FbCommand fcon = new FbCommand("select p.ID_PROJECTS, p.MNEMO from PROJECTS p order by 1", fb, ft))
                            {
                                using (FbDataReader fr = fcon.ExecuteReader())
                                {
                                    while (fr.Read())
                                    {
                                        //Добавляем проект
                                        if (db.tProject.Find(fr[0]) != null)
                                        {
                                            var m = db.tProject.Find(fr[0]);
                                            m.tProject_name = fr[1].ToString();
                                            db.Entry(m).State = System.Data.Entity.EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                        else
                                        {
                                            var m = new tProject { Id = (int)fr[0], tProject_name = fr[1].ToString() };
                                            db.tProject.Add(m);
                                            db.SaveChanges();
                                        }
                                        //теперь добавим подсистему
                                        using (FbCommand fcon_sub = new FbCommand("select s.ID_SUBSYSTEM, s.MNEMO  from SUBSYSTEMS s where s.ID_PROJECTS = @id order by 1", fb, ft))
                                        {
                                            fcon_sub.Parameters.AddWithValue("@id", (int)fr[0]);
                                            using(FbDataReader fr_sub = fcon_sub.ExecuteReader())
                                            {
                                                while(fr_sub.Read())
                                                {
                                                    if(db.tSubsystem.Find(fr_sub[0]) != null)
                                                    {
                                                        var sub = db.tSubsystem.Find(fr_sub[0]);
                                                        sub.tSubsystem_name = fr_sub[1].ToString();
                                                        db.Entry(sub).State = System.Data.Entity.EntityState.Modified;
                                                    }
                                                    else
                                                    {
                                                        var sub = new tSubsystem{Id = (int)fr_sub[0], tSubsystem_name = fr_sub[1].ToString(), tproject = db.tProject.Find(fr[0])};
                                                        db.tSubsystem.Add(sub);
                                                        db.SaveChanges();
                                                    }
                                                }
                                                fr_sub.Dispose();
                                            }
                                            fcon_sub.Dispose();
                                        }
                                        //Релизы проектов
                                        using (FbCommand fcon_sub = new FbCommand("select pr.ID_RELEASE_PROJECTS, pr.MNEMO from RELEASE_PROJECTS pr where pr.ID_PROJECTS = @id order by pr.MNEMO", fb, ft))
                                        {
                                            fcon_sub.Parameters.AddWithValue("@id", (int)fr[0]);
                                            using (FbDataReader fr_sub = fcon_sub.ExecuteReader())
                                            {
                                                while (fr_sub.Read())
                                                {
                                                    if (db.tReleaseProject.Find(fr_sub[0]) != null)
                                                    {
                                                        var sub = db.tReleaseProject.Find(fr_sub[0]);
                                                        sub.tReleaseProject_name = fr_sub[1].ToString();
                                                        db.Entry(sub).State = System.Data.Entity.EntityState.Modified;
                                                    }
                                                    else
                                                    {
                                                        var sub = new tReleaseProject { Id = (int)fr_sub[0], tReleaseProject_name = fr_sub[1].ToString(), tproject = db.tProject.Find(fr[0]) };
                                                        db.tReleaseProject.Add(sub);
                                                        db.SaveChanges();
                                                    }
                                                }
                                                fr_sub.Dispose();
                                            }
                                            fcon_sub.Dispose();
                                        }
                                    }
                                    fr.Dispose();
                                }
                                fcon.Dispose();
                            }
                            ft.Commit();
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
        }
        #endregion
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}