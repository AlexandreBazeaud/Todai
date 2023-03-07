using System;
using System.Collections.Generic;

namespace BlazorWebAssymblyWeb3.Server
{
    public partial class SwapOffer
    {
        public string Offerer { get; set; } = null!;
        public string TargetCollection { get; set; } = null!;
        public string OfferCollection { get; set; } = null!;
        public int TargetTokenId { get; set; }
        public int OfferTokenId { get; set; }
        public long EndTimestamp { get; set; }
        public int Id { get; set; }
        public string? OwnerOfTargeted { get; set; }
    }
}
