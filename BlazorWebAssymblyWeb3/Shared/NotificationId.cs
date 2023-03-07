using System;
using System.Collections.Generic;

namespace BlazorWebAssymblyWeb3.Server
{
    public partial class NotificationId
    {
        public NotificationId()
        {
            Notifications = new HashSet<Notification>();
        }

        public int Id { get; set; }
        public string Nom { get; set; } = null!;
        public string? Message { get; set; }

        public virtual ICollection<Notification> Notifications { get; set; }
    }
}
