using BlazorWebAssymblyWeb3.Shared.Forms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorWebAssymblyWeb3.Server
{
	public partial class OfferListed
	{
		private SeaportOrderParameters? order;
		public SeaportOrderParameters? seaportOrder
		{
			get
			{
				if (string.IsNullOrWhiteSpace(OrderJson))
					return null;
				if (order is null)
					order = JsonConvert.DeserializeObject<SeaportOrderParameters>(OrderJson);

				return order;
			}
		}

		public bool IsPaintswap;
		public string? PaintswapUrl;
		public int PriceInt
		{
			get
			{
				return int.Parse(Price);
			}
		}

	}
}

