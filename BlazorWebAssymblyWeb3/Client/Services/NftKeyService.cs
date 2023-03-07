using BlazorWebAssymblyWeb3.Server;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Newtonsoft.Json;
using System.Numerics;
using System.Text.Json.Serialization;
using static BlazorWebAssymblyWeb3.Client.PaintswapService;
using JsonIgnoreAttribute = System.Text.Json.Serialization.JsonIgnoreAttribute;

namespace BlazorWebAssymblyWeb3.Client.Services
{
	public class NftKeyService
	{
		private readonly GraphQLHttpClient _graphQlHttpClient;
		public NftKeyService()
		{
			var set = new JsonSerializerSettings()
			{
				ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
			};
			_graphQlHttpClient = new GraphQLHttpClient("https://nftkey.app/graphql", new NewtonsoftJsonSerializer(set));
		}

		public async Task<NFTKeyCollection> GetCollectionDataAsync(string pAlias)
		{
			var str = @"query GetERC721Collections($alias: String!) {
						  erc721CollectionByAlias(alias: $alias) {
						    ...ERC721CollectionInfo
						    __typename
						  }
						}
						
						fragment ERC721CollectionInfo on ERC721CollectionInfo {
						  id
						  alias
						  name
						  itemName
						  websiteUrl
						  bannerUrl
						  thumbnailUrl
						  isPixel
						  frontendPath
						  socialShareUrl
						  maxSupply
						  totalSupply
						  validSupply
						  totalVolume
						  totalVolumeUsd
						  last24HVolume
						  last24HVolumeUsd
						  last7DVolume
						  last7DVolumeUsd
						  last30DVolume
						  last30DVolumeUsd
						  totalSalesCount
						  address
						  marketplaceV2Address
						  chainId
						  isReleased
						  isPulling
						  isNoRarity
						  isDelisted
						  isArtist
						  floor
						  objectFit
						  __typename
						}";


			var req = new GraphQLRequest
			{
				Query = str,
				Variables = new
				{
					alias = pAlias
				}
			};

			var a = await _graphQlHttpClient.SendQueryAsync<NFTCollection>(req);
			return a.Data.erc721CollectionByAlias;
		}

		public async Task<NFTKeyCollection[]> GetCollectionsDataAsync()
		{
			var str = @"query GetERC721Collections {
						  erc721Collections {
						    ...ERC721CollectionInfo
						    __typename
						  }
						}
						
						fragment ERC721CollectionInfo on ERC721CollectionInfo {
						  id
						  alias
						  name
						  itemName
						  websiteUrl
						  bannerUrl
						  thumbnailUrl
						  isPixel
						  frontendPath
						  socialShareUrl
						  maxSupply
						  totalSupply
						  validSupply
						  totalVolume
						  totalVolumeUsd
						  last24HVolume
						  last24HVolumeUsd
						  last7DVolume
						  last7DVolumeUsd
						  last30DVolume
						  last30DVolumeUsd
						  totalSalesCount
						  address
						  marketplaceV2Address
						  chainId
						  isReleased
						  isPulling
						  isNoRarity
						  isDelisted
						  isArtist
						  floor
						  objectFit
						  __typename
						}";


			var req = new GraphQLRequest
			{
				Query = str
			};

			var a = await _graphQlHttpClient.SendQueryAsync<NFTTEST>(req);
			return a.Data.erc721Collections;
		}
	}

	public class NftKeySale
	{
		public string TokenId { get; set; }

		public string Price { get; set; }
		public string expireTimestamp { get; set; }

		[JsonIgnore]
		private int? priceInt { get; set; }

		[JsonIgnore]
		public int PriceInt
		{
			get
			{
				if (Price.Length < 18) return 0;
				priceInt ??= int.Parse(Price.Substring(0, Price.Length - 18));
				return priceInt.Value;
			}
		}
		public string NftKeySlang { get; set; }
		public string Address { get; set; }
		public static explicit operator AssetListing(NftKeySale p) => new()
		{
			ChainId = 250,
			TokenId = int.Parse(p.TokenId),
			Price = Helper.ToEther(BigInteger.Parse(p.Price)).ToString(),
			IsNftKey = true,
			AssetUrl = $"https://nftkey.app/collections/{p.NftKeySlang}/token-details/?tokenId={p.TokenId}"
		};
	}


	public class NFTTEST
	{
		public NFTKeyCollection[] erc721Collections { get; set; }

	}

	public class NFTCollection
	{
		public NFTKeyCollection erc721CollectionByAlias { get; set; }

	}
	#region data
	public class NFTKeyCollection
	{
		public string id { get; set; }
		public string alias { get; set; }
		public string name { get; set; }
		public string itemName { get; set; }
		public string websiteUrl { get; set; }
		public string bannerUrl { get; set; }
		public string thumbnailUrl { get; set; }
		public bool? isPixel { get; set; }
		public string? frontendPath { get; set; }
		public string socialShareUrl { get; set; }
		public int? maxSupply { get; set; }
		public int? totalSupply { get; set; }
		public int? validSupply { get; set; }
		public double totalVolume { get; set; }
		public double totalVolumeUsd { get; set; }
		public double last24HVolume { get; set; }
		public double last24HVolumeUsd { get; set; }
		public double last7DVolume { get; set; }
		public double last7DVolumeUsd { get; set; }
		public double last30DVolume { get; set; }
		public double last30DVolumeUsd { get; set; }
		public int totalSalesCount { get; set; }
		public string address { get; set; }
		public string? marketplaceV2Address { get; set; }
		public int chainId { get; set; }
		public bool? isReleased { get; set; }
		public bool? isPulling { get; set; }
		public bool? isNoRarity { get; set; }
		public bool? isDelisted { get; set; }
		public bool? isArtist { get; set; }
		public double? floor { get; set; }
		public string? objectFit { get; set; }
		public string __typename { get; set; }
	}
	#endregion

}
