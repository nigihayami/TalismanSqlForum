using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TalismanSqlForum.Models.Admin
{
    public class tRules
    {
        public int Id { get; set; }
        [AllowHtml]
        [Display(Name = "Краткое описание")]
        [MaxLength]
        public string tRules_rules{ get; set; }
    }
}