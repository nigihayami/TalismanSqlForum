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
    [Authorize(Roles = "admin")]
    public class AdminBtController : Controller
    {
        public ActionResult Sync()
        {
            //синхронизация спарвочника БТ
            if (!TryConnect(User.Identity.Name))
                return RedirectToAction("Settings", "Moder", new {returnUrl = Url.Action("Sync", "AdminBt")});
            //начинаем формирование справочников
            var username = User.Identity.Name;
            SyncBranch(username);
            SyncProject(username);

            return RedirectToAction("Bt", "Admin", new { mes = "Синхронизация завершена" });
        }


        static bool TryConnect(string username)
        {
            bool res = false;
            using (var db = new ApplicationDbContext())
            {
                foreach (var item in db.tModerator.Where(a => a.tUsers.UserName == username))
                {
                    try
                    {
                        var fc = new FbConnectionStringBuilder
                        {
                            UserID = item.tModerator_userId,
                            Password = item.tModerator_password,
                            Database = item.tModerator_database,
                            Pooling = false,
                            Charset = "win1251"
                        };
                        using (var fb = new FbConnection(fc.ConnectionString))
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
            using (var db = new ApplicationDbContext())
            {
                var t = db.tModerator.First(a => a.tUsers.UserName == username);
                if (t != null)
                {
                    var fc = new FbConnectionStringBuilder
                    {
                        UserID = t.tModerator_userId,
                        Password = t.tModerator_password,
                        Database = t.tModerator_database,
                        Charset = "win1251",
                        Pooling = false,
                        Role = "R_ADMIN"
                    };

                    using (var fb = new FbConnection(fc.ConnectionString))
                    {
                        try
                        {
                            fb.Open();
                            using (var ft = fb.BeginTransaction())
                            {
                                using (var fcon = new FbCommand("select b.id_branch, b.mnemo from branch b", fb, ft))
                                {
                                    using (var fr = fcon.ExecuteReader())
                                    {
                                        while (fr.Read())
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
                        catch
                        {
                            //Пропускаем все ошибки - сихн е удалась
                        }
                        finally
                        {
                            fb.Close();
                        }
                        fb.Dispose();
                    }
                }
                db.Dispose();
            }
        }
        #endregion
        #region Project
        static void SyncProject(string username)
        {
            //вывожу ее отдельно, так как здесь немного другая логика работы
            using (var db = new ApplicationDbContext())
            {
                var t = db.tModerator.First(a => a.tUsers.UserName == username);
                if (t != null)
                {
                    var fc = new FbConnectionStringBuilder
                    {
                        UserID = t.tModerator_userId,
                        Password = t.tModerator_password,
                        Database = t.tModerator_database,
                        Charset = "win1251",
                        Pooling = false,
                        Role = "R_ADMIN"
                    };

                    using (var fb = new FbConnection(fc.ConnectionString))
                    {
                        try
                        {
                            fb.Open();
                            using (var ft = fb.BeginTransaction())
                            {
                                using (
                                    var fcon = new FbCommand(
                                        "select p.ID_PROJECTS, p.MNEMO from PROJECTS p order by 1", fb, ft))
                                {
                                    using (var fr = fcon.ExecuteReader())
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
                                                var m = new tProject
                                                {
                                                    Id = (int)fr[0],
                                                    tProject_name = fr[1].ToString()
                                                };
                                                db.tProject.Add(m);
                                                db.SaveChanges();
                                            }
                                            //теперь добавим подсистему
                                            using (
                                                var fconSub =
                                                    new FbCommand(
                                                        "select s.ID_SUBSYSTEM, s.MNEMO  from SUBSYSTEMS s where s.ID_PROJECTS = @id order by 1",
                                                        fb, ft))
                                            {
                                                fconSub.Parameters.AddWithValue("@id", (int)fr[0]);
                                                using (var frSub = fconSub.ExecuteReader())
                                                {
                                                    while (frSub.Read())
                                                    {
                                                        if (db.tSubsystem.Find(frSub[0]) != null)
                                                        {
                                                            var sub = db.tSubsystem.Find(frSub[0]);
                                                            sub.tSubsystem_name = frSub[1].ToString();
                                                            db.Entry(sub).State =
                                                                System.Data.Entity.EntityState.Modified;
                                                        }
                                                        else
                                                        {
                                                            var sub = new tSubsystem
                                                            {
                                                                Id = (int)frSub[0],
                                                                tSubsystem_name = frSub[1].ToString(),
                                                                tproject = db.tProject.Find(fr[0])
                                                            };
                                                            db.tSubsystem.Add(sub);
                                                            db.SaveChanges();
                                                        }
                                                    }
                                                    frSub.Dispose();
                                                }
                                                fconSub.Dispose();
                                            }
                                            //Релизы проектов
                                            using (
                                                var fconSub =
                                                    new FbCommand(
                                                        "select pr.ID_RELEASE_PROJECTS, pr.MNEMO from RELEASE_PROJECTS pr where pr.ID_PROJECTS = @id order by pr.MNEMO",
                                                        fb, ft))
                                            {
                                                fconSub.Parameters.AddWithValue("@id", (int)fr[0]);
                                                using (var frSub = fconSub.ExecuteReader())
                                                {
                                                    while (frSub.Read())
                                                    {
                                                        if (db.tReleaseProject.Find(frSub[0]) != null)
                                                        {
                                                            var sub = db.tReleaseProject.Find(frSub[0]);
                                                            sub.tReleaseProject_name = frSub[1].ToString();
                                                            db.Entry(sub).State =
                                                                System.Data.Entity.EntityState.Modified;
                                                        }
                                                        else
                                                        {
                                                            var sub = new tReleaseProject
                                                            {
                                                                Id = (int)frSub[0],
                                                                tReleaseProject_name = frSub[1].ToString(),
                                                                tproject = db.tProject.Find(fr[0])
                                                            };
                                                            db.tReleaseProject.Add(sub);
                                                            db.SaveChanges();
                                                        }
                                                    }
                                                    frSub.Dispose();
                                                }
                                                fconSub.Dispose();
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
                        catch{}
                        finally
                        {
                            fb.Close();
                        }
                        fb.Dispose();
                    }
                }
                db.Dispose();
            }
        }
        #endregion
    }
}