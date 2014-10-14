using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TalismanSqlForum.Models.Stat;

namespace TalismanSqlForum.Models.Forum
{
    public class tForumList
    {
        public tForumList()
        {
            this.tForumThemes = new HashSet<tForumThemes>();
            this.StatForumLists = new HashSet<StatForumList>();
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
        public virtual ICollection<StatForumList> StatForumLists { get; set; }
    }
}