﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TalismanSqlForum.Models.Forum;

namespace TalismanSqlForum.Models.Users
{
    public class tUserNewMessages
    {
        public int Id { get; set; }
        public virtual ApplicationUser tUsers { get; set; }
        public virtual tForumMessages tForumMessages { get; set; }
    }
}