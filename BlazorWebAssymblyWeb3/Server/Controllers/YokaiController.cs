using BlazorWebAssymblyWeb3.Client.Data;
using BlazorWebAssymblyWeb3.Server.Data;
using BlazorWebAssymblyWeb3.Server;
using BlazorWebAssymblyWeb3.Server.Services;
using BlazorWebAssymblyWeb3.Shared.Forms;
using Discord.Webhook;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nethereum.Signer;
using Nethereum.Signer.EIP712;
using Nethereum.Util;

namespace BlazorWebAssymblyWeb3.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class YokaiController : Controller
{
    private readonly YokaiToolsContext _context;
    private readonly SignerHelper _signerHelper;
    private readonly IHttpClientFactory _factory;

    public YokaiController(YokaiToolsContext pContext, SignerHelper pSignerhelper, IHttpClientFactory pFactory)
    {
        _context = pContext;
        _signerHelper = pSignerhelper;
        _factory = pFactory;
    }

    [HttpGet("SearchWithKeywords")]
    public async Task<IActionResult> SearchWithKeywords(string pKeywords)
    {

        if (pKeywords.Length < 3)
            return BadRequest();
        var filteredProfiles = await _context.Profiles.AsNoTracking()
            .Where(x => x.Name != null && x.Name.StartsWith(pKeywords))
            .Take(8)
            .Select(x => new Profile
            {
                Nft = new Nft
                {
                    Collection = new Collection
                    {
                        Address = x.Nft.Collection.Address
                    },
                    TokenId = x.Nft.TokenId
                },
                Name = x.Name,
                Address = x.Address,
            })
            .ToListAsync();

        var filteredCollections = await _context.Collections.AsNoTracking()
            .Where(x => x.Name.StartsWith(pKeywords))
			.Take(8)
			.Select(x => new Collection
            {
                Name = x.Name,
                Address = x.Address,
                
            })
            .ToListAsync();

        return Json(new SearchByKeywordsResult(filteredProfiles, filteredCollections));
    }

	[HttpGet("GetHoldersFromCollection")]
    public async Task<IActionResult> GetHoldersFromCollection(string pCollection)
    {
        using var client = _factory.CreateClient();
        var htmlResult = await client.GetAsync("https://ftmscan.com/token/" + pCollection);
        if (htmlResult.IsSuccessStatusCode)
        {
            var html = await htmlResult.Content.ReadAsStringAsync();
            //Console.WriteLine(html);
            return Ok(YokaiPatcher.GetHoldersFromHtml(html));
        }

        return Ok("0");
    }
    
    [HttpGet("GetProfileNameIfSetFor")]
    public async Task<IActionResult> GetProfileNameIfSetFor(string pAddress)
    {
        var profile = await _context.Profiles.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Address.ToLower() == pAddress.ToLower());

        if (profile is null) return Ok("");
        return Ok(profile.Name);
    }

    [HttpGet("GetTokenIDMintedFor")]
    public async Task<IActionResult> GetTokenIDMintedFor(string pCollection, int pSkip = 0, int pFetch = 0)
    {
        if (pFetch > 30 || pFetch == 0)
            pFetch = 30;

        var collectionId = await _context.Collections.Where(x => x.Address.ToLower() == pCollection.ToLower())
            .Select(x => x.Id).FirstOrDefaultAsync();

        var ids = await _context.Nfts
            .AsNoTracking().Where(x => x.CollectionId == collectionId).Select(x => x.TokenId).ToListAsync();

        return Json(ids);
    }
    
    [HttpGet("GetFiltersFor")]
    public async Task<IActionResult> GetFilters(int pCollectionId)
    {
        var attributesTypes = await _context.AttributesTypes.AsNoTracking().Include(x => x.Attributeoptions)
            .Where(x => x.CollectionId == pCollectionId).ToListAsync();

        var test = await _context.Attributes
            .AsNoTracking()
            .Include(x => x.AttributeTypeOption)
            .Where(x => x.CollectionId == pCollectionId)
            .GroupBy(x => x.AttributeTypeOptionId)
            .Select(x => new { x.Key, count = x.Count() })
            .ToListAsync();

        foreach (var x1 in test)
        {
            attributesTypes.SelectMany(x => x.Attributeoptions).First(x => x.Id == x1.Key).Count = x1.count;
        }

        return Json(attributesTypes);
    }

    [HttpGet("GetAllAddresses")]
    public async Task<IActionResult> GetAllAddresses()
    {
        return Json(await _context.Collections.AsNoTracking()
            .Select(x => new Collection { Address = x.Address, Name = x.Name }).ToListAsync());
    }
    
    [HttpGet("GetAllMinimalCollection")]
    public async Task<IActionResult> GetAllMinimalCollection()
    {
        var collections = await _context.Collections.AsNoTracking().Select(x => new Collection
        {
            Id = x.Id,
            Address = x.Address,
            Name = x.Name,
            ProfilePicture = x.ProfilePicture,
            ChainId = x.ChainId,
        }).ToListAsync();

        return Json(collections);
    }

    [HttpGet("GetWhitelistedCollection")]
    public async Task<IActionResult> GetWhitelistedCollection()
    {
        var whitelistedCollections = await _context.Collections.AsNoTracking()
            .Include(x => x.CollectionLinkCategories)
            .Include(x => x.OrderFulfilledHistories)
            .Include(x => x.OfferListeds)
            .Where(x => x.IsWhitelisted.HasValue && x.IsWhitelisted.Value || x.IsVerified).Select(x => new Collection
            {
                Address = x.Address,
                Name = x.Name,
                Id = x.Id,
                ProfilePicture = x.ProfilePicture,
                ChainId = x.ChainId,
                TotalSupply = x.TotalSupply,
                Finite = x.Finite,
                ReleaseDate = x.ReleaseDate,
                CollectionLinkCategories = x.CollectionLinkCategories,
                IsVerified = x.IsVerified,
                IsWhitelisted = x.IsWhitelisted,
                OrderFulfilledHistories = x.OrderFulfilledHistories,
                OfferListeds = x.OfferListeds
            }).ToListAsync();

        foreach(var whitelistedCollection in whitelistedCollections)
        {
            var sales = whitelistedCollection.OrderFulfilledHistories;
            var sales7D = whitelistedCollection.OrderFulfilledHistories.Where(x => x.Date > DateTime.UtcNow.AddDays(-7)).ToList();
            var sales24H = whitelistedCollection.OrderFulfilledHistories.Where(x => x.Date > DateTime.UtcNow.AddDays(-1)).ToList();

            var stats = new CollectionStats
            {
                Volume = sales.Sum(x => x.PriceInt),
                Volume7D = sales7D.Sum(x => x.PriceInt),
                Volume24H = sales24H.Sum(x => x.PriceInt),
                NumberOfSales7D = sales7D.Count,
                NumberOfSales24H = sales24H.Count,
                NumberOfSales = sales.Count,
                AveragePrice = Math.Round(whitelistedCollection.OfferListeds.Count > 0 ? whitelistedCollection.OfferListeds.Average(x => x.PriceInt) : 0),
                FloorPrice = whitelistedCollection.OfferListeds.Count > 0 ? whitelistedCollection.OfferListeds.Min(x => x.PriceInt) : 0
            };
            whitelistedCollection.Stats = stats;
        }

        return Json(whitelistedCollections);
    }

    [HttpGet("GetAssetStatsFor")]
    public async Task<IActionResult> GetAssetStatsFor(string pCollection, int pTokenId, string? pOwner = "", string? pConnectedAddress = "")
    {
        var collection = await _context.Collections.AsNoTracking()
            .Include(x => x.Nfts.Where(x => x.TokenId == pTokenId))
            .ThenInclude(x => x.Attributes)
            .ThenInclude(x => x.AttributeTypeOption)
            .ThenInclude(x => x.AttributeType)
			.Include(x => x.Nfts.Where(x => x.TokenId == pTokenId))
            .ThenInclude(x => x.OfferListeds)
			.FirstOrDefaultAsync(x => x.Address.ToLower() == pCollection.ToLower());
        
        if (collection is null)
            return NotFound();

        Profile? profile = null;
        if(!string.IsNullOrWhiteSpace(pOwner))
            profile = await _context.Profiles.AsNoTracking()
                .Include(x => x.Nft)
                .ThenInclude(x => x.Collection)
                .FirstOrDefaultAsync(x => x.Address.ToLower() == pOwner.ToLower());//x.CollectionId == collection.Id && x.TokenId == pTokenId;

        var score = new { Score = 0, Rank = 0 };
        if (collection.IsRarityAble)
        {
            score = await _context.Rarities.AsNoTracking().Where(x => x.TokenId == pTokenId && x.CollectionId == collection.Id)
                .Select(x => new { x.Score, x.Rank }).FirstOrDefaultAsync();
        }

        var favoriteCount = await _context.Favorites.CountAsync(x =>
            x.CollectionId == collection.Id && x.TokenId == pTokenId);

        var isFavoritedByUser = false;
        if (!string.IsNullOrWhiteSpace(pConnectedAddress))
            isFavoritedByUser = await _context.Favorites.AnyAsync(x =>
                x.CollectionId == collection.Id && x.TokenId == pTokenId &&
                x.WalletAddress == pConnectedAddress);

        var actualSupply = 0;
        if (collection.IsVerified || !(collection.Finite.GetValueOrDefault()))
            actualSupply = await _context.Nfts.CountAsync(x => x.CollectionId == collection.Id);

        AssetListing? listing = null;
        if (collection.IsVerified)
            listing = await _context.AssetListings.AsNoTracking().FirstOrDefaultAsync(x => x.ChainId == 250 && x.CollectionAddress.ToLower() == collection.Address && x.TokenId == pTokenId);

        var nft = collection.Nfts.FirstOrDefault();

		var stats = new AssetStats
        {
            FavoriteCount = favoriteCount,
            Score = score?.Score ?? 0,
            Rank = score?.Rank ?? 0,
            IsFavoritedByUser = isFavoritedByUser,
            CollectionPicture = collection.ProfilePicture,
            CollectionName = collection.Name,
            TotalSupply = collection.TotalSupply > 0 ? collection.TotalSupply : actualSupply,
            ActualSupply = actualSupply,
            Name = nft?.Name,
            ProfileName = profile?.Name,
            ProfilePictureCollection = profile?.Nft?.Collection?.Address,
            ProfilePictureTokenId = profile?.TokenId,
            IsVerified = collection.IsVerified,
            IsWhitelisted = collection.IsWhitelisted ?? false,
            CollectionId = collection.Id,
            ChainId = collection.ChainId,
            NftKeyAlias = collection.NftKeyAlias,
            AssetListing = listing,
            OffersListed = nft?.OfferListeds?.Where(x => x.ExpirationDate > DateTime.UtcNow).ToList()
		};

        if (collection.Nfts.Count > 0)
        {
            var countOfOption = await _context.Attributes
                .AsNoTracking()
                .Include(x => x.AttributeTypeOption)
                .Where(x => _context.Attributes.Where(y => y.TokenId == pTokenId && y.CollectionId == collection.Id)
                                .Select(y => y.AttributeTypeOptionId).Contains(x.AttributeTypeOptionId) &&
                            x.CollectionId == collection.Id)
                .GroupBy(x => x.AttributeTypeOptionId)
                .Select(x => new { x.Key, count = x.Count() })
                .Take(collection.Nfts.Single().Attributes.Count)
                .ToListAsync();
        

            foreach (var attributeoption in collection.Nfts.Single().Attributes.Select(x => x.AttributeTypeOption))
            {
                var count = 0;
                if (collection.TotalSupply > 0 || !(collection.Finite.GetValueOrDefault()))
                    count = countOfOption.First(x => x.Key == attributeoption.Id).count;
                stats.AssetAttributesScores.Add(new AssetAttributesScore
                {
                    AttributeTypeName = attributeoption.AttributeType.Name,
                    AttributeOptionName = attributeoption.OptionValue,
                    Score = (attributeoption.Score == 0
                        ? Math.Round(1 / ((double)count / collection.TotalSupply))
                        : attributeoption.Score),
                    ApparitionCount = count,
                    ShouldBeCounted = attributeoption.AttributeType.ScoreNoCount ?? false
                });
            }
        }
        return Json(stats);
    }

    [HttpGet("GetCollectionConfig")]
    public async Task<IActionResult> IsWhiteListed(string pAddress, bool? pWithUserFavorite = false, string? pWalletAddress = "")
    {
        var collectionData = await _context.Collections.AsNoTracking()
            .Include(x => x.CollectionLinkCategories)
            .ThenInclude(x => x.Category)
            .Include(x => x.FavoritedCollections)
            .FirstOrDefaultAsync(x => x.Address.ToLower() == pAddress.ToLower());
        
        if (collectionData is null)
            return NotFound();

        if(collectionData.IsVerified)
            collectionData.ActualSupply = await _context.Nfts.CountAsync(x => x.CollectionId == collectionData.Id);
        
        // if (pWithUserFavorite)
        // {
        //     if (string.IsNullOrWhiteSpace(pWalletAddress))
        //         return BadRequest("WithUserFavorite need to have a wallet address set");
        //     collectionData.Favorites = await _context.Favorites.AsNoTracking().Where(x => x.CollectionId == collectionData.Id && x.WalletAddress.ToLower() == pWalletAddress.ToLower()).Select(x => x.TokenId).ToListAsync();
        // }
        
        return Json(collectionData);
    }

    [HttpGet("GetRankFor")]
    public async Task<IActionResult> GetRankFor(int pCollectionId, string pMode = "Score")
    {
        List<AssetStats> stat;

        if (pMode.Equals("Score", StringComparison.InvariantCultureIgnoreCase))
        {
			stat = await _context.Rarities.Include(x => x.Nft).AsNoTracking().Where(x => x.CollectionId == pCollectionId)
                .OrderByDescending(x => x.Score)
                .Select(x => new AssetStats
                {
                    Name = x.Nft.Name,
                    TokenId = x.TokenId,
					Rank = x.Rank,
				}).ToListAsync();
        }
        else
        {
			stat = await _context.Rarities.Include(x => x.Nft).AsNoTracking().Where(x => x.CollectionId == pCollectionId)
                .OrderByDescending(x => x.CustomScore)
			   .Select(x => new AssetStats
			   {
				   Name = x.Nft.Name,
				   TokenId = x.TokenId,
				   Rank = x.Rank,
			   }).ToListAsync();
        }

        return Json(stat);
    }

    public record struct FilterPayload(Dictionary<string, string> pFilters);

    [HttpPost("Filter")]
    public async Task<IActionResult> Filter(int pCollectionId, [FromBody] FilterPayload pPayload, string? pMode = "")
    {
        var pred = PredicateBuilder.False<Nft>();
        var keyValuePair = pPayload.pFilters.First();
        foreach (var s in keyValuePair.Value.Split(','))
        {
            pred = pred.Or(x =>
                x.Attributes.Any(
                    y => y.AttributeType.Name == keyValuePair.Key && y.AttributeTypeOption.OptionValue == s));
        }

        var types = _context.Nfts.Include(x => x.Attributes).ThenInclude(x => x.AttributeType)
            .Include(x => x.Attributes).ThenInclude(x => x.AttributeTypeOption)
            .Where(pred)
            .Where(x => x.CollectionId == pCollectionId)
            .AsNoTracking();


        foreach (var pPayloadPFilter in pPayload.pFilters.Skip(1))
        {
            pred = PredicateBuilder.False<Nft>();

            foreach (var s in pPayloadPFilter.Value.Split(','))
            {
                pred = pred.Or(x => x.Attributes.Any(y =>
                    y.AttributeType.Name == pPayloadPFilter.Key && y.AttributeTypeOption.OptionValue == s));
            }

            types = types.Intersect(_context.Nfts.Include(x => x.Attributes).ThenInclude(x => x.AttributeType)
                .Include(x => x.Attributes).ThenInclude(x => x.AttributeTypeOption).Where(pred)
                .Where(x => x.CollectionId == pCollectionId));
        }

        switch (pMode?.ToLower())
        {
            case "score":
                return Json(await types.Select(x => new
                {
                    x.TokenId,
                    x.Rarity.Rank
                }).OrderBy(x => x.Rank).ToListAsync());
                break;
            case "custom":
                return Json(await types.Select(x => new
                {
                    x.TokenId,
                    x.Rarity.CustomRank
                }).OrderBy(x => x.CustomRank).ToListAsync());
                break;
            default:
                return Json(await types.Select(x => new
                {
                    x.TokenId
                }).OrderBy(x => x.TokenId).ToListAsync());
                break;
        }
    }

    [HttpGet("GetGuidSignFor")]
    public IActionResult GetGuidSignFor(string pAddress)
    {
        var guid = Guid.NewGuid();
        _signerHelper.AddOrReplaceToSign(pAddress, guid);
        return Ok(guid.ToString());
    }

    public record struct ListedCollectionPayload(ListingData pListingData);

    [HttpPost("AddListedCollection")]
    public async Task<IActionResult> AddListedCollection(string pHash, string pAddress, [FromBody] ListedCollectionPayload pPayload)
    {
        if (!_signerHelper.TryGetGuidFor(pAddress, out Guid guid))
            return NotFound();

        var typedData = SignerHelper.Default(guid.ToString());

        var address = new MessageSigner().EcRecover(
            Sha3Keccack.Current.CalculateHash(Eip712TypedDataSigner.Current.EncodeTypedData(typedData)), pHash);
        if (address.ToLower() != pAddress.ToLower())
            return BadRequest();

        var listingData = pPayload.pListingData;

        if (!listingData.Validate())
            return BadRequest("Form lack crucial informations, please use the website form");

        var collection = await _context.Collections.FirstOrDefaultAsync(x =>
            x.Address.ToLower() == listingData.ContractAddress!.ToLower() && x.ChainId == listingData.Blockchain);

        if (collection != null)
            return BadRequest("Collection has already been verified");

        var toListCollection = await _context.ToVerifyCollections.FirstOrDefaultAsync(x =>
            x.Address.ToLower() == listingData.ContractAddress!.ToLower() && x.ChainId == listingData.Blockchain);

        if (toListCollection != null)
            return BadRequest("Collection is already being verified");

        //var count = await _context.ToVerifyCollections.CountAsync(x => x.)
        //Wallet address owner check
        var totalSupplyParse = int.TryParse(listingData.TotalSupply, out int totalSupply);
        if (!totalSupplyParse)
            return BadRequest("Total supply is not a number");

        _context.ToVerifyCollections.Add(new ToVerifyCollection
        {
            Address = listingData.ContractAddress!,
            Name = listingData.ProjectName!,
            ChainId = 250,
            IsMintable = listingData.MintStatut == "Minting",
            MintLink = listingData.SocialLinks.Website,
            TotalSupply = totalSupply,
            Finite = totalSupply != 0,
            IsSoldOut = listingData.MintStatut == "Soldout",
            Banner = "",
            ProfilePicture = "",
            IsRarityAble = listingData.IsRarityAble,
            AddressOfLister = pAddress,
            Keywords = listingData.Keywords,
            MintPrice = listingData.MintPrice,
            ReleaseDate = listingData.ReleaseDate,
            Twitter = listingData.SocialLinks?.Twitter
            
        });
        await _context.SaveChangesAsync();

        //var webhook = new DiscordWebhookClient("");
        //await webhook.SendMessageAsync($"New listing => {listingData.ProjectName} twitter => https://twitter.com/{listingData.SocialLinks?.Twitter}");
        //

        return Ok();
    }

    public record struct EditCollectionPayload(string pAddress, string pHash, CollectionEdit pEditInfo);

    [HttpPost("EditCollectionIfValid")]
    public async Task<IActionResult> EditCollectionIfValid([FromBody] EditCollectionPayload pPayload)
    {
        if (!_signerHelper.TryGetGuidFor(pPayload.pAddress, out Guid guid))
            return NotFound();

        var collection = await _context.Collections.FirstOrDefaultAsync(x => x.Id == pPayload.pEditInfo.Id);
        
        var typedData = SignerHelper.Default(guid.ToString());
        var address = new MessageSigner().EcRecover(
            Sha3Keccack.Current.CalculateHash(Eip712TypedDataSigner.Current.EncodeTypedData(typedData)),
            pPayload.pHash);
        if (address.ToLower() != pPayload.pAddress.ToLower()) return BadRequest();

        if (collection == null || !(collection.OwnerAddress?.Equals(address, StringComparison.InvariantCultureIgnoreCase) ?? false))
            return BadRequest();

        var editInfo = pPayload.pEditInfo;
        collection.Description = editInfo.Description;
        collection.Discord = editInfo.Discord;
        collection.Website = editInfo.Website;
        collection.Name = editInfo.Name ?? "";
        collection.ProfilePicture = editInfo.ProfileImage;
        collection.Banner = editInfo.BackgroundImage;
        collection.Royalty = editInfo.Royalty;

        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpGet("GetAssetName")]
    public async Task<IActionResult> GetAssetName(int pCollectionId, int pTokenId)
    {
        var assetsName = await _context.Nfts.AsNoTracking().Where(x => x.CollectionId == pCollectionId && pTokenId == x.TokenId).Select(x => x.Name).FirstOrDefaultAsync();
        if (assetsName is null) return BadRequest();
        return Ok(assetsName);
    }
    
    [HttpGet("GetSwapOffersForUser")]
    public async Task<IActionResult> GetSwapOffersForUser(string pUserAddress)
    {
        var timestampOffsetNow = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        
        var swapOffers = await _context.SwapOffers.AsNoTracking()
            .Where(x => (x.Offerer.ToLower() == pUserAddress.ToLower() || x.OwnerOfTargeted.ToLower() == pUserAddress.ToLower()) && x.EndTimestamp > timestampOffsetNow.TotalSeconds)
            .OrderBy(x => x.EndTimestamp)
            .ToListAsync();
        
        var offererAddresses = swapOffers.Where(x => !x.Offerer.Equals(pUserAddress, StringComparison.CurrentCultureIgnoreCase))
            .Select(x => x.Offerer);
        var targetedAddresses = swapOffers.Where(x => !x.OwnerOfTargeted.Equals(pUserAddress, StringComparison.CurrentCultureIgnoreCase))
            .Select(x => x.OwnerOfTargeted);

        var addresses = offererAddresses.Union(targetedAddresses).ToList();
        
        var profiles = await _context.Profiles.Where(x => addresses.Contains(x.Address.ToLower())).ToListAsync();
        
        foreach (var swapOffer in swapOffers)
        {
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(swapOffer.EndTimestamp);
            swapOffer.TimeUntilExpire = dateTime - DateTime.UtcNow;
            swapOffer.OffererNickname = profiles.FirstOrDefault(x => x.Address.ToLower() == swapOffer.Offerer.ToLower())?.Name ?? "";
            swapOffer.TargetOwnerNickname = profiles.FirstOrDefault(x => x.Address.ToLower() == swapOffer.OwnerOfTargeted.ToLower())?.Name ?? "";
        }

        return Json(swapOffers);
    }

    [HttpGet("GetSwapOffersForAsset")]
    public async Task<IActionResult> GetSwapOffersFor(string pCollectionAddress, int pTokenId)
    {
        var collectionAddress = pCollectionAddress.ToLower();
        var swapOffers = await _context.SwapOffers.AsNoTracking().Where(x =>
            // (x.OfferCollection.ToLower() == collectionAddress && x.OfferTokenId == pTokenId) ||
            (x.TargetCollection.ToLower() == collectionAddress && x.TargetTokenId == pTokenId)).ToListAsync();

        var addresses = swapOffers.Select(x => x.Offerer).Distinct();

        var nicknames = await _context.Profiles.AsNoTracking().Where(x => addresses.Contains(x.Address.ToLower()))
            .ToListAsync();

        //Dictionary<string, string> nicknames = new();
        foreach (var swapOffer in swapOffers)
        {
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(swapOffer.EndTimestamp);
            swapOffer.TimeUntilExpire = dateTime - DateTime.UtcNow;

            var profile = nicknames.FirstOrDefault(x =>
                x.Address.Equals(swapOffer.Offerer, StringComparison.InvariantCultureIgnoreCase));
            if (profile is null) continue;
            swapOffer.OffererNickname = profile.Name;
        }

        return Json(swapOffers);
    }

    [HttpGet("Trendings")]
    public async Task<IActionResult> Trendings()
    {
        return Json(await _context.Collections.Take(2).Where(x => x.Id == 4 || x.Id == 5).ToListAsync());
    }

    [HttpGet("FavoriteAsset")]
    public async Task<IActionResult> FavoriteAsset(int pCollectionId, int pTokenId, string pSignatureHash, string pAddress)
    {
        if (!_signerHelper.TryGetGuidFor(pAddress, out Guid guid))
            return NotFound();

        var typedData = SignerHelper.Default(guid.ToString());

        var address = new MessageSigner().EcRecover(
            Sha3Keccack.Current.CalculateHash(Eip712TypedDataSigner.Current.EncodeTypedData(typedData)),
            pSignatureHash);
        if (address.ToLower() != pAddress.ToLower()) return BadRequest();

        var favorite = await _context.Favorites.AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.CollectionId == pCollectionId &&
                x.WalletAddress.ToLower() == pAddress.ToLower() && x.TokenId == pTokenId);

        if (favorite is null)
        {
            favorite = new Favorite
            {
                WalletAddress = pAddress,
                CollectionId = pCollectionId,
                TokenId = pTokenId,
                Since = DateTime.UtcNow,
            };
            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();
        }

        return Ok();
    }

    // const int TOFETCH = 25;
    // [HttpGet("GetLeaderboardsYokai")]
    // public async Task<IActionResult> GetLeaderboardsYokai(int page)
    // {
    //     var toSkip = (page-1) * TOFETCH;
    //
    //     var leaders = await _context.Yokai
    //         .FromSqlRaw(
    //             $"select * from (select *, RANK() over(order by Score) as rang from Yokai) as u order by Score LIMIT {toSkip},{TOFETCH}")
    //         .ToListAsync();
    //
    //     return Json(leaders);
    // }
}