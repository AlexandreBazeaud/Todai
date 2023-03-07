using BlazorWebAssymblyWeb3.Server.Data;
using BlazorWebAssymblyWeb3.Server.Services;
using BlazorWebAssymblyWeb3.Shared;
using BlazorWebAssymblyWeb3.Shared.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nethereum.Signer;
using Nethereum.Signer.EIP712;
using Nethereum.Util;
using System.Linq;
using System.Runtime.InteropServices;

namespace BlazorWebAssymblyWeb3.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class CollectionController : Controller
{
    private readonly YokaiToolsContext _context;
    private readonly SignerHelper _signerHelper;

    public CollectionController(YokaiToolsContext pContext, SignerHelper pSignerhelper)
    {
        _context = pContext;
        _signerHelper = pSignerhelper;
    }

    #region GET

    [HttpGet("Top8Collection")]
    public async Task<IActionResult> Top8Collection(string pCollectionAddress)
    {
        return Json(await _context.Rarities.AsNoTracking().Include(x => x.Nft).ThenInclude(x => x.Collection).Where(x => x.Nft.Collection.Address.ToLower() == pCollectionAddress.ToLower() && x.Rank > 0).OrderBy(x => x.Rank).Take(8).ToListAsync());
    }
    
    [HttpGet("UserFavoritesOf")]
    public async Task<IActionResult> UserFavoritesOf(int pCollectionId, string pWalletAddress)
    {
        var favorites = await _context.Favorites.AsNoTracking().Where(x => x.CollectionId == pCollectionId).Select(x => x.TokenId).ToListAsync();
        return Json(favorites);
    }
    
    [HttpGet("GetRandomCollection")]
    public async Task<IActionResult> GetRandomCollection(int pChainId = 250)
    {
        var address =  await _context.Collections.AsNoTracking().Where(x => x.IsVerified && x.ChainId == pChainId).OrderBy(r => Guid.NewGuid()).Take(1).Select(x => x.Address).FirstOrDefaultAsync();
        return Ok(address);
    }
    
    [HttpGet("GetCollectionsCount")]
    public async Task<IActionResult> GetCollections(string? pFilter = "")
    {
        var keywordsMatch = await _context.CollectionLinkKeywords.Where(x => !string.IsNullOrWhiteSpace(pFilter) && x.KeywordName.ToLower() == pFilter.ToLower()).AsNoTracking().Select(x => x.CollectionId).ToListAsync();
        var count = await _context.Collections
            .Where(x => string.IsNullOrWhiteSpace(pFilter) || x.Name.Contains(pFilter) || keywordsMatch.Contains(x.Id))
            .AsNoTracking().CountAsync();
        return Ok(count);
    }
    
    [HttpGet("GetCollections")]
    public async Task<IActionResult> GetCollections(int pSkip = 0, int pFetch = 0, string? pFilter = "")
    {
        if (pFetch > 25 || pFetch == 0)
            pFetch = 25;
        var keywordsMatch = await _context.CollectionLinkKeywords.Where(x => x.KeywordName.ToLower() == pFilter.ToLower()).AsNoTracking().Select(x => x.CollectionId).ToListAsync();
        var count = await _context.Collections
            .Where(x => string.IsNullOrWhiteSpace(pFilter) || x.Name.Contains(pFilter) || keywordsMatch.Contains(x.Id))
            .AsNoTracking().CountAsync();//max to load

        var whitelistedCollections = await _context.Collections
            .Where(x => string.IsNullOrWhiteSpace(pFilter) || x.Name.Contains(pFilter) || keywordsMatch.Contains(x.Id)).Skip(pSkip).Take(pFetch)
            .AsNoTracking().Select(x => new Collection
            {
                Address = x.Address,
                Name = x.Name,
                Id = x.Id,
                ProfilePicture = x.ProfilePicture,
                ChainId = x.ChainId,
                //TotalSupply = x.TotalSupply,
                Finite = x.Finite,
                //Website = x.Website,
                IsWhitelisted = x.IsWhitelisted,
                Description = x.Description,
                Banner = x.Banner,
                IsVerified = x.IsVerified
            }).ToListAsync();

        return Json(new Explorer.ExplorerPayload(whitelistedCollections, count));
    }

	[HttpGet("GetOwnedCollections")]
    public async Task<IActionResult> GetOwnedCollections(string pAddress)
	{
        var owned = await _context.Collections.AsNoTracking().Where(x => x.OwnerAddress != null && x.OwnerAddress.ToLower() == pAddress.ToLower()).Select(x => new Collection()
        {
            Address = x.Address,
            Name = x.Name
        }).ToListAsync();

        return Json(owned);
	}

    [HttpGet("GetRoyalty")]
    public async Task<IActionResult> GetRoyalty(string collectionAddress)
    {
        collectionAddress = collectionAddress.ToLower();

        var royaltyData = await _context.Collections.AsNoTracking().Where(x => x.Address == collectionAddress).Select(x => new RoyaltyData(x.Royalty, x.OwnerAddress)).FirstOrDefaultAsync();
        
        return Json(royaltyData);
    }
	#endregion

	#region POST
	public record struct FavoritePayload(int pCollectionId, string pSignatureHash, string pAddress);
    [HttpPost("Favorite")]
    public async Task<IActionResult> FavoriteAsset([FromBody]FavoritePayload pPayload)
    {
        if (!_signerHelper.TryGetGuidFor(pPayload.pAddress, out Guid guid))
            return NotFound();

        var typedData = SignerHelper.Default(guid.ToString());

        var address = new MessageSigner().EcRecover(
            Sha3Keccack.Current.CalculateHash(Eip712TypedDataSigner.Current.EncodeTypedData(typedData)),
            pPayload.pSignatureHash);
        if (address.ToLower() != pPayload.pAddress.ToLower()) return BadRequest();

        var favorite = await _context.FavoritedCollections.AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.CollectionId == pPayload.pCollectionId &&
                x.WalletAddress.ToLower() == pPayload.pAddress.ToLower());

        if (favorite is not null) return Ok();
        favorite = new FavoritedCollection
        {
            WalletAddress = pPayload.pAddress,
            CollectionId = pPayload.pCollectionId,
            Since = DateTime.UtcNow,
        };
        _context.FavoritedCollections.Add(favorite);
        await _context.SaveChangesAsync();

        return Ok();
    }

    private string[] acceptedExtension = new[] { ".png", ".jpg", ".svg" };
	[HttpPost("UpdateProfilePicture")]
    public async Task<IActionResult> UpdateProfilePicture([FromForm] string hash, [FromForm] string userAddress, [FromForm] string collectionAddress, [FromForm] IEnumerable<IFormFile> files)
	{
        if(!_signerHelper.VerifyHashFor(userAddress, hash)) return Unauthorized();

        long maxFileSize = 1024 * 1024 * 15;
        var file = files.Single();
        if (file.Length > maxFileSize)
            return BadRequest("File too big");

        if(!acceptedExtension.Contains(Path.GetExtension(file.FileName).ToLower()))
            return BadRequest();

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            await using var localFile = System.IO.File.Create($"/home/shared/images/{collectionAddress}/ProfilePicture{Path.GetExtension(file.FileName)}");
            await file.CopyToAsync(localFile);
            localFile.Close();
		}
		else
		{
            var directoryPath = $"{AppContext.BaseDirectory}/{collectionAddress}";

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

			await using var localFile = System.IO.File.Create($"{directoryPath}/ProfilePicture{Path.GetExtension(file.FileName)}");
            await file.CopyToAsync(localFile);
			localFile.Close();
		}
        return Ok();
	}

    [HttpPost("UpdateBackgroundPicture")]
    public async Task<IActionResult> UpdateBackgroundPicture([FromForm]string hash, [FromForm]string userAddress, [FromForm] string collectionAddress, [FromForm] IEnumerable<IFormFile> files)
    {
        if (!_signerHelper.VerifyHashFor(userAddress, hash)) return Unauthorized();

        long maxFileSize = 1024 * 1024 * 15;
        var file = files.Single();
        if (file.Length > maxFileSize)
            return BadRequest("File too big");

		if (!acceptedExtension.Contains(Path.GetExtension(file.FileName).ToLower()))
			return BadRequest();

		if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            await using var localFile = System.IO.File.Create($"/home/shared/images/{collectionAddress}/BackgroundPicture{Path.GetExtension(file.FileName)}");
            await file.CopyToAsync(localFile);
			localFile.Close();
		}
        else
        {
			var directoryPath = $"{AppContext.BaseDirectory}/{collectionAddress}";

			if (!Directory.Exists(directoryPath))
				Directory.CreateDirectory(directoryPath);
			await using var localFile = System.IO.File.Create($"{directoryPath}/BackgroundPicture{Path.GetExtension(file.FileName)}");
            await file.CopyToAsync(localFile);
			localFile.Close();
		}
        return Ok();
    }

    #endregion
}