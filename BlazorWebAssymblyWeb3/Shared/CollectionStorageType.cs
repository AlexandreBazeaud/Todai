using System;
using System.Collections.Generic;

namespace BlazorWebAssymblyWeb3.Server
{
    public partial class CollectionStorageType
    {
        public CollectionStorageType()
        {
            Collections = new HashSet<Collection>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<Collection> Collections { get; set; }
    }
}
