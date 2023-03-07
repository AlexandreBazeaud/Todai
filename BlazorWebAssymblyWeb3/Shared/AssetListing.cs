using System;
using System.Collections.Generic;

namespace BlazorWebAssymblyWeb3.Server
{
    public partial class AssetListing
    {
        public int Id { get; set; }
        public string OrderJson { get; set; } = null!;
        public string OrderHash { get; set; } = null!;
        public string Signature { get; set; } = null!;
        public string CollectionAddress { get; set; } = null!;
        public int? ChainId { get; set; }
        public string Price { get; set; } = null!;
        public int TokenId { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string? Offerer { get; set; }
    }
}
