using System.Numerics;
using BlazorWebAssymblyWeb3.Shared.Forms;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Signer;
using Nethereum.Signer.EIP712;
using Nethereum.Util;

namespace BlazorWebAssymblyWeb3.Server.Services;

public class SignerHelper
{
    private readonly Dictionary<string, Guid> toSign;

    public SignerHelper()
    {
        toSign = new Dictionary<string, Guid>();
    }

    public bool VerifyHashFor(string pAddress, string pHash)
	{
        var userAddress = pAddress.ToLower();
        if (!TryGetGuidFor(userAddress, out Guid guid))
            return false;

        var typedData = Default(guid.ToString());

        var recoverdAddress = new MessageSigner().EcRecover(
            Sha3Keccack.Current.CalculateHash(Eip712TypedDataSigner.Current.EncodeTypedData(typedData)),
            pHash);

        return recoverdAddress.Equals(userAddress, StringComparison.InvariantCultureIgnoreCase);
    }

    public void AddOrReplaceToSign(string pAddress, Guid pGuid)
    {
        var address = pAddress.ToLower();
        if (toSign.ContainsKey(address))
            toSign[address] = pGuid;
        else
            toSign.Add(pAddress, pGuid);
    }

    public bool TryGetGuidFor(string pAddress, out Guid pGuid)
    {
        pGuid = Guid.Empty;
        
        var address = pAddress.ToLower();
        if (!toSign.ContainsKey(address))
            return false;

        pGuid = toSign[address];
        return true;
    }
	public class DomainNoVerifying : IDomain
	{
		[Parameter("string", "name", 1)]
		public virtual string Name { get; set; }

		[Parameter("string", "version", 2)]
		public virtual string Version { get; set; }

		[Parameter("uint256", "chainId", 3)]
		public virtual BigInteger? ChainId { get; set; }


	}
	public static TypedData<DomainNoVerifying> Default(string pContent, BigInteger? pChainId = null) => new()
    {
        Domain = new DomainNoVerifying
		{
            Name = "Todai",
            Version = "1",
            ChainId = pChainId ?? 250,
        },
        Types = new Dictionary<string, MemberDescription[]>
        {
            ["EIP712Domain"] = new[]
            {
                new MemberDescription { Name = "name", Type = "string" },
                new MemberDescription { Name = "version", Type = "string" },
                new MemberDescription { Name = "chainId", Type = "uint256" }
            },
            ["Message"] = new[]
            {
                new MemberDescription { Name = "contents", Type = "string" }
            }
        },
        PrimaryType = "Message",
        Message = new[]
        {
            new MemberValue
            {
                TypeName = "string", Value = pContent
            }
        }
    };

    public static TypedData<Domain> MarketplaceOrderDefault() => new()
    {
        Domain = new Domain
        {
            Name = "Seaport",
            Version = "1",
            ChainId = 250,
			VerifyingContract = "0xCd594B4Ea6059bE5F99839eB39dd4404cDa9829d"
		},
        Types = new Dictionary<string, MemberDescription[]>
        {
            ["EIP712Domain"] = new[]
            {
                new MemberDescription { Name = "name", Type = "string" },
                new MemberDescription { Name = "version", Type = "string" },
                new MemberDescription { Name = "chainId", Type = "uint256" },
                new MemberDescription { Name = "verifyingContract", Type = "address"}
            },
            ["OrderComponents"] = new[]
            {
				new MemberDescription { Name = "offerer", Type = "address"},
                new MemberDescription { Name = "zone", Type= "address"},
                new MemberDescription { Name = "offer", Type = "OfferItem[]"},
                new MemberDescription { Name = "consideration", Type = "ConsiderationItem[]"},
                new MemberDescription { Name = "orderType", Type = "uint8"},
                new MemberDescription { Name = "startTime", Type = "uint256"},
                new MemberDescription { Name = "endTime", Type = "uint256" },
                new MemberDescription { Name = "zoneHash", Type = "bytes32"},
                new MemberDescription { Name = "salt", Type = "uint256"},
                new MemberDescription { Name = "conduitKey", Type = "bytes32"},
                new MemberDescription { Name = "nonce", Type = "uint256"}
            },
			["OfferItem"] = new[]
			{
				new MemberDescription { Name = "itemType", Type = "uint8"},
				new MemberDescription { Name = "token", Type = "address"},
				new MemberDescription { Name = "identifierOrCriteria", Type = "uint256"},
				new MemberDescription { Name = "startAmount", Type = "uint256"},
				new MemberDescription { Name = "endAmount", Type = "uint256"}

			},
			["ConsiderationItem"] = new[]
			{
				new MemberDescription { Name = "itemType", Type = "uint8"},
				new MemberDescription { Name = "token", Type = "address"},
				new MemberDescription { Name = "identifierOrCriteria", Type = "uint256"},
				new MemberDescription { Name = "startAmount", Type = "uint256"},
				new MemberDescription { Name = "endAmount", Type = "uint256"},
                new MemberDescription { Name = "recipient", Type = "address"}
			}
        },
        PrimaryType = "OrderComponents",
    };
}