using System;
using System.Collections.Generic;

namespace BlazorWebAssymblyWeb3.Server
{
    public partial class CollectionLinkKeyword
    {
        public int CollectionId { get; set; }
        public int Id { get; set; }
        public string KeywordName { get; set; } = null!;

        public virtual Collection Collection { get; set; } = null!;
    }
}
