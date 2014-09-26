using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace TalismanSqlForum.Models.ViewModel
{
    public class tForumThemes_Edit
    {
        [AllowHtml]
        [MaxLength]
        [Display(Name = "Краткое описание")]
        public string tForumThemes_desc { get; set; }
        [Required]
        [Display(Name = "Название темы")]
        [MaxLength(100)]
        public string tForumThemes_name { get; set; }
    }
}