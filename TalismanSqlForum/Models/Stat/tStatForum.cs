using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Web;
using TalismanSqlForum.Models.Forum;

namespace TalismanSqlForum.Models.Stat
{
    public class StatForum
    {
        public int Id { get; set; }
        public string StatForumIp { get; set; }
    }
    public class StatForumList
    {
        public int Id { get; set; }
        public string StatForumIp { get; set; }
        public virtual tForumList TForumLists { get; set; } 
    }
}