using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Signer.EIP712;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;
using System.Threading.Tasks;

namespace BlazorWebAssymblyWeb3.Shared.Forms
{
	public class SeaportOrder
	{
		public SeaportOrderParameters parameters { get; set; }
		public string signature { get; set; }
		public string orderHash { get; set; }
		public int numerator { get; set; }
		public int denominator { get; set; }
		public string extraData { get; set; }
	}

	
	public class SeaportOrderParameters
	{
		[Parameter("address",1)]
		public string offerer { get; set; }

		[Parameter("address",2)]
		public string zone { get; set; }

		[Parameter("tuple[]","offer",3,"OfferItem[]")]
		public List<SeaportOffer> offer { get; set; }

		[Parameter("tuple[]", "consideration", 4,"ConsiderationItem[]")]
		public List<SeaportConsideration> consideration { get; set; }

		[Parameter("uint8",5)]
		public int orderType { get; set; }

		//[JsonProperty("zoneHash")]
		//public string zoneHash { get; set; }

		[JsonIgnore]
		[Parameter("bytes32", "zoneHash",8)]
		public byte[]? zoneHash { get; set; }


		[Parameter("uint256",9)]
		public string salt { get; set; }


		//[JsonProperty("conduitKey")]
		//public string conduitKey { get; set; }

		[JsonIgnore]
		[Parameter("bytes32", "conduitKey",10)]
		public byte[]? conduitKey { get; set; }

		[Parameter("uint256",6)]
		public ulong startTime { get; set; }

		[Parameter("uint256",7)]
		public ulong endTime { get; set; }

		[Parameter("uint256",11)]
		public int nonce { get; set; }
	}

	public class SeaportConsideration
	{
		[Parameter("uint8",1)]
		public int itemType { get; set; }

		[Parameter("address",2)]
		public string token { get; set; }

		[Parameter("uint256",3)]
		public int identifierOrCriteria { get; set; }

		[Parameter("uint256",4)]
		public string startAmount { get; set; }

		[Parameter("uint256",5)]
		public string endAmount { get; set; }

		[Parameter("address", 6)]
		public string recipient { get; set; }
	}

	public class SeaportOffer
	{
		[Parameter("uint8",1)]
		public int itemType { get; set; }

		[Parameter("address",2)]
		public string token { get; set; }

		[Parameter("uint256",3)]
		public int identifierOrCriteria { get; set; }

		[Parameter("uint256",4)]
		public string startAmount { get; set; }

		[Parameter("uint256",5)]
		public string endAmount { get; set; }
	}
}
