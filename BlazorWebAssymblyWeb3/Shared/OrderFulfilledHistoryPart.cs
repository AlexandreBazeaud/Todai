using BlazorWebAssymblyWeb3.Client.Data;
using BlazorWebAssymblyWeb3.Shared.Forms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorWebAssymblyWeb3.Server
{
	public partial class OrderFulfilledHistory
	{
		private Yokai? yokai;

		public Yokai Yokai
		{
			get
			{
				if (yokai is null)
				{
					yokai = new Yokai(TokenId);

					if (Collection != null)
						yokai.ContractAddress = Collection.Address;

					if (Nft != null)
					{
						yokai.Data = new YokaiData { name = Nft.Name };

                        if (Nft.Rarity != null)
                            yokai.Rank = Nft.Rarity.Rank;
                    }
				}
				return yokai;
			}
		}

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

		private int? priceInt;
		public int? PriceInt
		{
			get
			{
				if (priceInt is null)
					priceInt = int.Parse(PriceWei);


				return priceInt;
			}
		}

	}
}
