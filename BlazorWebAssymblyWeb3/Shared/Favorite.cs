using System;
using System.Collections.Generic;

namespace BlazorWebAssymblyWeb3.Server
{
    public partial class Favorite
    {
        public int TokenId { get; set; }
        public string WalletAddress { get; set; } = null!;
        public DateTime Since { get; set; }
        public int CollectionId { get; set; }

        public virtual Nft Nft { get; set; } = null!;
    }
}
