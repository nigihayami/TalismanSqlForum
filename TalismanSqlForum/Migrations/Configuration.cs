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
               new tForumList { tForumList_name = "ФЭО", tForumList_description = "Обсуждение модуля \"ФЭО\" ПК Талисман-SQL", tForumList_hide = false, tForumList_icon = "icon-libreoffice" },
               new tForumList { tForumList_name = "Администрирование", tForumList_description = "Обсуждение модуля \"Администрирование\" ПК Талисман-SQL", tForumList_hide = false, tForumList_icon = "icon-tools" },
               new tForumList { tForumList_name = "Репликация", tForumList_description = "Обсуждение модуля \"Репликация\" ПК Талисман-SQL", tForumList_hide = false, tForumList_icon = "icon-copy" },
               new tForumList { tForumList_name = "Бухгалтерия", tForumList_description = "Обсуждение модуля \"Бухгалтерия\" ПК Талисман-SQL", tForumList_hide = false, tForumList_icon = "icon-calculate" },
               new tForumList { tForumList_name = "Зарплата", tForumList_description = "Обсуждение модуля \"Зарплата\" ПК Талисман-SQL", tForumList_hide = false, tForumList_icon = "icon-dollar-2" },
               new tForumList { tForumList_name = "Кадры", tForumList_description = "Обсуждение модуля \"Кадры\" ПК Талисман-SQL", tForumList_hide = false, tForumList_icon = "icon-user-3" },
               new tForumList { tForumList_name = "Общие вопросы", tForumList_description = "Ваши вопросы и пожелания", tForumList_hide = false, tForumList_icon = "icon-support" },
               new tForumList { tForumList_name = "Система учета замечаний", tForumList_description = "Обсуждение модуля \"Система учета замечаний\" ПК Талисман-SQL", tForumList_hide = false, tForumList_icon = "icon-wrench" }
               );
            if (context.tRules.Count() == 0)
            {
                context.tRules.Add(new Models.Admin.tRules
                {
                    tRules_rules = 
                        "На форуме действуют определенные правила, поэтому, будьте внимательны и соблюдайте их. " +
                        "<hr/>"+
                        "<ol> " +
                        "<li>Администрация форума оставляет за собой право периодически вносить поправки и (или) изменения в правила. Новые решения и требования вступают в силу с момента опубликования.</li> " +
                        "<li>Администрация форума может принимать решения расходящиеся с Правилами.</li> " +
                        "<li>Все сообщения от пользователей являются мнением их авторов, за которое администрация форума ответственности не несёт.</li> " +
                        "<li>Сообщения непристойного или оскорбительного содержания будут удаляться.</li> " +
                        "<li>Запрещается передавать персональные данные своего аккаунта (логин, пароль) другим пользователям форума.</li> " +
                        "<li>Тема для обсуждения является собственностью форума, а не участником который ее создал. Поэтому темы могут переименоваться, переноситься, корректироваться, удаляться администрацией форума на свое усмотрение.</li> " +
                        "<li>Прежде чем создать новую тему, убедитесь, что такой вопрос еще не был задан. В противном случае тема может быть удалена, а вопрос перенаправлен в уже обсуждаемую тему.</li> " +
                        "<li>Запрещается создавать новые темы с малосодержательным названием, например:<ul><li>«Помогите!!!»;<li>«Есть пара вопросов»;</ul> " +
                        "    и т.п. Если такая тема имеет важное содрежание она может быть переименована. Если тема малосодержательна, она будет удалена.</li> " +
                        "<li>Запрещается создавать темы в форумах, не подходящих по тематике. Например, задавать вопросы по кадрам в форуме по заработной плате. Такие темы будут переноситься в соответсвующий раздел форума.</li> " +
                        "<li>Запрещается создавать одинаковые темы сразу в нескольких форумах одновременно.</li> " +
                        "<li>Просьба НЕ ПИСАТЬ ЗАГЛАВНЫМИ БУКВАМИ и не злоупотреблять восклицательными, вопросительными и другими знаками.</li> " +
                        "<li>Запрещается помещение сообщений, содержащих пpизывы к наpyшению действyющего законодательства.</li> " +
                        "<li>Запрещается помещение сообщений, содержащих рекламное содержание.</li> " +
                        "<li> Администрация форума не несет ответственности за авторские права на материалы, размещенные на форуме пользователями.</li></ul>"
                });
            }
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
                    DateReg = System.DateTime.Now,
                    IsNew = false
                };

                manager.Create(user, "qvantologiya!");
                manager.AddToRole(user.Id, "admin");
            }
        }
    }
}
