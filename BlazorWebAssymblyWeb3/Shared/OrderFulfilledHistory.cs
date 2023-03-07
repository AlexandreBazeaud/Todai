using System;
using System.Collections.Generic;

namespace BlazorWebAssymblyWeb3.Server
{
    public partial class OrderFulfilledHistory
    {
        public string? OrderJson { get; set; }
        public int ChainId { get; set; }
        public int TokenId { get; set; }
        public int CollectionId { get; set; }
        public string PriceWei { get; set; } = null!;
        public DateTime Date { get; set; }
        public bool IsOffer { get; set; }
        public int Id { get; set; }
        public string? Seller { get; set; }
        public string? Buyer { get; set; }

        public virtual Collection Collection { get; set; } = null!;
        public virtual Nft Nft { get; set; } = null!;
    }
}
