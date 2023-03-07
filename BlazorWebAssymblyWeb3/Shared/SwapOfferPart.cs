using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using BlazorWebAssymblyWeb3.Client.Data;

namespace BlazorWebAssymblyWeb3.Server
{
    public partial class SwapOffer
    { 
        [NotMapped]
        public string TargetOwnerNickname { get; set; }
        [NotMapped]
        public string OffererNickname { get; set; }
        [NotMapped]
        public TimeSpan TimeUntilExpire { get; set; }
        [NotMapped]
        public Yokai OfferedNft { get; set; }
        [NotMapped]
        public Yokai TargetedNft { get; set; }
        [NotMapped]
        public string TargetOwnerAddress { get; set; }
    }
}
