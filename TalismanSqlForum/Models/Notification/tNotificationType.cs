using System.Collections.Generic;

namespace TalismanSqlForum.Models.Notification
{
    public class tNotificationType
    {
        public tNotificationType()
        {
            this.tNotification = new HashSet<tNotification>();
        }
        public int Id { get; set; }
        public string tNotificationType_name { get; set; }

        public virtual ICollection<tNotification> tNotification { get; set; }
    }
}