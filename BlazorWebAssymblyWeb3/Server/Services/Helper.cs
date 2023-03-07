using Microsoft.AspNetCore.Mvc.Rendering;

namespace BlazorWebAssymblyWeb3.Server.Services;

public static class Helper
{
    public static string ERC721Abi = File.ReadAllText(AppContext.BaseDirectory + "/abi.json");
    public static string DiscordLoginSecret = "";
    
    //public static bool UpdateElementIfDifferent<T>(T pToEdit, object? pPotentialNewValue, string pPropertyName)
    //{
    //    var property = pToEdit.GetType().GetProperty(pPropertyName);
    //    if (property is null) return false;
        
    //    var a = property.GetValue(pToEdit);

    //    if (a is null && pPotentialNewValue is null) 
    //        return false;
    //    // if (a is null && pPotentialNewValue != null)
    //    //     property.SetValue(pToEdit, pPotentialNewValue);
    //    if (a != null && a.Equals(pPotentialNewValue)) 
    //        return false;
        
    //    property.SetValue(pToEdit, pPotentialNewValue);
    //    return true;
    //}

    public static byte[] HashToBytes(string pHash)
	{

		var cut = pHash.Substring(2);
		var bytes = new byte[32];


		for (int i = 0; i < cut.Length / 2; i++)
		{
			bytes[i] = byte.Parse($"{cut[i * 2]}{cut[(i * 2) + 1]}", System.Globalization.NumberStyles.HexNumber);
		}

        return bytes;
	}
	public static bool IsDebug(this IHtmlHelper htmlHelper)
	{
        #if DEBUG
        		return true;
        #else
              return false;
        #endif
	}
}