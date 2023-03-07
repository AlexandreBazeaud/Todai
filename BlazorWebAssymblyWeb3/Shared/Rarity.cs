using System;
using System.Collections.Generic;

namespace BlazorWebAssymblyWeb3.Server
{
    public partial class Rarity
    {
        public int TokenId { get; set; }
        public int CollectionId { get; set; }
        public int Rank { get; set; }
        public int Score { get; set; }
        public int CustomScore { get; set; }
        public int? CustomRank { get; set; }

        public virtual Nft Nft { get; set; } = null!;
    }
}
