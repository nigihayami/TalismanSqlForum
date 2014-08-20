using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TalismanSqlForum.Models.Forum
{
    public class tForumMessages
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Сообщение")]
        [AllowHtml]
        public string tForumMessages_messages { get; set; }
        [Required]
        public System.DateTime tForumMessages_datetime { get; set; }

        [AllowHtml]
        public string tForumMessages_offer { get; set; }


        public virtual tForumThemes tForumThemes { get; set; }
        public virtual ApplicationUser tUsers { get; set; }
    }
}