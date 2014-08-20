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
               new tForumList { tForumList_name = "���", tForumList_description = "���������� ������ \"���\" �� ��������-SQL" },
               new tForumList { tForumList_name = "�����������������", tForumList_description = "���������� ������ \"�����������������\" �� ��������-SQL" },
               new tForumList { tForumList_name = "����������", tForumList_description = "���������� ������ \"����������\" �� ��������-SQL" },
               new tForumList { tForumList_name = "�����������", tForumList_description = "���������� ������ \"�����������\" �� ��������-SQL" },
               new tForumList { tForumList_name = "��������", tForumList_description = "���������� ������ \"��������\" �� ��������-SQL" },
               new tForumList { tForumList_name = "�����", tForumList_description = "���������� ������ \"�����\" �� ��������-SQL" },
               new tForumList { tForumList_name = "����� �������", tForumList_description = "���� ������� � ���������" },
               new tForumList { tForumList_name = "������� ����� ���������", tForumList_description = "���������� ������ \"������� ����� ���������\" �� ��������-SQL" }
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
                    Name_Org = "����������� ����������",
                    Mnemo_Org = "����������� ����������",
                    Inn = "0000000000",
                    Adres = "���������, ������� 166/1",
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
