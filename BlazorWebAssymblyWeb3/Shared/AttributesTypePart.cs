using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorWebAssymblyWeb3.Server
{
    public partial class AttributesType
    {
		[NotMapped]
		public int ActiveFilter { get; set; }
    }
}
