using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TalismanSqlForum.Models.ViewModel
{
    public class tForumThemes_Edit
    {
        [AllowHtml]
        [MaxLength]
        public string tForumThemes_desc { get; set; }
    }
}