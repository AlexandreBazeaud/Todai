using System;
using System.Collections.Generic;

namespace BlazorWebAssymblyWeb3.Server
{
    public partial class Collection
    {
        public Collection()
        {
            AttributesTypes = new HashSet<AttributesType>();
            CollectionLinkCategories = new HashSet<CollectionLinkCategory>();
            CollectionLinkKeywords = new HashSet<CollectionLinkKeyword>();
            FavoritedCollections = new HashSet<FavoritedCollection>();
            Nfts = new HashSet<Nft>();
            OfferListeds = new HashSet<OfferListed>();
            OrderFulfilledHistories = new HashSet<OrderFulfilledHistory>();
        }

        public int Id { get; set; }
        public string Address { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? ProfilePicture { get; set; }
        public string? Banner { get; set; }
        public bool IsSoldOut { get; set; }
        public bool IsRarityAble { get; set; }
        public int TotalSupply { get; set; }
        public bool? Finite { get; set; }
        public bool IsMintable { get; set; }
        public string? MintLink { get; set; }
        public int ChainId { get; set; }
        public string? Description { get; set; }
        public string? OwnerAddress { get; set; }
        public string? Website { get; set; }
        public string? Discord { get; set; }
        public bool? IsWhitelisted { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public int? StorageType { get; set; }
        public bool HaveCustomScore { get; set; }
        public string? Twitter { get; set; }
        public int Slice { get; set; }
        public bool? TokenIdChosen { get; set; }
        public bool IsVerified { get; set; }
        public bool IsOnArtion { get; set; }
        public int? MintPrice { get; set; }
        public bool NotAlwaysTheSameAttributeCount { get; set; }
        public string? NftKeyAlias { get; set; }
        public int CurrentSupply { get; set; }
        public bool StartAtZero { get; set; }
        public double? Royalty { get; set; }

        public virtual CollectionStorageType? StorageTypeNavigation { get; set; }
        public virtual ICollection<AttributesType> AttributesTypes { get; set; }
        public virtual ICollection<CollectionLinkCategory> CollectionLinkCategories { get; set; }
        public virtual ICollection<CollectionLinkKeyword> CollectionLinkKeywords { get; set; }
        public virtual ICollection<FavoritedCollection> FavoritedCollections { get; set; }
        public virtual ICollection<Nft> Nfts { get; set; }
        public virtual ICollection<OfferListed> OfferListeds { get; set; }
        public virtual ICollection<OrderFulfilledHistory> OrderFulfilledHistories { get; set; }
    }
}
