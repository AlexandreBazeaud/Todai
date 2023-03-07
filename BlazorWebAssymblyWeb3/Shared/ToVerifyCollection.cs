using System;
using System.Collections.Generic;

namespace BlazorWebAssymblyWeb3.Server
{
    public partial class ToVerifyCollection
    {
        public ToVerifyCollection()
        {
            ToVerifyCollectionLinkKeywords = new HashSet<ToVerifyCollectionLinkKeyword>();
        }

        public int Id { get; set; }
        public string Address { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string ProfilePicture { get; set; } = null!;
        public string Banner { get; set; } = null!;
        public bool IsSoldOut { get; set; }
        public bool IsRarityAble { get; set; }
        public int TotalSupply { get; set; }
        public bool Finite { get; set; }
        public bool IsMintable { get; set; }
        public string MintLink { get; set; } = null!;
        public int ChainId { get; set; }
        public string? Description { get; set; }
        public string? Keywords { get; set; }
        public string AddressOfLister { get; set; } = null!;
        public int MintPrice { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string? Twitter { get; set; }

        public virtual ICollection<ToVerifyCollectionLinkKeyword> ToVerifyCollectionLinkKeywords { get; set; }
    }
}
