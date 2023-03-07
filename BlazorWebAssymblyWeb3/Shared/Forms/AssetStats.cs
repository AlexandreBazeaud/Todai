using BlazorWebAssymblyWeb3.Client.Data;
using BlazorWebAssymblyWeb3.Server;

namespace BlazorWebAssymblyWeb3.Shared.Forms;

public class AssetStats
{
    public int FavoriteCount { get; set; }
    public int Score { get; set; }
    public int Rank { get; set; }
    public bool IsFavoritedByUser { get; set; }
    public string? Name { get; set; }
    public int ActualSupply { get; set; }
    public int CollectionId { get; set; }
    public string? CollectionPicture { get; set; }
    public string? CollectionName { get; set; }
    public bool IsWhitelisted { get; set; }
    public bool IsVerified { get; set; }
    public int? TotalSupply { get; set; }
    public string? ProfileName { get; set; }
    public string? ProfilePictureCollection { get; set; }
    public int? ProfilePictureTokenId { get; set; }
    public int? ChainId { get; set; }
    public string? NftKeyAlias { get; set; }
    public Yokai? ProfilePictureNFt { get; set; } = null;

    public int? TokenId { get; set; }


    public List<AssetAttributesScore> AssetAttributesScores { get; set; } = new();
    public AssetListing? AssetListing { get; set; }
    public List<OfferListed>? OffersListed { get; set; }
}

public class AssetAttributesScore
{
    public string AttributeTypeName { get; set; } = "";
    public string AttributeOptionName { get; set; } = "";
    public double Score { get; set; }
    public int  ApparitionCount { get; set; }
    public bool ShouldBeCounted { get; set; }
}