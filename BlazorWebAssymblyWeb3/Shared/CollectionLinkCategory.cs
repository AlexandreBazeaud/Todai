using System;
using System.Collections.Generic;

namespace BlazorWebAssymblyWeb3.Server
{
    public partial class CollectionLinkCategory
    {
        public int CollectionId { get; set; }
        public int CategoryId { get; set; }
        public int Id { get; set; }

        public virtual Category Category { get; set; } = null!;
        public virtual Collection Collection { get; set; } = null!;
    }
}
