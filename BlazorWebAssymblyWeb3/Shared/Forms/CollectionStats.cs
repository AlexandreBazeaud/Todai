using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorWebAssymblyWeb3.Shared.Forms
{
    public class CollectionStats
    {
		public int? Volume { get; set; }
		public int? Volume7D { get; set; }
		public int? Volume24H { get; set; }
		public int? NumberOfSales7D { get; set; }
		public int? NumberOfSales24H { get; set; }
		public int? NumberOfSales { get; set; }
		public double? AveragePrice { get; set; }
		public int? FloorPrice { get; set; }
	}
}
