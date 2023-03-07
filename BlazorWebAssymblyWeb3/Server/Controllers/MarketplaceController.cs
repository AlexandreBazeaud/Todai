
using BlazorWebAssymblyWeb3.Server.Data;
using BlazorWebAssymblyWeb3.Server.Services;
using BlazorWebAssymblyWeb3.Shared.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nethereum.Signer;
using Nethereum.Signer.EIP712;
using Nethereum.Util;
using Newtonsoft.Json;
using Nethereum.Hex;
using Nethereum.Hex.HexConvertors.Extensions;
using System.Numerics;
using Nethereum.Web3;

namespace BlazorWebAssymblyWeb3.Server.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class MarketplaceController : Controller
	{

		private readonly YokaiToolsContext _context;
		private readonly MarketplaceService _marketplaceService;
		private readonly SignerHelper _signerHelper;
		private readonly Web3 _web3;
		//private readonly PaintswapService
		//private readonly Eip712TypedDataSigner _eip712TypedDataSigner = new Eip712TypedDataSigner();

		public MarketplaceController(YokaiToolsContext pContext, SignerHelper pSignerhelper, MarketplaceService pMarketplaceService)
		{
			_context = pContext;
			_signerHelper = pSignerhelper;
			_marketplaceService = pMarketplaceService;
			_web3 = new Web3("https://rpc.ftm.tools/");
		}

		#region orderValidation
		[HttpPost("ValidateOrder")]
		public async Task<IActionResult> ValidateOrder(SeaportOrder pOrder)
		{
			var hash = await _marketplaceService.GetAndVerifyOrderHash(pOrder);
			var orderHash = BitConverter.ToString(Helper.HashToBytes(pOrder.orderHash));

			//var test = _eip712TypedDataSigner.RecoverFromSignatureV4(pOrder.parameters, SignerHelper.MarketplaceOrderDefault(), pOrder.signature);

			if (orderHash != hash) return BadRequest("Wrong hash");

			var status = await _marketplaceService.GetOrderStatusAsync(pOrder.orderHash);

			if (!status.isValidated) return BadRequest("Not validated");

			var isOrderExisting = await _context.AssetListings.AsNoTracking().AnyAsync(x => x.OrderHash.ToLower() == pOrder.orderHash);

			if (isOrderExisting) return BadRequest("Order already exist");
			if (pOrder.parameters.offer.Count != 1) return BadRequest("Invalid order");
			if (pOrder.parameters.consideration.Count != 2 && pOrder.parameters.consideration.Count != 3) return BadRequest("Invalid order");

			var collectionData = await _context.Collections.AsNoTracking().Where(x => x.Address == pOrder.parameters.offer.Single().token).Select(x => new Collection(){
				IsVerified = x.IsVerified,
				OwnerAddress = x.OwnerAddress,
				Royalty = x.Royalty
			}).FirstOrDefaultAsync();

			if (collectionData is null || !collectionData.IsVerified) return BadRequest("Collection is not verified");

			if(collectionData.OwnerAddress != null && collectionData.Royalty > 0 && !pOrder.parameters.consideration.Any(x => x.recipient == collectionData.OwnerAddress))
				return BadRequest("Invalid order");

			decimal priceDcm = 0;
			BigInteger userPart = 0;
			BigInteger ownerPart = 0;
			BigInteger royaltyPart = 0;
			foreach(var consideration in pOrder.parameters.consideration)
            {
				string price = consideration.startAmount;
				if (price.HasHexPrefix())
					price = price.RemoveHexPrefix();

				var priceBI = BigInteger.Parse($"0{price}", System.Globalization.NumberStyles.AllowHexSpecifier);

				if (priceDcm == 0)
					userPart = priceBI;
				else if (consideration.recipient == collectionData.OwnerAddress)
					royaltyPart = priceBI;
				else  // if not royalty part
					ownerPart = priceBI;

				priceDcm += Web3.Convert.FromWei(priceBI);
			}

			var diff = (Math.Exp(BigInteger.Log(ownerPart) - BigInteger.Log(ownerPart + userPart + royaltyPart)) * 100) - 2;//percent fee
			if (diff >= 0.01)
				return BadRequest("Invalid order");

			if (collectionData.OwnerAddress != null && collectionData.Royalty > 0)
			{
				var diffRoyalty = (Math.Exp(BigInteger.Log(royaltyPart) - BigInteger.Log(ownerPart + userPart + royaltyPart)) * 100) - collectionData.Royalty;//percent royalty
				if (diff >= 0.01)
					return BadRequest("Invalid order");
			}

			var listing = new AssetListing
			{
				ChainId = 250,
				OrderHash = pOrder.orderHash,
				CollectionAddress = pOrder.parameters.offer.Single().token,
				Price = priceDcm.ToString("G29"),
				Signature = pOrder.signature,
				TokenId = pOrder.parameters.offer.Single().identifierOrCriteria,
				OrderJson = JsonConvert.SerializeObject(pOrder.parameters),
				CreationDate = DateTime.UtcNow,
				ExpirationDate = DateTimeOffset.FromUnixTimeSeconds((long)pOrder.parameters.endTime).DateTime,
				Offerer = pOrder.parameters.offerer
			};
			

			_context.AssetListings.Add(listing);

			await _context.SaveChangesAsync();
			
			return Json(listing);
		}

		[HttpPost("ValidateOrderOffer")]
		public async Task<IActionResult> ValidateOrderOffer(SeaportOrder pOrder)
		{
			var hash = await _marketplaceService.GetAndVerifyOrderHash(pOrder);
			var orderHash = BitConverter.ToString(Helper.HashToBytes(pOrder.orderHash));

			//var test = _eip712TypedDataSigner.RecoverFromSignatureV4(pOrder.parameters, SignerHelper.MarketplaceOrderDefault(), pOrder.signature);
			
			if (orderHash != hash) return BadRequest("Wrong hash");

			var status = await _marketplaceService.GetOrderStatusAsync(pOrder.orderHash);

			if (!status.isValidated) return BadRequest("Not validated");

			var isOrderExisting = await _context.OfferListeds.AsNoTracking().AnyAsync(x => x.OrderHash.ToLower() == pOrder.orderHash);

			if (isOrderExisting) return BadRequest("Order already exist");
			if (pOrder.parameters.offer.Count != 1) return BadRequest("Invalid order");
			if (pOrder.parameters.consideration.Count != 2) return BadRequest("Invalid order");

			var isWFTMoffered = pOrder.parameters.offer.Single().token.Equals("0x21be370d5312f44cb42ce377bc9b8a0cef1a4c83"); //WFTM address

			if (!isWFTMoffered) return BadRequest("Not offering WFTM");

			var wantedNFT = pOrder.parameters.consideration.Single(x => x.identifierOrCriteria != 0);
			var collection = await _context.Collections.AsNoTracking()
				.Include(x => x.Nfts.Where(y => y.TokenId == wantedNFT.identifierOrCriteria))
				.Include(x => x.OfferListeds.Where(y => y.TokenId == wantedNFT.identifierOrCriteria && y.Offerer == pOrder.parameters.offerer.ToLower()))
				.FirstOrDefaultAsync(x => x.Address == wantedNFT.token);

			if (collection is null || !(collection.IsVerified)) return BadRequest("Collection is not verified");

			if (collection.Nfts.Count != 1) return BadRequest("Nft not found");

			if (collection.OfferListeds.Count > 0) return BadRequest("Offer for this NFT already exist, please cancel or update the existing one");

			decimal priceDcm = 0;
			BigInteger userPart = 0;
			BigInteger ownerPart = 0;
            BigInteger royaltyPart = 0;
            var sellerPrice = pOrder.parameters.offer.Single().startAmount;
			if (sellerPrice.HasHexPrefix())
				sellerPrice = sellerPrice.RemoveHexPrefix();

			var priceBI = BigInteger.Parse($"0{sellerPrice}", System.Globalization.NumberStyles.AllowHexSpecifier);
			userPart = priceBI;
			priceDcm += Web3.Convert.FromWei(priceBI);

			var feePrice = pOrder.parameters.consideration.Single(x => x.identifierOrCriteria == 0).startAmount;
			if (feePrice.HasHexPrefix())
				feePrice = feePrice.RemoveHexPrefix();

			priceBI = BigInteger.Parse($"0{feePrice}", System.Globalization.NumberStyles.AllowHexSpecifier);
			ownerPart = priceBI;

			//priceDcm += Nethereum.Web3.Web3.Convert.FromWei(priceBI);

			var diff = (Math.Exp(BigInteger.Log(ownerPart) - BigInteger.Log(ownerPart + userPart)) * 100) - 2;
			if (diff >= 0.01)
				return BadRequest("Invalid order");

			var contract = _web3.Eth.GetContract(Helper.ERC721Abi, wantedNFT.token);
			var function = contract.GetFunction("ownerOf");
			var owner = (await function.CallAsync<string>(wantedNFT.identifierOrCriteria)).ToLower();

			var newOffer = new OfferListed
			{
				ChainId = 250,
				OrderHash = pOrder.orderHash,
				CollectionId = collection.Id,
				Price = priceDcm.ToString("G29"),
				Signature = pOrder.signature,
				TokenId = wantedNFT.identifierOrCriteria,
				OrderJson = JsonConvert.SerializeObject(pOrder.parameters),
				CreationDate = DateTime.UtcNow,
				ExpirationDate = DateTimeOffset.FromUnixTimeSeconds((long)pOrder.parameters.endTime).DateTime,
				Offerer = pOrder.parameters.offerer,
				Receiver = owner
			};

			var offererProfile = await _context.Profiles.AsNoTracking().FirstOrDefaultAsync(x => x.Address == pOrder.parameters.offerer);

			_context.Notifications.Add(new Notification
			{
				Address = owner,
				Read = false,
				EventId = 4,
				Date = DateTime.UtcNow,
				Data = $"{offererProfile?.Name ?? newOffer.Offerer},{collection.Nfts.Single().Name},{newOffer.Price}"//{0} want to buy your {1} for {2}FTM
			});

			_context.OfferListeds.Add(newOffer);

			await _context.SaveChangesAsync();

			return Json(newOffer);
		}
        #endregion

        #region Cancel
        [HttpPost("CancelOrderOffer")]
		public async Task<IActionResult> CancelOrderOffer(SeaportOrder pOrder)
		{
			var status = await _marketplaceService.GetOrderStatusAsync(pOrder.orderHash);
			if (!status.isCancelled) return BadRequest("Order is not canceled");

			var offerOrder = await _context.OfferListeds.FirstOrDefaultAsync(x => x.OrderHash.ToLower() == pOrder.orderHash.ToLower());
			if (offerOrder is null) return Ok();
			_context.OfferListeds.Remove(offerOrder);
			await _context.SaveChangesAsync();

            return Ok();
		}
        
        #endregion

        [HttpGet("GetSoldForUser")]
		public async Task<IActionResult> GetSoldForUser(string userAddress)
		{
			var fulfilled = await _context.OrderFulfilledHistories.AsNoTracking().Include(x => x.Collection).Where(x => !x.IsOffer && x.Seller.ToLower() == userAddress.ToLower())
				.Select(x => new OrderFulfilledHistory
				{
					ChainId = x.ChainId,
					TokenId	= x.TokenId,
					Collection = new Collection
					{
						Address = x.Collection.Address
					},
					Nft = new Nft
					{
						Name = x.Nft.Name,
						Rarity = new Rarity
						{
							Rank = x.Nft.Rarity.Rank
						},
					},
					Date =	x.Date,
					PriceWei = x.PriceWei,
					IsOffer = x.IsOffer,
					CollectionId= x.CollectionId,
				}).ToListAsync();
			return Json(fulfilled);
		}

		[HttpGet("GetOffersForUser")]
		public async Task<IActionResult> GetOffersForUser(string userAddress)
		{
			var fulfilled = await _context.OfferListeds.AsNoTracking().Where(x => x.Offerer == userAddress.ToLower()).ToListAsync();
			return Json(fulfilled);
		}

		[HttpGet("GetOrdersForCollection")]
		public async Task<IActionResult> GetOrdersForCollection(string pCollectionAddress)
		{
			var listings = await _context.AssetListings.AsNoTracking()
				.Where(x => x.CollectionAddress.ToLower() == pCollectionAddress.ToLower() && x.ExpirationDate > DateTime.UtcNow)
				.ToListAsync();

			var offers = await _context.OfferListeds.AsNoTracking()
				.Where(x => x.Collection.Address == pCollectionAddress && x.ExpirationDate > DateTime.UtcNow)
				.ToListAsync();

			return Json(new OrderAndOffersData(offers,listings));
		}

		[HttpGet("GetSoldForCollection")]
		public async Task<IActionResult> GetSoldForCollection(int collectionId, int take = 25, int skip = 0)
		{
			var solds = await _context.OrderFulfilledHistories.AsNoTracking().OrderByDescending(x => x.Date).Include(x => x.Nft).ThenInclude(x => x.Rarity).Where(x => x.CollectionId == collectionId && !x.IsOffer).Skip(skip).Take(take).ToListAsync();

			return Json(solds);
		}

        [HttpGet("GetBoughtForUser")]
        public async Task<IActionResult> GetBoughtForUser(string userAddress, int take = 25, int skip = 0)
        {
            //.Skip(skip).Take(take)
            var solds = await _context.OrderFulfilledHistories.AsNoTracking().Include(x => x.Collection).Include(x => x.Nft).ThenInclude(x => x.Rarity).Where(x => x.Buyer == userAddress.ToLower() && !x.IsOffer).ToListAsync();

            return Json(solds);
        }

        [HttpGet("GetCollectionMarketplaceStats")]
		public async Task<IActionResult> GetCollectionMarketplaceStats(int pCollectionId)
		{
			var histories = await _context.OrderFulfilledHistories.Where(x => x.CollectionId == pCollectionId).ToListAsync();
			if(histories.Count == 0)
				return Json(new CollectionStats());
			var sales = histories;
			var sales7D = histories.Where(x => x.Date > DateTime.UtcNow.AddDays(-7)).ToList();
			var sales24H = histories.Where(x => x.Date > DateTime.UtcNow.AddDays(-1)).ToList();


			var offers = await _context.OfferListeds.Where(x => x.CollectionId == pCollectionId).ToListAsync();

			return Json(new CollectionStats
			{
				Volume = sales.Sum(x => x.PriceInt),
				Volume7D = sales7D.Sum(x => x.PriceInt),
				Volume24H = sales24H.Sum(x => x.PriceInt),
				NumberOfSales7D = sales7D.Count,
				NumberOfSales24H = sales24H.Count,
				NumberOfSales = sales.Count,
				AveragePrice = Math.Round(offers.Count > 0 ? offers.Average(x => x.PriceInt):0),
				FloorPrice = offers.Count > 0 ? offers.Min(x => x.PriceInt) : 0
			});
		}

		[HttpGet("DeclineOffer")]
		public async Task<IActionResult> DeclineOffer(int offerId, string signatureHash, string address)
		{

			if (!_signerHelper.TryGetGuidFor(address, out Guid guid))
				return NotFound();

			address = address.ToLower();
			var typedData = SignerHelper.Default(guid.ToString());

			var recoveredAddress = new MessageSigner().EcRecover(
				Sha3Keccack.Current.CalculateHash(Eip712TypedDataSigner.Current.EncodeTypedData(typedData)),
				signatureHash);
			if (recoveredAddress.ToLower() != address.ToLower()) return BadRequest();


			var offer = await _context.OfferListeds.FirstOrDefaultAsync(x => x.Id == offerId);
			if (offer is null)
				return NotFound();

			if (offer.Receiver.ToLower() != address)
				return Unauthorized();

			offer.Hidden = true;

			await _context.SaveChangesAsync();
			return Ok();
		}

		[HttpGet("GetUsersOffer")]
		public async Task<IActionResult> GetUsersOffer(string userAddress)
		{
			var offers = await _context.OfferListeds.AsNoTracking().Where(x => (x.Receiver == userAddress.ToLower() || x.Offerer == userAddress.ToLower()) && x.Hidden != true && x.ExpirationDate > DateTime.UtcNow).Select(x => new OfferListed
			{
				Offerer = x.Offerer,
				Receiver = x.Receiver,
				OrderJson = x.OrderJson,
				ExpirationDate = x.ExpirationDate,
				CreationDate = x.CreationDate,
				Price = x.Price,
				TokenId = x.TokenId,
				Signature = x.Signature,
				Collection = new Collection
				{
					Address = x.Collection.Address
				},
				Nft = new Nft
				{
					Name = x.Nft.Name
				},
				Id = x.Id
			}).ToListAsync();
			return Json(offers);
		}

		[HttpGet("GetBestOffer")]
		public async Task<IActionResult> GetBestOffer(int TokenId, string CollectionAddress)
		{
			var allOffers = await _context.OfferListeds.Where(x => x.TokenId == TokenId && x.Collection.Address.ToLower() == CollectionAddress.ToLower() && x.ExpirationDate > DateTime.UtcNow).ToListAsync();

			var max = allOffers.OrderByDescending(x => x.PriceInt).FirstOrDefault();
			return Json(max);
		}

		[HttpGet("GetChestState")]
		public async Task<IActionResult> GetChestState()
		{
			var chestss = await _context.YokaiToolsGeneralChestCounts.AsNoTracking().OrderBy(x => x.RarityId).Where(x => x.Edition == 1).Select(x => x.Count).ToListAsync();

			return Json(chestss);
		}

		[HttpGet("GetRewardChestFor")]
		public async Task<IActionResult> GetRewardChestFor(string userAddress)
		{

			var closedChests = await _context.RewardValues.Include(x => x.RewardRarityNavigation).AsNoTracking().Where(x => x.EarnerAddress == userAddress.ToLower() && !x.Opened).Select(x => new RewardValue
			{
				Id = x.Id,
				EarnerAddress = x.EarnerAddress,
				RewardRarity = x.RewardRarity,
				RewardRarityNavigation = x.RewardRarityNavigation,
				Opened = x.Opened
			}).ToListAsync();

			var openedChests = await _context.RewardValues.Include(x => x.RewardRarityNavigation).AsNoTracking().Where(x => x.EarnerAddress == userAddress.ToLower() && x.Opened).Select(x => new RewardValue
			{
				Id = x.Id,
				EarnerAddress = x.EarnerAddress,
				RewardRarity = x.RewardRarity,
				RewardRarityNavigation = x.RewardRarityNavigation,
				RewardType = x.RewardType,
				NftAddress = x.NftAddress,
				TokenId = x.TokenId,
				Value = x.Value,
				Opened = x.Opened
			}).ToListAsync();
			closedChests.AddRange(openedChests);
			closedChests = closedChests.OrderByDescending(x => x.Id).ToList();
			return Json(closedChests);
		}

		[HttpPost("OpenChestFor")]
		public async Task<IActionResult> OpenChestFor(ChestToOpenData data)
		{
			if (!_signerHelper.TryGetGuidFor(data.Address, out Guid guid))
				return NotFound();

			var typedData = SignerHelper.Default(guid.ToString());

			var address = new MessageSigner().EcRecover(Sha3Keccack.Current.CalculateHash(Eip712TypedDataSigner.Current.EncodeTypedData(typedData)), data.Signature);
			if (address.ToLower() != data.Address.ToLower())
				return Unauthorized();

			var chest = await _context.RewardValues.Include(x => x.RewardTypeNavigation).Include(x => x.RewardRarityNavigation).FirstOrDefaultAsync(x => x.Id == data.Id);
			if (chest is null)
				return NotFound();

			chest.Opened = true;
			await _context.SaveChangesAsync();
			return Json(chest);
		}


		/*
		 
		 
		 listen to orde validation event, add in database from todai admin api

		 */

	}
}
