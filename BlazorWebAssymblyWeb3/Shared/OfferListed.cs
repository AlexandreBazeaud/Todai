using System;
using System.Collections.Generic;

namespace BlazorWebAssymblyWeb3.Server
{
    public partial class OfferListed
    {
        public int Id { get; set; }
        public string OrderJson { get; set; } = null!;
        public string OrderHash { get; set; } = null!;
        public string Signature { get; set; } = null!;
        public int TokenId { get; set; }
        public int ChainId { get; set; }
        public string Price { get; set; } = null!;
        public DateTime CreationDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public int CollectionId { get; set; }
        public string? Offerer { get; set; }
        public string? Receiver { get; set; }
        public bool? Hidden { get; set; }

        public virtual Collection Collection { get; set; } = null!;
        public virtual Nft Nft { get; set; } = null!;
    }
}
