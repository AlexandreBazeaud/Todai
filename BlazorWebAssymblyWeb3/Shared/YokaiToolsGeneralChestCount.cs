using System;
using System.Collections.Generic;

namespace BlazorWebAssymblyWeb3.Server
{
    public partial class YokaiToolsGeneralChestCount
    {
        public int RarityId { get; set; }
        public int Count { get; set; }
        public int Edition { get; set; }

        public virtual RewardRarity Rarity { get; set; } = null!;
    }
}
