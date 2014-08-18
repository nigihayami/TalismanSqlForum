using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TalismanSqlForum.Models.Moderator
{
    public class tModerator
    {
        public int Id { get; set; }

        [Display(Name = "Путь к БТ")]
        public string tModerator_database { get; set; }
        [Display(Name = "Пользователь БТ")]
        public string tModerator_userId { get; set; }
        [Display(Name = "Пароль пользователя БТ")]
        public string tModerator_password { get; set; }

        public virtual ApplicationUser tUsers { get; set; }
    }
}