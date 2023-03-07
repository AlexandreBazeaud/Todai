using BlazorWebAssymblyWeb3.Server.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BlazorWebAssymblyWeb3.Server.Pages
{
    public class _HostModel : PageModel
    {
        public string SiteName { get; set; } = "Todai(beta) - NFT platform";
        public string PageDescription { get; set; } = "Take control over the world of digital assets with Todai";
        public string Image { get; set; } = "https://todai.world/media/Todai_logo.png";
        public async Task OnGetAsync()
        {
            (SiteName, PageDescription, Image) = await GetMetaData();
        }

        private readonly YokaiToolsContext _context;
        public _HostModel(YokaiToolsContext context)
        {
            _context = context;
        }
        
        private async Task<(string, string, string)> GetMetaData()
        {
            if (Request.Path.HasValue && Request.Path.Value.Contains("asset"))
            {
                var values = Request.Path.Value.Split('/');

                if (!int.TryParse(values[3], out var tokenId))
                    return ("Todai(beta) - NFT platform", "Take control over the world of digital assets with Todai", "https://todai.world/media/Todai_logo.png");

				return (await _context.Nfts.Where(x => x.TokenId == tokenId && x.Collection.Address == values[2]).Select(x => x.Name).FirstOrDefaultAsync() ?? "Asset","Todai(beta) - NFT platform",$"https://todai.world/images/{values[2]}/{tokenId}.png");
            }

            return ("Todai(beta) - NFT platform", "Take control over the world of digital assets with Todai", "https://todai.world/media/Todai_logo.png");
        }
    }
}