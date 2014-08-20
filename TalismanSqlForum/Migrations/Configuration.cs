namespace TalismanSqlForum.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using TalismanSqlForum.Models;
    using TalismanSqlForum.Models.Forum;

    internal sealed class Configuration : DbMigrationsConfiguration<TalismanSqlForum.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(TalismanSqlForum.Models.ApplicationDbContext context)
        {
            context.tForumLists.AddOrUpdate(a => a.tForumList_name,
               new tForumList { tForumList_name = "ФЭО", tForumList_description = "Обсуждение модуля \"ФЭО\" ПК Талисман-SQL" },
               new tForumList { tForumList_name = "Администрирование", tForumList_description = "Обсуждение модуля \"Администрирование\" ПК Талисман-SQL" },
               new tForumList { tForumList_name = "Репликация", tForumList_description = "Обсуждение модуля \"Репликация\" ПК Талисман-SQL" },
               new tForumList { tForumList_name = "Бухгалтерия", tForumList_description = "Обсуждение модуля \"Бухгалтерия\" ПК Талисман-SQL" },
               new tForumList { tForumList_name = "Зарплата", tForumList_description = "Обсуждение модуля \"Зарплата\" ПК Талисман-SQL" },
               new tForumList { tForumList_name = "Кадры", tForumList_description = "Обсуждение модуля \"Кадры\" ПК Талисман-SQL" },
               new tForumList { tForumList_name = "Общие вопросы", tForumList_description = "Ваши вопросы и пожелания" },
               new tForumList { tForumList_name = "Система учета замечаний", tForumList_description = "Обсуждение модуля \"Система учета замечаний\" ПК Талисман-SQL" }
               );
            if (!context.Roles.Any(r => r.Name == "admin"))
            {
                var store = new RoleStore<IdentityRole>(context);
                var manager = new RoleManager<IdentityRole>(store);
                var role = new IdentityRole { Name = "admin" };

                manager.Create(role);
            }
            if (!context.Roles.Any(r => r.Name == "user"))
            {
                var store = new RoleStore<IdentityRole>(context);
                var manager = new RoleManager<IdentityRole>(store);
                var role = new IdentityRole { Name = "user" };

                manager.Create(role);
            }
            if (!context.Roles.Any(r => r.Name == "moderator"))
            {
                var store = new RoleStore<IdentityRole>(context);
                var manager = new RoleManager<IdentityRole>(store);
                var role = new IdentityRole { Name = "moderator" };

                manager.Create(role);
            }

            if (!context.Users.Any(u => u.UserName == "admin@talismansql.ru"))
            {
                var store = new UserStore<ApplicationUser>(context);
                var manager = new UserManager<ApplicationUser>(store);
                var user = new ApplicationUser {
                    Email  = "admin@talismansql.ru",
                    UserName = "admin@talismansql.ru",
                    Name_Org = "Программные разработки",
                    Mnemo_Org = "Программные разработки",
                    Inn = "0000000000",
                    Adres = "Краснодар, Садовая 166/1",
                    NickName = "admin",
                    LastIn = System.DateTime.Now,
                    DateReg = System.DateTime.Now
                };

                manager.Create(user, "qvantologiya!");
                manager.AddToRole(user.Id, "admin");
            }
        }
    }
}
