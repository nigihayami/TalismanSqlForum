﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using TalismanSqlForum.Models.Offer;
using TalismanSqlForum.Models.Users;

namespace TalismanSqlForum.Models.Forum
{
    public class tForumThemes
    {
        public tForumThemes()
        {
            this.tForumMessages = new HashSet<tForumMessages>();
            this.toffer = new HashSet<tOffer>();
        }
        public int Id { get; set; }
        [Required]
        [Display(Name = "Название темы")]
        [MaxLength(100)]
        public string tForumThemes_name { get; set; }

        public System.DateTime tForumThemes_datetime { get; set; }

        [AllowHtml]
        [Display(Name="Краткое описание")]
        [MaxLength]
        public string tForumThemes_desc { get; set; }

        [Display(Name = "Закрепленная тема")]
        public bool tForumThemes_top { get; set; }
        [Display(Name = "Закрытая тема")]
        public bool tForumThemes_close { get; set; }

        public bool tForumThemes_hide { get; set; }

        public virtual tForumList tForumList { get; set; }
        public virtual ApplicationUser tUsers { get; set; }
        public virtual ICollection<tForumMessages> tForumMessages { get; set; }

        public virtual ICollection<tUserNewThemes> tUserNewForumThemes { get; set; }
        public virtual ICollection<tOffer> toffer { get; set; }
    }
}