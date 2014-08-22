using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using TalismanSqlForum.Models.Forum;
using TalismanSqlForum.Models.Moderator;
using TalismanSqlForum.Models.Users;

namespace TalismanSqlForum.Models
{
    // Чтобы добавить данные профиля для пользователя, можно добавить дополнительные свойства в класс ApplicationUser. Дополнительные сведения см. по адресу: http://go.microsoft.com/fwlink/?LinkID=317594.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Обратите внимание, что authenticationType должен совпадать с типом, определенным в CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Здесь добавьте утверждения пользователя
            return userIdentity;
        }
        public ApplicationUser()
        {
            this.tForumThemes = new HashSet<tForumThemes>();
            this.tForumMessages = new HashSet<tForumMessages>();
            this.tModerator = new HashSet<tModerator>();
            this.tUserNewThemes = new HashSet<tUserNewThemes>();
        }
        [Required]
        [Display(Name = "Полное наименование учреждения")]
        public string Name_Org { get; set; }
        [Required]
        [Display(Name = "Краткое наименование учреждения")]
        public string Mnemo_Org { get; set; }
        [Required]
        [Display(Name = "ИНН")]
        [StringLength(10, ErrorMessage = "Значение {0} должно содержать не менее {2} символов.", MinimumLength = 10)]
        public string Inn { get; set; }
        [Required]
        [Display(Name = "Адрес")]
        public string Adres { get; set; }
        [Display(Name = "Контактное лицо")]
        public string Contact_Name { get; set; }

        [Required]
        [Display(Name = "Использовать ник")]
        public string NickName { get; set; }
        [Display(Name = "Дата последнего входа")]
        public System.DateTime LastIn { get; set; }

        [Display(Name = "Дата регистрации")]
        public System.DateTime DateReg { get; set; }

        public virtual ICollection<tForumThemes> tForumThemes { get; set; }
        public virtual ICollection<tForumMessages> tForumMessages { get; set; }
        public virtual ICollection<tModerator> tModerator { get; set; }
        public virtual ICollection<tUserNewThemes> tUserNewThemes { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }
        protected override void OnModelCreating(System.Data.Entity.DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<tForumList>()
                .HasMany(a => a.tForumThemes)
                .WithRequired(b => b.tForumList)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<tForumThemes>()
                .HasMany(a => a.tForumMessages)
                .WithRequired(b => b.tForumThemes)
                .WillCascadeOnDelete(true);
            modelBuilder.Entity<tForumThemes>()
                .HasMany(a => a.tUserNewForumThemes)
                .WithRequired(b => b.tForumThemes)
                .WillCascadeOnDelete(true);

            base.OnModelCreating(modelBuilder);
        }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        public System.Data.Entity.DbSet<TalismanSqlForum.Models.Forum.tForumList> tForumLists { get; set; }

        public System.Data.Entity.DbSet<TalismanSqlForum.Models.Forum.tForumThemes> tForumThemes { get; set; }

        public System.Data.Entity.DbSet<TalismanSqlForum.Models.Forum.tForumMessages> tForumMessages { get; set; }

        public System.Data.Entity.DbSet<TalismanSqlForum.Models.Moderator.tModerator> tModerator { get; set; }

        public System.Data.Entity.DbSet<IdentityUserRole> IdentityUserRole { get; set; }

        public System.Data.Entity.DbSet<tUserNewThemes> tUserNewThemes { get; set; }
    }
}