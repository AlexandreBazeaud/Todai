using BlazorWebAssymblyWeb3.Shared.Forms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorWebAssymblyWeb3.Server
{
	public partial class AssetListing
	{
		private SeaportOrder? order;
		public SeaportOrder? seaportOrder
		{
			get
			{
				if (string.IsNullOrWhiteSpace(OrderJson))
					return null;
				if (order is null)
					order = JsonConvert.DeserializeObject<SeaportOrder>(OrderJson);

				return order;
			}
		}

		public bool IsPaintswap;
		public bool IsNftKey;
		public string? AssetUrl;
		public int PriceInt
		{
			get
			{
				return int.Parse(Price);
			}
		}
	}
}
