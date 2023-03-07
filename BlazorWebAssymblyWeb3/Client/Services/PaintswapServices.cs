using System.Net.Http.Json;
using System.Numerics;
using System.Text.Json.Serialization;
using BlazorWebAssymblyWeb3.Client.Data;
using BlazorWebAssymblyWeb3.Client.Services;
using BlazorWebAssymblyWeb3.Server;

namespace BlazorWebAssymblyWeb3.Client;

public class PaintswapService
{
    private readonly HttpClient _httpClient;

    public PaintswapService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    private const int NUMTOFETCH = 100;

    public async Task<List<PaintSwapSale>> GetSalesForCollections(string pCollection)
    {
        var sales = new List<PaintSwapSale>();
        var result = await _httpClient.GetFromJsonAsync<PaintSwapSalesResult>($"sales?collections={pCollection}&includeActive=true&numToFetch={NUMTOFETCH}&numToSkip={sales.Count}");

        sales.AddRange(result.sales);
        return sales;
    }

    public async Task<PaintSwapSale?> GetSaleForNftIfExist(string pCollection, Yokai pNft)
    {
        var result = await _httpClient.GetFromJsonAsync<PaintSwapSalesResult>($"sales?collections={pCollection}&includeActive=true&numToFetch=1&numToSkip=0&search={pNft.Data.name}");


        var sale = result.sales.FirstOrDefault();
        if (sale is null || !int.TryParse(sale.tokenId, out int tokenId)) return null;

        return tokenId != pNft.TokenId ? null : sale;
    }

    
    public async Task<PaintswapCollection?> GetCollectionStats(string pCollection)
    {
        var httpResult = await _httpClient.GetAsync($"collections/{pCollection}");
        if (!httpResult.IsSuccessStatusCode) return null;
        
        var result = await httpResult.Content.ReadFromJsonAsync<PaintswapCollectionResult>();
        return result?.collection;
    }
    
    public class PaintSwapSalesResult
    {
        public PaintSwapSale[] sales { get; set; }
    }

    public class PaintSwapSale
    {
        public string tokenId { get; set; }
        public string price { get; set; }
        public string id { get; set; }
        public string address { get; set; }

        public string endTime { get; set; }


		private int? priceInt;

        public int PriceInt
        {
            get
            {
                priceInt ??= int.Parse(price.Substring(0, price.Length - 18));
                return priceInt.Value;
            }
        }

		public static explicit operator AssetListing(PaintSwapSale p) => new()
		{
			ChainId = 250,
            TokenId = int.Parse(p.tokenId),
            Price = Helper.ToEther(BigInteger.Parse(p.price)).ToString(),
            IsPaintswap = true,
            AssetUrl = $"https://paintswap.finance/marketplace/assets/{p.address}/{p.tokenId}"
		};
	}

    public class BigNumber
    {
        public string type { get; set; }
        public string hex { get; set; }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Sale
    {
        public BigNumber floorSaleTVL { get; set; }
        public string numActiveSales { get; set; }
        public string numTradesLast24Hours { get; set; }
        public string numTradesLast7Days { get; set; }
        public string timestampLastSale { get; set; }
        public string timestampLastTrim { get; set; }
        public string totalTrades { get; set; }
        public string totalVolumeTraded { get; set; }
        public string totalVolumeTradedFTM { get; set; }
        public string volumeLast24Hours { get; set; }
        public string volumeLast7Days { get; set; }
    }

    public class Stats
    {
        public string averagePrice { get; set; }
        public string floor { get; set; }
        public string floorFTM { get; set; }
        public string id { get; set; }
        public string lastSellPrice { get; set; }
        public string lastSellPriceFTM { get; set; }
        public string numOwners { get; set; }
        public string totalMinted { get; set; }
        public string totalNFTs { get; set; }
        public string totalVolumeTraded { get; set; }

        [JsonIgnore]
        public int TotalVolumeTraderFTMInt => totalVolumeTradedFTM?.Length > 18 ? int.Parse(totalVolumeTraded?.Substring(0, totalVolumeTraded.Length - 18) ?? "0") : 0;

        public string totalVolumeTradedFTM { get; set; }
        public string floorCap { get; set; }
        public Sale? sale { get; set; }
    }

    public class PaintswapCollection
    {
        public string id { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public string address { get; set; }
        public string owner { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public bool nsfw { get; set; }
        public int? mintPriceLow { get; set; }
        public int? mintPriceHigh { get; set; }
        public bool verified { get; set; }
        public int startBlock { get; set; }
        public string website { get; set; }
        public string twitter { get; set; }
        public string discord { get; set; }
        public string medium { get; set; }
        public string telegram { get; set; }
        public string reddit { get; set; }
        public string poster { get; set; }
        public string banner { get; set; }
        public string thumbnail { get; set; }
        public string standard { get; set; }
        public bool featured { get; set; }
        public bool displayed { get; set; }
        public object imageStyle { get; set; }
        public object customMetadata { get; set; }
        public Stats stats { get; set; }
    }

    public class PaintswapCollectionResult
    {
        public PaintswapCollection collection { get; set; }
    }


    
    public string GetBaseUrl()
    {
        return _httpClient.BaseAddress.ToString();
    }
}