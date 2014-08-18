﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TalismanSqlForum.Models.Forum
{
    public class tForumList
    {
        public tForumList()
        {
            this.tForumThemes = new HashSet<tForumThemes>();
        }
        public int Id { get; set; }
        [Required]
        [Display(Name = "Название форума")]
        public string tForumList_name { get; set; }
        [Display(Name = "Краткое описание")]
        public string tForumList_description { get; set; }

        public virtual ICollection<tForumThemes> tForumThemes { get; set; }
    }
}