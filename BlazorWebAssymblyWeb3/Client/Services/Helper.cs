using System.Numerics;
using BlazorWebAssymblyWeb3.Client.Data;
using MetaMask.Blazor;
using Nethereum.Util;
using Org.BouncyCastle.Asn1.X509;

namespace BlazorWebAssymblyWeb3.Client.Services;

public class Helper
{
    private readonly MetaMaskService _metaMaskService;
    public Helper(MetaMaskService pMetamaskService)
    {
        _metaMaskService = pMetamaskService;
    }
    
    public static string ToReadableVolume(int Volume)
    {
        var volumeString = Volume.ToString();
        //Console.WriteLine(volumeString);
        var readableString = "";
		//1125143
		switch (volumeString.Length)
        {
			case var _ when volumeString.Length < 4:
                readableString = volumeString;
				break;
			case var _ when volumeString.Length > 7:
				readableString = volumeString.Substring(0, volumeString.Length - 6) +'M';
				break;
			case var _ when volumeString.Length == 7:
				readableString = volumeString.Substring(0, volumeString.Length - 5).Insert(1, ",") + 'M';
				break;
			case var _ when volumeString.Length > 4:
				readableString = volumeString.Substring(0, volumeString.Length - 3) + 'K';
				break;
			case var _ when volumeString.Length == 4:
                readableString = volumeString.Substring(0, volumeString.Length - 2).Insert(1, ",") + 'K';
				break;
        }

        return readableString;
	}

    public static string ToReadableSoldDate(DateTime pDateSold)
    {
		var datetime =  DateTime.UtcNow - pDateSold;
		string message = "";

		if (datetime.TotalDays > 0)
			message = $"{Math.Round(datetime.TotalDays)} days ago";
        else if(datetime.TotalHours > 0)
            message = $"{Math.Round(datetime.TotalHours)} hours ago";
        else if (datetime.TotalMinutes > 0)
			message = $"{Math.Round(datetime.TotalMinutes)} minutes ago";
		else if (datetime.TotalSeconds > 0)
			message = $"{Math.Round(datetime.TotalSeconds)} seconds ago";
		return message;
	}

    public static string ToReadableExpirationDate(DateTime pTimestamp)
    {
        var datetime = pTimestamp - DateTime.UtcNow;
        Console.WriteLine(datetime);
        string message = "a";
        
        if (datetime.TotalDays > 0)
            message = $"Ends in {Math.Round(datetime.TotalDays)} days";
        else if (datetime.TotalHours > 0)
            message = $"Ends in {Math.Round(datetime.TotalHours)} hours";
        else if (datetime.TotalMinutes > 0)
            message = $"Ends in {Math.Round(datetime.TotalMinutes)} minutes";
        else if (datetime.TotalSeconds > 0)
            message = $"Ends in {Math.Round(datetime.TotalSeconds)} seconds";
        return message;
    }

    public static string ToReadableExpirationDate(long pTimestamp)
    {
        var datetime = DateTimeOffset.FromUnixTimeSeconds(pTimestamp).DateTime;
        return ToReadableExpirationDate(datetime);
	}

    public static string ToReadableChestTimer(TimeSpan time)
    {
        string message = "";

        if (time.TotalDays > 0)
            message = $"{time.Days}j {time.ToString("hh\\:mm\\:ss")}s";
        else
            message = $"{time.ToString("hh\\:mm\\:ss")}s";
        
        return message;
    }

    public static decimal ToEther(BigInteger value)
	{
		
		var unitValue =  BigIntegerExtensions.ParseInvariant("1000000000000000000");
		var decimalPlacesToUnit =  unitValue.ToStringInvariant().Length - 1;
		return (decimal)new BigDecimal(value, decimalPlacesToUnit * -1);
	}

    public async Task<TransactionResult> CreateSignTransactionAndPayload<T>(T pMessage, BigInteger? pChainId = null)
    {
        try
        {
            var payload = new TypedDataPayload<T>
            {
                Domain = new Domain
                {
                    Name = "Todai",
                    Version = "1",
                    ChainId = pChainId ?? 250
                },
                Types = new Dictionary<string, TypeMemberValue[]>
                {
                    ["EIP712Domain"] = new[]
                    {
                        new TypeMemberValue { Name = "name", Type = "string" },
                        new TypeMemberValue { Name = "version", Type = "string" },
                        new TypeMemberValue { Name = "chainId", Type = "uint256" }
                    },
                    ["Message"] = new[]
                    {
                        new TypeMemberValue { Name = "contents", Type = "string" }
                    }
                },
                PrimaryType = "Message",
                Message = pMessage
            }.ToJson();
            return new TransactionResult(true,null, await _metaMaskService.SignTypedDataV4(payload));
        }
        catch (Exception ex)
        {
            return new TransactionResult(false, ex.Message, null);
        }
    }
}

public record struct TransactionResult(bool IsSuccess, string? Error, string? Hash);