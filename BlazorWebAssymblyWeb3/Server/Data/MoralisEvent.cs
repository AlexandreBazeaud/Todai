using Newtonsoft.Json;

namespace BlazorWebAssymblyWeb3.Server.Data
{
	public class MoralisEvent<T>
	{
		public string triggerName { get; set; }
		public T @object { get; set; }
		public bool master { get; set; }
		public Log log { get; set; }
		public string ip { get; set; }
		public Context context { get; set; }
		public User user { get; set; }
		public string installationId { get; set; }
	}

	public class OrderFulfilled
	{
		public BlockTimestamp block_timestamp { get; set; }
		public List<List<string>> consideration { get; set; }
		public List<List<string>> offer { get; set; }
		public int log_index { get; set; }
		public string transaction_hash { get; set; }
		public string address { get; set; }
		public string block_hash { get; set; }
		public int block_number { get; set; }
		public bool confirmed { get; set; }
		public string fulfiller { get; set; }
		public string offerer { get; set; }
		public string orderHash { get; set; }
		public int transaction_index { get; set; }
		public string zone { get; set; }
		public DateTime createdAt { get; set; }
		public DateTime updatedAt { get; set; }
		public string objectId { get; set; }
		public string className { get; set; }
	}

	public class BlockTimestamp
	{
		public string __type { get; set; }
		public DateTime iso { get; set; }
	}
	public class Options
	{
		public bool jsonLogs { get; set; }
		public string logsFolder { get; set; }
		public bool verbose { get; set; }
		public int maxLogFiles { get; set; }
	}

	public class Log
	{
		public Options options { get; set; }
		public string appId { get; set; }
	}

	public class Value
	{
		[JsonProperty("$numberDecimal")]
		public string NumberDecimal { get; set; }
	}

	public class Context
	{
	}

	public class RoleCoreservices
	{
		public bool read { get; set; }
		public bool write { get; set; }
	}


	public class User
	{
		public string username { get; set; }
		public DateTime createdAt { get; set; }
		public DateTime updatedAt { get; set; }
		public string sessionToken { get; set; }
		public string objectId { get; set; }
	}
}
