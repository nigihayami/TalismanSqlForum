using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TalismanSqlForum.Models.Forum;

namespace TalismanSqlForum.Models.Users
{
    public class tUserNewThemes
    {
        public int Id { get; set; }
        public virtual ApplicationUser tUsers { get; set; }
        public virtual tForumThemes tForumThemes { get; set; }
    }
}