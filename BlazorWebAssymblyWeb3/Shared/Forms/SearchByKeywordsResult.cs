using BlazorWebAssymblyWeb3.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorWebAssymblyWeb3.Shared.Forms
{
	public record class SearchByKeywordsResult(List<Profile> Profiles, List<Collection> Collections);
	public record class OrderAndOffersData(List<OfferListed> Offers, List<AssetListing> Listings);
	//{
	//	public readonly List<Profile> Profiles { get; set; }
	//	public readonly List<Collection> Collections { get; set; }
	//}
}
