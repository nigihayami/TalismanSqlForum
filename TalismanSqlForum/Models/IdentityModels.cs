using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using TalismanSqlForum.Models.Forum;
using TalismanSqlForum.Models.Moderator;
using TalismanSqlForum.Models.Users;
using TalismanSqlForum.Models.Admin;
using TalismanSqlForum.Models.Notification;
using TalismanSqlForum.Models.Offer;
using TalismanSqlForum.Models.Stat;

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
            this.tNotification = new HashSet<tNotification>();
            this.tusernewmessages = new HashSet<tUserNewMessages>();
        }
        [Required]
        [Display(Name = "Полное наименование учреждения")]
        public string Name_Org { get; set; }
        [Required]
        [Display(Name = "Краткое наименование учреждения")]
        public string Mnemo_Org { get; set; }
        [Required]
        [Display(Name = "ИНН")]
        [StringLength(12, ErrorMessage = "Значение {0} должно содержать не менее {2} символов.", MinimumLength = 10)]
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

        [Display(Name = "Новый пользователь")]
        public bool IsNew { get; set; }

        public virtual ICollection<tForumThemes> tForumThemes { get; set; }
        public virtual ICollection<tForumMessages> tForumMessages { get; set; }
        public virtual ICollection<tModerator> tModerator { get; set; }
        public virtual ICollection<tUserNewThemes> tUserNewThemes { get; set; }
        public virtual ICollection<tNotification> tNotification { get; set; }

        public virtual ICollection<tUserNewMessages> tusernewmessages { get; set; }
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
            #region tOffer
            modelBuilder.Entity<tOffer>()
                .HasRequired(a => a.tproject)
                .WithMany(b => b.toffer)
                .HasForeignKey(a => a.tOffer_tProject_id)
                .WillCascadeOnDelete(false);
            modelBuilder.Entity<tOffer>()
                .HasRequired(a => a.tsubsystem)
                .WithMany(b => b.toffer)
                .HasForeignKey(a => a.tOffer_tSubsystem_id)
                .WillCascadeOnDelete(false);
            modelBuilder.Entity<tOffer>()
                .HasRequired(a => a.treleaseproject)
                .WithMany(b => b.toffer)
                .HasForeignKey(a=> a.tOffer_tReleaseProject_id)
                .WillCascadeOnDelete(false);
            modelBuilder.Entity<tOffer>()
                .HasRequired(a => a.treleaseproject_exec)
                .WithMany(b => b.toffer_exec)
                .HasForeignKey(a=> a.tOffer_tReleaseProject_exec_id)
                .WillCascadeOnDelete(false);
            modelBuilder.Entity<tOffer>()
                .HasRequired(a => a.tbranch)
                .WithMany(b => b.toffer)
                .HasForeignKey(a => a.tOffer_tBranch_id)
                .WillCascadeOnDelete(false);
            #endregion
            base.OnModelCreating(modelBuilder);
        }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        public System.Data.Entity.DbSet<tForumList> tForumLists { get; set; }

        public System.Data.Entity.DbSet<tForumThemes> tForumThemes { get; set; }

        public System.Data.Entity.DbSet<tForumMessages> tForumMessages { get; set; }

        public System.Data.Entity.DbSet<tModerator> tModerator { get; set; }

        public System.Data.Entity.DbSet<tUserNewThemes> tUserNewThemes { get; set; }

        public System.Data.Entity.DbSet<tRules> tRules { get; set; }

        public System.Data.Entity.DbSet<tNotificationType> tNotificationType { get; set; }
        public System.Data.Entity.DbSet<tNotification> tNotification { get; set; }

        public System.Data.Entity.DbSet<tOffer> tOffer { get; set; }
        public System.Data.Entity.DbSet<tBranch> tBranch { get; set; }
        public System.Data.Entity.DbSet<tProject> tProject { get; set; }
        public System.Data.Entity.DbSet<tReleaseProject> tReleaseProject { get; set; }
        public System.Data.Entity.DbSet<tSubsystem> tSubsystem { get; set; }

        public System.Data.Entity.DbSet<tUserNewMessages> tUserNewMessages { get; set; }
        public System.Data.Entity.DbSet<StatForum> StatForum { get; set; }
        public System.Data.Entity.DbSet<StatForumList> StatForumList { get; set; }
    }
}