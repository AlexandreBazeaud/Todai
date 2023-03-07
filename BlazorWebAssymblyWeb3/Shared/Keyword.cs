using System;
using System.Collections.Generic;

namespace BlazorWebAssymblyWeb3.Server
{
    public partial class Keyword
    {
        public Keyword()
        {
            ToVerifyCollectionLinkKeywords = new HashSet<ToVerifyCollectionLinkKeyword>();
        }

        public int Id { get; set; }
        public string Keyword1 { get; set; } = null!;

        public virtual ICollection<ToVerifyCollectionLinkKeyword> ToVerifyCollectionLinkKeywords { get; set; }
    }
}
