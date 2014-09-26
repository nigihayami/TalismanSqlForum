using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using TalismanSqlForum.Models.Offer;
using TalismanSqlForum.Models.Users;

namespace TalismanSqlForum.Models.Forum
{
    public class tForumMessages
    {
        public tForumMessages()
        {
            this.toffer = new HashSet<tOffer>();
            this.tusernewmessages = new HashSet<tUserNewMessages>();
        }
        public int Id { get; set; }
        [Required]
        [Display(Name = "Сообщение")]
        [AllowHtml]
        public string tForumMessages_messages { get; set; }
        [Required]
        public System.DateTime tForumMessages_datetime { get; set; }

        [AllowHtml]
        public string tForumMessages_offer { get; set; }

        public bool tForumMessages_hide { get; set; }


        public virtual tForumThemes tForumThemes { get; set; }
        public virtual ApplicationUser tUsers { get; set; }

        public virtual ApplicationUser tUsers_Edit_name { get; set; }
        public System.DateTime? tUsers_Edit_datetime { get; set; }

        public virtual ICollection<tOffer> toffer { get; set; }
        public virtual ICollection<tUserNewMessages> tusernewmessages { get; set; }
    }
}