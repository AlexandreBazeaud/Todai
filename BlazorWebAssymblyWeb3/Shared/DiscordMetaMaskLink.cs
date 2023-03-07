using System;
using System.Collections.Generic;

namespace BlazorWebAssymblyWeb3.Server
{
    public partial class DiscordMetaMaskLink
    {
        public string DiscordId { get; set; } = null!;
        public string Address { get; set; } = null!;
        public DateTime LastUpdate { get; set; }
        public string Guid { get; set; } = null!;
        public DateTime Created { get; set; }
    }
}
