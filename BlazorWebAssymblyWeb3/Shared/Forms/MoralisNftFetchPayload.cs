using BlazorWebAssymblyWeb3.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorWebAssymblyWeb3.Shared.Forms
{
	public class Result
	{
		public string token_address { get; set; }
		public string token_id { get; set; }
		public string owner_of { get; set; }
		public string block_number { get; set; }
		public string block_number_minted { get; set; }
		public string token_hash { get; set; }
		public string amount { get; set; }
		public string contract_type { get; set; }
		public string name { get; set; }
		public string symbol { get; set; }
		public string token_uri { get; set; }
		public string metadata { get; set; }
		public DateTime? last_token_uri_sync { get; set; }
		public DateTime? last_metadata_sync { get; set; }
	}

	public class MoralisNftFetchPayload
	{
		public int total { get; set; }
		public int page { get; set; }
		public int page_size { get; set; }
		public string cursor { get; set; }
		public List<Result> result { get; set; }
		public string status { get; set; }
	}

	public record GetAllNftsFromPayload(Dictionary<string, int[]> userOwnedNFt, string pUserAddress);
	public record class GetAllNftsFromResponse(List<Nft> pNfts,int pNftToal);
	public record RoyaltyData(double? Royalty, string RoyaltyAddress);
}
