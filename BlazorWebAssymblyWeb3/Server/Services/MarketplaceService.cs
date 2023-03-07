using BlazorWebAssymblyWeb3.Shared.Forms;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.Web3;
using System.Numerics;

namespace BlazorWebAssymblyWeb3.Server.Services
{
	public class MarketplaceService
	{

		private readonly Web3 _web3;
		private readonly string _abi;
		private readonly Contract _contract;
		private const string MarketplaceAddress = "0xCd594B4Ea6059bE5F99839eB39dd4404cDa9829d";

		public MarketplaceService()
		{
			_abi = File.ReadAllText($"{AppContext.BaseDirectory}/MarketplaceAbi.json");
			_web3 = new Web3("https://rpc.ftm.tools/");
			_contract = _web3.Eth.GetContract(_abi, MarketplaceAddress);
		}

		public async Task<data> GetOrderStatusAsync(string pOrderHash)
		{
			var functionOrderStatus = _contract.GetFunction("getOrderStatus");

			var bytes = Helper.HashToBytes(pOrderHash);


			var status = await functionOrderStatus.CallAsync<data>(bytes);
			return status;
		}


		public async Task<string> GetAndVerifyOrderHash(SeaportOrder pOrder)
		{
			var functionGetOrderHash = _contract.GetFunction("getOrderHash");


			pOrder.parameters.conduitKey = Helper.HashToBytes("0x0000000000000000000000000000000000000000000000000000000000000000");
			pOrder.parameters.zoneHash = Helper.HashToBytes("0x0000000000000000000000000000000000000000000000000000000000000000");

			//pOrder.parameters.offer



			//var input = functionGetOrderHash.CreateTransactionInput("0x89B07Ba2d3c04A55632060AA9ea372E1408e3d7B", pOrder.parameters);

			var hash = await functionGetOrderHash.CallAsync<byte[]>(pOrder.parameters);
			return BitConverter.ToString(hash);
		}

		[FunctionOutput]
		public class data
		{
			[Parameter("bool")]
			public bool isValidated { get; set; }
			[Parameter("bool")]
			public bool isCancelled { get; set; }
			[Parameter("uint256")]
			public int totalFilled { get; set; }
			[Parameter("uint256")]
			public int totalSize { get; set; }
		}
	}
}
