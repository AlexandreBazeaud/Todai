using System.Text;
using System.Text.Json.Serialization;

namespace BlazorWebAssymblyWeb3.Client.Data;

public class Yokai
{
    public int TokenId { get; init; }

    //[Parameter]

    private YokaiData? data;
    public YokaiData? Data
    {
        get
        {
            return data;
        }
        set
        {
            if (value is null)
            {
                data = value;
                return;
            }

            if (!string.IsNullOrWhiteSpace(value.image) && value.image.StartsWith("ipfs://"))
                value.image = value.image.Replace("ipfs://","https://ipfs.io/ipfs/");
            data = value;
        }
    }
    public string BlockChainTokenUri { get; set; }

    public bool IsDownloaded => Data != null;
    public bool IsNonExistent = false;
    public int Rank { get; set; }
    public string ContractAddress { get; set; }
		
    //private readonly Function _tokenUriFunction;

    public Yokai()
    {
        
    }
    
    public Yokai(int pTokenId)
    {
        TokenId = pTokenId;
    }
    
    public Yokai(int pTokenId, int pRank) : this(pTokenId)
    {
        Rank = pRank;
    }

    public Yokai(int pTokenId, int pRank, string pAddress) : this(pTokenId, pRank)
    {
        ContractAddress = pAddress.ToLower();
    }

    public Yokai(int pTokenId, string pAddress) : this(pTokenId)
    {
        ContractAddress = pAddress.ToLower();
    }

    public Yokai(int pTokenId, string pAddress, string pBlockchainUri) : this(pTokenId, pAddress)
    {
        BlockChainTokenUri = pBlockchainUri;
    }
}

public class YokaiData
{
    public string image { get; set; }
    public string name { get; set; }
    public string description { get; set; }
    public NftAttributes[] attributes { get; set; }
}

public class NftAttributes
{
    public string trait_type { get; set; }
    public string value { get; set; }
}
