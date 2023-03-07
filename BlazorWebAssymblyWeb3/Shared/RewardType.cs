using System;
using System.Collections.Generic;

namespace BlazorWebAssymblyWeb3.Server
{
    public partial class RewardType
    {
        public RewardType()
        {
            RewardValues = new HashSet<RewardValue>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<RewardValue> RewardValues { get; set; }
    }
}
