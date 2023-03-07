using System;
using System.Collections.Generic;

namespace BlazorWebAssymblyWeb3.Server
{
    public partial class Category
    {
        public Category()
        {
            CollectionLinkCategories = new HashSet<CollectionLinkCategory>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<CollectionLinkCategory> CollectionLinkCategories { get; set; }
    }
}
