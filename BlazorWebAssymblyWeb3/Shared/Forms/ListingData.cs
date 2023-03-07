using System.ComponentModel.DataAnnotations;

namespace BlazorWebAssymblyWeb3.Shared.Forms;
#pragma warning disable CS8618
public class ListingData
{
    public ListingData()
    {
        SocialLinks = new();
    }
    
    [Required]
    public string ProjectName { get; set; }
	
    public int MintPrice { get; set; }
    public DateTime ReleaseDate { get; set; } = DateTime.Now;
    public string? Storage { get; set; }
    public string MintStatut { get; set; }
    public string? Format { get; set; }
    public string? TotalSupply { get; set; }
    public SocialLinks SocialLinks { get; set; }
    public bool IsRarityAble { get; set; }
    
    [Required]
	[MaxLength(42), MinLength(42)]
    public string ContractAddress { get; set; }
    public string? Keywords { get; set; }
    public int Blockchain { get; set; }

    public bool Validate()
    {
        if (string.IsNullOrWhiteSpace(ProjectName))
            return false;
        if (string.IsNullOrWhiteSpace(ContractAddress))
            return false;

        return true;
    }
}

public class SocialLinks
{
    public string? Twitter { get; set; }
    public string? Website { get; set; }
    public string? Discord { get; set; }
    public string? Telegram { get; set; }
    public string? KakaoTalk { get; set; }
    public string? Medium { get; set; }
}
#pragma warning restore CS8618