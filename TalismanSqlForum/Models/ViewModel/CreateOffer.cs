using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using TalismanSqlForum.Models;
using TalismanSqlForum.Models.Forum;

namespace TalismanSqlForum.Models.ViewModel
{
    public class CreateOffer
    {
        public tForumMessages _message { get; set; }

        [Display(Name = "Ошибка?")]
        public bool _iserror { get; set; }
        [Required]
        [Display(Name = "Место обнаружения")]
        public string _location { get; set; }

        public string _comment { get; set; }

        [Display(Name = "Организация")]
        public int id_branch { get; set; }
        [Display(Name = "Проект")]
        public int id_projects { get; set; }
        [Display(Name = "Версия проекта")]
        public int id_release_projects { get; set; }
        [Display(Name = "Версия реализации")]
        public int id_release_projects_exec { get; set; }
        [Display(Name = "Подсистема")]
        public int id_subsystem { get; set; }

    }
    public class Branch
    {
        [Key]
        public int id {get;set;}
        public string name {get;set;}
    }

    public class val
    {
        public int id { get; set; }
        public string name { get; set; }
    }
    public class rep
    {
        public static List<val> get(string _table, int id, string UserId)
        {
            List<val> b = new List<val> { };
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                FbConnectionStringBuilder fc = new FbConnectionStringBuilder();
                var t = db.tModerator.Where(a => a.tUsers.UserName == UserId);
                if (t.Count() != 0)
                {
                    fc.Database = t.First().tModerator_database;
                    fc.UserID = t.First().tModerator_userId;
                    fc.Password = t.First().tModerator_password;
                    fc.Pooling = false;
                    fc.Role = "R_ADMIN";
                    fc.Charset = "win1251";

                    using (FbConnection fb = new FbConnection(fc.ConnectionString))
                    {
                        try
                        {
                            fb.Open();
                            using (FbTransaction ft = fb.BeginTransaction())
                            {
                                string com = "";
                                switch (_table)
                                {
                                    case "BRANCH":
                                        com = "select id_branch, mnemo from branch order by 2";
                                        break;
                                    case "RELEASE_PROJECTS":
                                        com = "select ID_RELEASE_PROJECTS, mnemo from RELEASE_PROJECTS ";
                                        break;
                                    case "PROJECTS":
                                        com = "select ID_PROJECTS, mnemo from PROJECTS order by 2";
                                        break;
                                    case "SUBSYSTEMS":
                                        com = "select ID_SUBSYSTEM, mnemo from SUBSYSTEMS ";
                                        break;
                                }
                                using (FbCommand fcon = new FbCommand(com, fb, ft))
                                {
                                    switch (_table)
                                    {
                                        case "RELEASE_PROJECTS":
                                            fcon.CommandText += " where id_projects = " + id.ToString() + " order by 2 desc";
                                            break;
                                        case "SUBSYSTEMS":
                                            fcon.CommandText += " where id_projects = " + id.ToString() + " order by 2";
                                            break;
                                    }
                                    using (FbDataReader fr = fcon.ExecuteReader())
                                    {
                                        while (fr.Read())
                                        {
                                            b.Add(new val { id = (int)fr[0], name = fr[1].ToString() });
                                        }
                                        fr.Dispose();
                                    }
                                    fcon.Dispose();
                                }
                                ft.Dispose();
                            }
                        }
                        catch
                        {
                            b.Add(new val { id = -1, name = "Не удалось подключиться к БТ" });
                        }
                        finally
                        {
                            fb.Close();
                        }
                        fb.Dispose();
                    }
                    db.Dispose();
                }
                else
                {
                    b.Add(new val { id = -1, name = "Настройте соединение с БТ" });
                }
                return b;
            }
        }
    }
}