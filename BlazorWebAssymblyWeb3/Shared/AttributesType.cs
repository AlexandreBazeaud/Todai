using System;
using System.Collections.Generic;

namespace BlazorWebAssymblyWeb3.Server
{
    public partial class AttributesType
    {
        public AttributesType()
        {
            Attributeoptions = new HashSet<Attributeoption>();
            Attributes = new HashSet<Attribute>();
        }

        public int Id { get; set; }
        public int Index { get; set; }
        public string Name { get; set; } = null!;
        public int CollectionId { get; set; }
        public string Type { get; set; } = null!;
        public bool? ScoreNoCount { get; set; }

        public virtual Collection Collection { get; set; } = null!;
        public virtual ICollection<Attributeoption> Attributeoptions { get; set; }
        public virtual ICollection<Attribute> Attributes { get; set; }
    }
}
