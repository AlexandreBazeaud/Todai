using BlazorWebAssymblyWeb3.Server;

namespace BlazorWebAssymblyWeb3.Shared.Forms;

public class ExplorerItemValue
{
    public string Url { get; set; }
    public string BannerUrl { get; set; }
    public string ProfilePicture { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public bool IsCollection { get; set; }
    public bool? IsVerified { get; set; }
    public bool? IsWhitelisted { get; set; }
    
    
    public static explicit operator ExplorerItemValue(Profile p) => new()
    {
        IsCollection = false,
        Description = p.Bio,
        BannerUrl = "media/profile_default_bg.png",
        Name = p.Name!,
        Url = "/profile/" + p.Address,
        ProfilePicture =  p.Nft?.Collection?.Address is null ? "" :  $"https://todai.world/images/{p.Nft.Collection.Address}/{p.TokenId}"
    };

    public static explicit operator ExplorerItemValue(Collection c) => new()
    {
        IsCollection = true,
        Description = c.Description,
        BannerUrl = $"https://todai.world/images/{c.Address}/BackgroundPicture",
        Name = c.Name,
        Url = "/collection/"+c.Address,
        ProfilePicture = $"https://todai.world/images/{c.Address}/ProfilePicture",
		IsWhitelisted = c.IsWhitelisted,
        IsVerified = c.IsVerified
    };
}