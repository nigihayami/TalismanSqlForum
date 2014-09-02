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
               new tForumList { tForumList_name = "���", tForumList_description = "���������� ������ \"���\" �� ��������-SQL", tForumList_hide = false, tForumList_icon = "icon-libreoffice" },
               new tForumList { tForumList_name = "�����������������", tForumList_description = "���������� ������ \"�����������������\" �� ��������-SQL", tForumList_hide = false, tForumList_icon = "icon-tools" },
               new tForumList { tForumList_name = "����������", tForumList_description = "���������� ������ \"����������\" �� ��������-SQL", tForumList_hide = false, tForumList_icon = "icon-copy" },
               new tForumList { tForumList_name = "�����������", tForumList_description = "���������� ������ \"�����������\" �� ��������-SQL", tForumList_hide = false, tForumList_icon = "icon-calculate" },
               new tForumList { tForumList_name = "��������", tForumList_description = "���������� ������ \"��������\" �� ��������-SQL", tForumList_hide = false, tForumList_icon = "icon-dollar-2" },
               new tForumList { tForumList_name = "�����", tForumList_description = "���������� ������ \"�����\" �� ��������-SQL", tForumList_hide = false, tForumList_icon = "icon-user-3" },
               new tForumList { tForumList_name = "����� �������", tForumList_description = "���� ������� � ���������", tForumList_hide = false, tForumList_icon = "icon-support" },
               new tForumList { tForumList_name = "������� ����� ���������", tForumList_description = "���������� ������ \"������� ����� ���������\" �� ��������-SQL", tForumList_hide = false, tForumList_icon = "icon-wrench" }
               );
            if (context.tRules.Count() == 0)
            {
                context.tRules.Add(new Models.Admin.tRules
                {
                    tRules_rules = 
                        "�� ������ ��������� ������������ �������, �������, ������ ����������� � ���������� ��. " +
                        "<hr/>"+
                        "<ol> " +
                        "<li>������������� ������ ��������� �� ����� ����� ������������ ������� �������� � (���) ��������� � �������. ����� ������� � ���������� �������� � ���� � ������� �������������.</li> " +
                        "<li>������������� ������ ����� ��������� ������� ������������ � ���������.</li> " +
                        "<li>��� ��������� �� ������������� �������� ������� �� �������, �� ������� ������������� ������ ��������������� �� ����.</li> " +
                        "<li>��������� ������������� ��� ��������������� ���������� ����� ���������.</li> " +
                        "<li>����������� ���������� ������������ ������ ������ �������� (�����, ������) ������ ������������� ������.</li> " +
                        "<li>���� ��� ���������� �������� �������������� ������, � �� ���������� ������� �� ������. ������� ���� ����� ���������������, ������������, ����������������, ��������� �������������� ������ �� ���� ����������.</li> " +
                        "<li>������ ��� ������� ����� ����, ���������, ��� ����� ������ ��� �� ��� �����. � ��������� ������ ���� ����� ���� �������, � ������ ������������� � ��� ����������� ����.</li> " +
                        "<li>����������� ��������� ����� ���� � ������������������ ���������, ��������:<ul><li>���������!!!�;<li>����� ���� ��������;</ul> " +
                        "    � �.�. ���� ����� ���� ����� ������ ���������� ��� ����� ���� �������������. ���� ���� �����������������, ��� ����� �������.</li> " +
                        "<li>����������� ��������� ���� � �������, �� ���������� �� ��������. ��������, �������� ������� �� ������ � ������ �� ���������� �����. ����� ���� ����� ������������ � �������������� ������ ������.</li> " +
                        "<li>����������� ��������� ���������� ���� ����� � ���������� ������� ������������.</li> " +
                        "<li>������� �� ������ ���������� ������� � �� �������������� ����������������, ��������������� � ������� �������.</li> " +
                        "<li>����������� ��������� ���������, ���������� �p����� � ��py����� ������y����� ����������������.</li> " +
                        "<li>����������� ��������� ���������, ���������� ��������� ����������.</li> " +
                        "<li> ������������� ������ �� ����� ��������������� �� ��������� ����� �� ���������, ����������� �� ������ ��������������.</li></ul>"
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
                    Name_Org = "����������� ����������",
                    Mnemo_Org = "����������� ����������",
                    Inn = "0000000000",
                    Adres = "���������, ������� 166/1",
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
