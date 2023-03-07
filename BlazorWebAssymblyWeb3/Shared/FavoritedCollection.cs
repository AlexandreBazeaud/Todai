using System;
using System.Collections.Generic;

namespace BlazorWebAssymblyWeb3.Server
{
    public partial class FavoritedCollection
    {
        public string WalletAddress { get; set; } = null!;
        public int CollectionId { get; set; }
        public DateTime Since { get; set; }

        public virtual Collection Collection { get; set; } = null!;
    }
}
