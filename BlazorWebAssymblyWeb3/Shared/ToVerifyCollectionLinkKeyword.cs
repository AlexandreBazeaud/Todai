using System;
using System.Collections.Generic;

namespace BlazorWebAssymblyWeb3.Server
{
    public partial class ToVerifyCollectionLinkKeyword
    {
        public int ToVerifyCollectionId { get; set; }
        public int KeywordId { get; set; }
        public int Id { get; set; }

        public virtual Keyword Keyword { get; set; } = null!;
        public virtual ToVerifyCollection ToVerifyCollection { get; set; } = null!;
    }
}
