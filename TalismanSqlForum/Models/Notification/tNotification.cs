namespace TalismanSqlForum.Models.Notification
{
    public class tNotification
    {
        public int Id { get; set; }
        public System.DateTime tNotification_date { get; set; }
        public bool tNotification_IsRead { get; set; }
        public string tNotification_message { get; set; }
        public string tNotification_href { get; set; }

        public virtual tNotificationType tNotificationType { get; set; }
        public virtual ApplicationUser tUsers { get; set; }
    }
}