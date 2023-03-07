using System;
using System.Collections.Generic;

namespace BlazorWebAssymblyWeb3.Server
{
    public partial class RewardRarity
    {
        public RewardRarity()
        {
            RewardValues = new HashSet<RewardValue>();
            YokaiToolsGeneralChestCounts = new HashSet<YokaiToolsGeneralChestCount>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<RewardValue> RewardValues { get; set; }
        public virtual ICollection<YokaiToolsGeneralChestCount> YokaiToolsGeneralChestCounts { get; set; }
    }
}
