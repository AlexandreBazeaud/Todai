using System;
using System.Collections.Generic;

namespace BlazorWebAssymblyWeb3.Server
{
    public partial class Notification
    {
        public int Id { get; set; }
        public string Address { get; set; } = null!;
        public int EventId { get; set; }
        public DateTime Date { get; set; }
        public string? Data { get; set; }
        public bool Read { get; set; }

        public virtual NotificationId Event { get; set; } = null!;
    }
}
