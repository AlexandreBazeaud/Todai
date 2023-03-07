using System;
using System.Collections.Generic;

namespace BlazorWebAssymblyWeb3.Server
{
    public partial class Attributeoption
    {
        public Attributeoption()
        {
            Attributes = new HashSet<Attribute>();
        }

        public int Id { get; set; }
        public string OptionValue { get; set; } = null!;
        public int AttributeTypeId { get; set; }
        public int Score { get; set; }

        public virtual AttributesType AttributeType { get; set; } = null!;
        public virtual ICollection<Attribute> Attributes { get; set; }
    }
}
