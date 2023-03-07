using System;
using System.Collections.Generic;

namespace BlazorWebAssymblyWeb3.Server
{
    public partial class RewardValue
    {
        public int Id { get; set; }
        public int RewardType { get; set; }
        public int RewardRarity { get; set; }
        public decimal Value { get; set; }
        public string EarnerAddress { get; set; } = null!;
        public string? NftAddress { get; set; }
        public int? TokenId { get; set; }
        public bool Opened { get; set; }

        public virtual RewardRarity RewardRarityNavigation { get; set; } = null!;
        public virtual RewardType RewardTypeNavigation { get; set; } = null!;
    }
}
