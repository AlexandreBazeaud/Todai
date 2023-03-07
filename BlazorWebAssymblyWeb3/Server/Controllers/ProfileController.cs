using BlazorWebAssymblyWeb3.Server.Data;
using BlazorWebAssymblyWeb3.Server.Services;
using BlazorWebAssymblyWeb3.Shared;
using BlazorWebAssymblyWeb3.Shared.Forms;
using Discord;
using Discord.Rest;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nethereum.Signer;
using Nethereum.Signer.EIP712;
using Nethereum.Util;
using Nethereum.Web3;
using Newtonsoft.Json;


namespace BlazorWebAssymblyWeb3.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class ProfileController : Controller
{
	private readonly YokaiToolsContext _context;
	private readonly SignerHelper _signerHelper;
	private readonly Web3 _web3;
	private readonly ILogger<ProfileController> _logger;
	private readonly IHttpClientFactory _factory;

	public ProfileController(YokaiToolsContext pContext, SignerHelper pSignerHelper, ILogger<ProfileController> pLogger, IHttpClientFactory pHttpClientFactory)
	{
		_context = pContext;
		_web3 = new Web3("https://rpc.ftm.tools/");
		_signerHelper = pSignerHelper;
		_logger = pLogger;
		_factory = pHttpClientFactory;
	}

	#region GET

	[HttpGet("GetListingsOf")]
	public async Task<IActionResult> GetListingsOf(string pUserAddress)
	{
		var now = DateTime.UtcNow;
		var listings = await _context.AssetListings.AsNoTracking()
			.Where(x => x.Offerer.ToLower() == pUserAddress.ToLower() && x.ExpirationDate > now)
			.ToListAsync();

		return Json(listings);
	}

	[HttpGet("GetProfilesCount")]
	public async Task<IActionResult> GetCollections(string? pFilter = "")
	{
		var count = await _context.Profiles
			.Where(x => x.Name != null && (string.IsNullOrWhiteSpace(pFilter) || x.Name.Contains(pFilter)))
			.AsNoTracking().CountAsync();
		return Ok(count);
	}

	[HttpGet("GetProfiles")]
	public async Task<IActionResult> GetProfiles(int pSkip = 0, int pFetch = 0, string? pFilter = "")
	{
		if (pFetch is > 25 or <= 0)
			pFetch = 25;

		var count = await _context.Profiles
			.Where(x => x.Name != null && (string.IsNullOrWhiteSpace(pFilter) || x.Name.Contains(pFilter)))
			.AsNoTracking().CountAsync();

		var profiles = await _context.Profiles
			.Include(x => x.Nft)
			.ThenInclude(x => x.Collection)
			.Where(x => x.Name != null && (string.IsNullOrWhiteSpace(pFilter) || x.Name.Contains(pFilter))).Skip(pSkip)
			.Take(pFetch)
			.AsNoTracking().Select(x => new Profile
			{
				Address = x.Address,
				Name = x.Name,
				Id = x.Id,
				Bio = x.Bio,
				TokenId = x.TokenId,
				Nft = new Nft
				{
					Collection = new Collection
					{
						Address = x.Nft.Collection.Address
					}
				}
			}).ToListAsync();

		return Json(new Explorer.ExplorerUsersPayload(profiles, count));
	}

	[HttpGet("GetFavoritesOf")]
	public async Task<IActionResult> GetFavoritesOf(string pUserAddress)
	{
		var favorites = await _context.Favorites.AsNoTracking()
			.Include(x => x.Nft)
			.ThenInclude(x => x.Collection)
			.Where(x => x.WalletAddress.ToLower() == pUserAddress.ToLower())
			.Select(x => new Favorite
			{
				CollectionId = x.CollectionId,
				TokenId = x.TokenId,
				Nft = new Nft
				{
					Collection = new Collection
					{
						Address = x.Nft.Collection.Address
					},
				}
			})
			.ToListAsync();

		return Json(favorites);
	}

	[HttpGet("GetNotifcations")]
	public async Task<IActionResult> GetNotificationsFor(string address)
	{
		var notifs = await _context.Notifications.AsNoTracking().Include(x => x.Event).Where(x => x.Address.ToLower() == address.ToLower()).ToListAsync();

		return Json(notifs);
	}

	[HttpDelete("ClearAllNotifications")]
	public async Task<IActionResult> ClearAllNotifications(string address)
	{
		var notifs = await _context.Notifications.Where(x => x.Address.ToLower() == address.ToLower()).ToListAsync();
		_context.Notifications.RemoveRange(notifs);
		await _context.SaveChangesAsync();
		return Ok();
	}

	[HttpPatch("ReadNotification")]
	public async Task<IActionResult> ReadNotification([FromQuery]string address, [FromQuery] uint notificationId)
	{
		var notif = await _context.Notifications.FirstOrDefaultAsync(x => x.Address.ToLower() == address.ToLower() && x.Id == notificationId);
		if (notif is null) return NotFound();
		notif.Read = true;
		await _context.SaveChangesAsync();
		return Ok();
	}

	[HttpPost("GetAllNftsFrom")]
	public async Task<IActionResult> GetAllNftsFrom(GetAllNftsFromPayload payload)
	{
		//var client = _factory.CreateClient("moralis");
		
		//var nftsTask = client.GetStringAsync($"https://deep-index.moralis.io/api/v2/{payload.pUserAddress}/nft?chain=fantom&format=decimal");

		var pred = PredicateBuilder.False<Nft>();
		var query = _context.Nfts
			.Include(x => x.Favorites.Where(y => y.WalletAddress == payload.pUserAddress))
			.Include(x => x.Rarity)
            .Include(x => x.Attributes)
            .ThenInclude(x => x.AttributeTypeOption)
            .Include(x => x.OfferListeds)
			.Include(x => x.Collection)
			.AsNoTracking();
		foreach (var id in payload.userOwnedNFt)
		{
			pred = pred.Or(x => x.Collection.Address == id.Key && id.Value.Contains(x.TokenId));
		}

		var nftsInfos = await query.Where(pred).Select(x =>
			new Nft
			{
				Collection = new Collection
				{
					Address = x.Collection.Address,
					Id = x.Collection.Id,
					Name = x.Collection.Name
				},
				Attributes = x.Attributes,
				Name = x.Name,
				TokenId = x.TokenId,
				Rarity = x.Rarity,
				CollectionId = x.CollectionId,
				Favorites = x.Favorites,
				OfferListeds = x.OfferListeds.Select(y => new OfferListed
				{
					Price = y.Price,
				}).ToList(),
			}

		).ToListAsync();
		//var a = nftsInfos.ToQueryString();

		MoralisNftFetchPayload data = null;
		//var data = JsonConvert.DeserializeObject<MoralisNftFetchPayload>(await nftsTask);
		return Json(new GetAllNftsFromResponse(nftsInfos, data?.total ?? 0));
	}

	[HttpGet("GetProfileOf")]
	public async Task<IActionResult> GetProfileOf(string pAddress)
	{
		if (pAddress.Length != 42 || !pAddress.StartsWith("0x"))
			return BadRequest();

		var profile = await _context.Profiles.AsNoTracking()
			.Include(x => x.Nft)
			.ThenInclude(x => x.Collection)
			.Include(x => x.ProfileIdFolloweds.Where(y => y.Name != null))
			.ThenInclude(x => x.Nft)
			.ThenInclude(x => x.Collection)
			.Include(x => x.ProfileIdFollowings.Where(y => y.Name != null))
			.ThenInclude(x => x.Nft)
			.ThenInclude(x => x.Collection)
			.Where(x => x.Address.ToLower().Equals(pAddress.ToLower()))
			.FirstOrDefaultAsync();

		if (profile is null)
		{
			profile = new Profile
			{
				Address = pAddress,
				Name = null,
				Level = 0
				
			};
			_context.Profiles.Add(profile);
			await _context.SaveChangesAsync();
		}

		return Json(profile);
	}


	#endregion

	#region POST
	public record struct FollowPayload(string pAddress, string pHash, string pAddressToFollow);

	[HttpPost("Follow")]
	public async Task<IActionResult> Follow([FromBody] FollowPayload pPayload)
	{
		if (pPayload.pAddressToFollow.ToLower() == pPayload.pAddress.ToLower())
			return BadRequest("Cannot follow yourself");

		if (!_signerHelper.TryGetGuidFor(pPayload.pAddress, out var guid))
			return NotFound();

		var typedData = SignerHelper.Default(guid.ToString());
		var address = new MessageSigner().EcRecover(
			Sha3Keccack.Current.CalculateHash(Eip712TypedDataSigner.Current.EncodeTypedData(typedData)),
			pPayload.pHash);
		if (address.ToLower() != pPayload.pAddress.ToLower()) return BadRequest();

		var profile = await _context.Profiles.FirstOrDefaultAsync(x => x.Address.ToLower() == pPayload.pAddress.ToLower());
		if (profile is null)
			return BadRequest();

		var profileToFollow = await _context.Profiles.FirstOrDefaultAsync(x => x.Address.ToLower() == pPayload.pAddressToFollow);
		if (profileToFollow is null)
			return BadRequest();

		_context.Notifications.Add(new Notification
		{
			Address = profileToFollow.Address,
			Data = profile.Name ?? profile.Address,//{0} is now watching you!
			EventId = 6,
			Read = false,
			Date = DateTime.UtcNow
		});

		profile.ProfileIdFolloweds.Add(profileToFollow);
		await _context.SaveChangesAsync();
		return Ok();
	}

	[HttpPost("Unfollow")]
	public async Task<IActionResult> Unfollow([FromBody] FollowPayload pPayload)
	{
		if (pPayload.pAddressToFollow.ToLower() == pPayload.pAddress.ToLower())
			return BadRequest("Cannot unfollow yourself");

		if (!_signerHelper.TryGetGuidFor(pPayload.pAddress, out var guid))
			return NotFound();

		var typedData = SignerHelper.Default(guid.ToString());
		var address = new MessageSigner().EcRecover(
			Sha3Keccack.Current.CalculateHash(Eip712TypedDataSigner.Current.EncodeTypedData(typedData)),
			pPayload.pHash);
		if (address.ToLower() != pPayload.pAddress.ToLower()) return BadRequest();

		var profile = await _context.Profiles.Include(x => x.ProfileIdFolloweds).FirstOrDefaultAsync(x => x.Address.ToLower() == pPayload.pAddress.ToLower());
		if (profile is null)
			return BadRequest();

		var profileToUnfollow = await _context.Profiles.FirstOrDefaultAsync(x => x.Address.ToLower() == pPayload.pAddressToFollow);
		if (profileToUnfollow is null)
			return BadRequest();

		if (profile.ProfileIdFolloweds.All(x => x.Id != profileToUnfollow.Id))
			return BadRequest();

		profile.ProfileIdFolloweds.Remove(profileToUnfollow);
		await _context.SaveChangesAsync();
		return Ok();
	}

	public record struct EditProfilePayload(string pAddress, string pHash, ProfileEdit pEditInfo);

	[HttpPost("EditProfile")]
	public async Task<IActionResult> EditProfile([FromBody] EditProfilePayload pPayload)
	{
		if (!_signerHelper.TryGetGuidFor(pPayload.pAddress, out var guid))
			return NotFound();

		var typedData = SignerHelper.Default(guid.ToString());
		var address = new MessageSigner().EcRecover(
			Sha3Keccack.Current.CalculateHash(Eip712TypedDataSigner.Current.EncodeTypedData(typedData)),
			pPayload.pHash);
		if (address.ToLower() != pPayload.pAddress.ToLower()) return BadRequest();

		var profile =
			await _context.Profiles.FirstOrDefaultAsync(x => x.Address.ToLower() == pPayload.pAddress.ToLower());
		if (profile is null)
			return BadRequest();

		var editInfo = pPayload.pEditInfo;
		profile.Activity = editInfo.Activity;
		profile.Bio = editInfo.Bio;
		profile.Localisation = editInfo.Localisation;
		profile.InstagramNickname = editInfo.InstagramNickname;
		profile.TwitterHandle = editInfo.TwitterHandle;
		profile.Link = editInfo.Link;
		profile.Gender = editInfo.Gender;

		await _context.SaveChangesAsync();
		return Ok();
	}
	#endregion

	[HttpPatch("UpdateUsername")]
	public async Task<IActionResult> UpdateUsername(string pHash, string pAddress, string pUsername)
	{
		if (!_signerHelper.TryGetGuidFor(pAddress, out Guid guid))
			return NotFound();

		var typedData = SignerHelper.Default(guid.ToString());

		var address = new MessageSigner().EcRecover(
			Sha3Keccack.Current.CalculateHash(Eip712TypedDataSigner.Current.EncodeTypedData(typedData)), pHash);
		if (address.ToLower() != pAddress.ToLower())
			return Unauthorized();

		var profile = await _context.Profiles.FirstOrDefaultAsync(x => x.Address.ToLower() == pAddress.ToLower());
		if (profile is null) return BadRequest();
		profile.Name = pUsername;
		profile.DateOfNickname ??= DateTime.UtcNow;

		await _context.SaveChangesAsync();
		return Ok();
	}

	public record struct DiscordLinkPayload(string pCode, string pAddress, string pHash);
	[HttpPost("LinkDiscordToProfile")]
	public async Task<IActionResult> LinkDiscordToProfile(DiscordLinkPayload pPayload)
	{
		if (!_signerHelper.TryGetGuidFor(pPayload.pAddress, out Guid guid))
			return NotFound();

		var typedData = SignerHelper.Default(guid.ToString());

		var address = new MessageSigner().EcRecover(
			Sha3Keccack.Current.CalculateHash(Eip712TypedDataSigner.Current.EncodeTypedData(typedData)), pPayload.pHash);
		if (address.ToLower() != pPayload.pAddress.ToLower())
			return Unauthorized();


		var dict = new Dictionary<string, string>
		{
			{ "client_id", "923646316419088425" },
			{ "client_secret", Helper.DiscordLoginSecret },
			{ "grant_type", "authorization_code" },
			{ "code", pPayload.pCode },
			{ "redirect_uri", "https://localhost:7103/discordlink"},
			{ "scope", "identify"}
		};

		var uri = new Uri("https://discordapp.com/api/");
		using var client = new HttpClient();

		client.BaseAddress = uri;
		//on demande a discord de nous donner son code
		var t = await client.PostAsync("oauth2/token", new FormUrlEncodedContent(dict));
		if (!t.IsSuccessStatusCode)
			_logger.LogCritical("status code not ok => {0}", t.StatusCode);
		var data = await t.Content.ReadAsStringAsync();
		_logger.LogWarning(data);
		var reponse = JsonConvert.DeserializeObject<DiscordTokenResponse>(data);

		//puis on fait une demande de son id
		var bearerClient = new DiscordRestClient();
		await bearerClient.LoginAsync(TokenType.Bearer, reponse.access_token);

		if (bearerClient.LoginState != LoginState.LoggedIn) return BadRequest("Server error");

		var profile = await _context.Profiles.FirstOrDefaultAsync(x => x.Address.ToLower() == pPayload.pAddress.ToLower());
		if (profile is null) return BadRequest();
		profile.DiscordId = (long)bearerClient.CurrentUser.Id;
		await _context.SaveChangesAsync();
		_logger.LogInformation("Client added for {UserId}-{UserName}", bearerClient.CurrentUser.Id, bearerClient.CurrentUser.Username);

		return Ok(bearerClient.CurrentUser.Username);
	}

	[HttpGet("SetNFTProfilePicture")]
	public async Task<IActionResult> SetNFTProfilePicture(string pAddress, string pHash, string pCollectionAddress, uint pTokenId)
	{
		if (!_signerHelper.TryGetGuidFor(pAddress, out Guid guid))
			return Unauthorized();

		var typedData = SignerHelper.Default(guid.ToString());

		var address = new MessageSigner().EcRecover(
			Sha3Keccack.Current.CalculateHash(Eip712TypedDataSigner.Current.EncodeTypedData(typedData)), pHash);
		if (address.ToLower() != pAddress.ToLower())
			return BadRequest();

		var contract = _web3.Eth.GetContract(Helper.ERC721Abi, pCollectionAddress);
		var function = contract.GetFunction("ownerOf");
		var owner = await function.CallAsync<string>(pTokenId);

		if (owner.ToLower() != pAddress.ToLower())
			return BadRequest();

		var profile = await _context.Profiles.FirstOrDefaultAsync(x => x.Address.ToLower() == pAddress.ToLower());
		var collection = await _context.Collections.AsNoTracking()
			.Include(x => x.Nfts.Where(y => y.TokenId == pTokenId))
			.FirstOrDefaultAsync(x => x.Address.ToLower() == pCollectionAddress.ToLower() && x.IsVerified);

		if (profile is null || collection is null || collection.Nfts.Count == 0) return NotFound();

		var nft = collection.Nfts.Single();
		profile.CollectionId = nft.CollectionId;
		profile.TokenId = nft.TokenId;
		await _context.SaveChangesAsync();

		return Ok();
	}
}