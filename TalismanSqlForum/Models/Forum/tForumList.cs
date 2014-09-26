using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
        [Display(Name = "Скрытый")]
        public bool tForumList_hide { get; set; }

        [Display(Name = "Иконка форума ")]
        public string tForumList_icon { get; set; }
        public virtual ICollection<tForumThemes> tForumThemes { get; set; }
    }
}