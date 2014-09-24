using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using TalismanSqlForum.Models.Forum;

namespace TalismanSqlForum.Models.Offer
{
    //Замеание
    public class tOffer
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Ошибка?")]
        public bool tOffer_error { get; set; }
        [Required]
        [Display(Name = "Место обнаружения")]
        public string tOffer_place { get; set; }

        [Display(Name = "Дополнения к постановке")]
        [MaxLength]
        public string tOffer_desc { get; set; }

        [Required]
        [Display(Name="Проект")]
        public int tOffer_tProject_id { get; set; }
        [Required]
        [Display(Name = "Релиз")]
        public int tOffer_tReleaseProject_id { get; set; }
        [Required]
        [Display(Name = "Релиз исполнения")]
        public int tOffer_tReleaseProject_exec_id { get; set; }
        [Required]
        [Display(Name = "Подсистема")]
        public int tOffer_tSubsystem_id { get; set; }
        [Required]
        [Display(Name = "Заказчик")]
        public int tOffer_tBranch_id { get; set; }

        public int tOffer_docnumber { get; set; }

        public virtual tBranch tbranch { get; set; }
        public virtual tProject tproject { get; set; }
        public virtual tReleaseProject treleaseproject { get; set; }
        public virtual tReleaseProject treleaseproject_exec { get; set; }
        public virtual tSubsystem tsubsystem { get; set; }
        //привязка идет к сообщению или теме
        public virtual tForumThemes tforumthemes { get; set; }
        public virtual tForumMessages tforummessages { get; set; }
    }
    //Орг
    public class tBranch
    {
        public tBranch()
        {
            this.toffer = new HashSet<tOffer>();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Организация")]
        public string tBranch_name { get; set; }

        public virtual ICollection<tOffer> toffer { get; set; }
    }
    //Проект
    public class tProject
    {
        public tProject()
        {
            this.toffer = new HashSet<tOffer>();
            this.treleaseproject = new HashSet<tReleaseProject>();
            this.tsubsystem = new HashSet<tSubsystem>();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Проект")]
        public string tProject_name { get; set; }

        public virtual ICollection<tOffer> toffer { get; set; }
        public virtual ICollection<tReleaseProject> treleaseproject { get; set; }
        public virtual ICollection<tSubsystem> tsubsystem { get; set; }
    }
    //Реализации проектов
    public class tReleaseProject
    {
        public tReleaseProject()
        {
            this.toffer = new HashSet<tOffer>();
            this.toffer_exec = new HashSet<tOffer>();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Версия проекта")]
        public string tReleaseProject_name { get; set; }

        public virtual tProject tproject { get; set; }
        public virtual ICollection<tOffer> toffer { get; set; }
        public virtual ICollection<tOffer> toffer_exec { get; set; }
    }
    //Подсистема
    public class tSubsystem
    {
        public tSubsystem()
        {
            this.toffer = new HashSet<tOffer>();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Подсистема")]
        public string tSubsystem_name { get; set; }

        public virtual tProject tproject { get; set; }
        public virtual ICollection<tOffer> toffer { get; set; }
    }
    //вспомогательный список
    public class val
    {
        public int id { get; set; }
        public string name { get; set; }
    }
}