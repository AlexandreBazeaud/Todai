using System;
using System.Collections.Generic;

namespace BlazorWebAssymblyWeb3.Server
{
    public partial class Attribute
    {
        public int Id { get; set; }
        public int AttributeTypeOptionId { get; set; }
        public int AttributeTypeId { get; set; }
        public int CollectionId { get; set; }
        public int TokenId { get; set; }

        public virtual AttributesType AttributeType { get; set; } = null!;
        public virtual Attributeoption AttributeTypeOption { get; set; } = null!;
        public virtual Nft Nft { get; set; } = null!;
    }
}
