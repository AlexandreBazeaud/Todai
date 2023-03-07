using System;
using System.Collections.Generic;

namespace BlazorWebAssymblyWeb3.Server
{
    public partial class Nft
    {
        public Nft()
        {
            Attributes = new HashSet<Attribute>();
            Favorites = new HashSet<Favorite>();
            OfferListeds = new HashSet<OfferListed>();
            OrderFulfilledHistories = new HashSet<OrderFulfilledHistory>();
        }

        public int CollectionId { get; set; }
        public int TokenId { get; set; }
        public string? Name { get; set; }

        public virtual Collection Collection { get; set; } = null!;
        public virtual Profile? Profile { get; set; }
        public virtual Rarity Rarity { get; set; } = null!;
        public virtual ICollection<Attribute> Attributes { get; set; }
        public virtual ICollection<Favorite> Favorites { get; set; }
        public virtual ICollection<OfferListed> OfferListeds { get; set; }
        public virtual ICollection<OrderFulfilledHistory> OrderFulfilledHistories { get; set; }
    }
}
