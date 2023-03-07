using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using HtmlAgilityPack;

namespace BlazorWebAssymblyWeb3.Client.Data;

public static class YokaiPatcher
{
    public static bool IsValidEthAddress(this string pPotentialAddress)
    {
        return new Regex(@"^0x[a-fA-F0-9]{40}$",RegexOptions.IgnoreCase).IsMatch(pPotentialAddress);
    }
    
    public static string ShortenAddress(string? pAddress)
    {
        if (pAddress is null) return "";
        return $"{pAddress.Substring(0, 5)}…{pAddress[^4..]}";
    }

    public static string ShortenName(string pName, int pLength = 10)
	{
        if (pName is null) return "";
        if (pName.Length < pLength)
            return pName;

        return $"{pName.Substring(0, pLength)}...";
	}
    public static string GetHoldersFromHtml(string pHtml)
    {
        try
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(pHtml);

            var nodes = doc.DocumentNode.SelectNodes("//div");
            if (nodes != null)
                return nodes.FirstOrDefault(x => x.Id == "ContentPlaceHolder1_tr_tokenHolders")?.Element("div")
                    .Elements("div").Last().InnerHtml.Split(" ").First(x => x != "\n");
            return "";
        }
        catch
        {
            return "";
        }
    }
    
    public static void RemoveAnimation(Yokai pT)
    {
        var svg = Encoding.UTF8.GetString(Convert.FromBase64String(pT.Data.image.Substring(26)));
        var doc = XDocument.Parse(svg);
        var node = doc.Elements("animateTransform");
        foreach (var xElement in node)
        {
            Console.WriteLine(xElement.Value);
            xElement.Remove();
        }
        
        
        pT.Data.image = "data:image/svg+xml;base64,"+Convert.ToBase64String(Encoding.UTF8.GetBytes(doc.ToString()));
        Console.WriteLine(pT.Data.image);
    }
    
    public static void Patch(Yokai pT)
    {
        if (!pT.IsDownloaded || pT.IsNonExistent || pT.Data.attributes is null || pT.Data.attributes.Count() == 0) return;

        var attrs = pT.Data.attributes;
        var toFix = attrs.FirstOrDefault(x => x.trait_type == "Eyes" && x.value == "Moon Kin");
        var toFix2 = attrs.FirstOrDefault(x => x.trait_type == "Mark" && x.value == "Kin Moon");

        if (toFix is null && toFix2 is null)
        {
            return;
        }
        var svg = Encoding.UTF8.GetString(Convert.FromBase64String(pT.Data.image.Substring(26)));
        var doc = XDocument.Parse(svg);
        
        if (toFix2 != null)
        {
            var node = doc.Descendants("{http://www.w3.org/2000/svg}linearGradient").FirstOrDefault(x => x.Attribute("id")?.Value == "Kin Moon Gradient");

            node?.SetAttributeValue("id","Kin_Moon_Gradient");
        }

        if (toFix != null)
        {
            
            var gradients = doc.Descendants("{http://www.w3.org/2000/svg}linearGradient").Where(x => x.Attribute("id")?.Value == "Moon Aka").ToList();
            var paths = doc.Descendants("{http://www.w3.org/2000/svg}path").Where(x => x.Attribute("id")?.Value == "Moon Aka").ToList();
            if (gradients.Count > 0)
            {
                var i = 1;
                foreach (var gradient in gradients)
                {
                    gradient.SetAttributeValue("id",$"Moon_Aka_{i}"); 
                    i++;
                }
            }

            if (paths.Count > 0)
            {
                var i = 1;
                foreach (var path in paths)
                {
                    path.SetAttributeValue("fill",$"url(#Moon_Aka_{i})"); 
                    i++;
                }
            }
            
        }
        pT.Data.image = "data:image/svg+xml;base64,"+Convert.ToBase64String(Encoding.UTF8.GetBytes(doc.ToString()));
    }
    
    
}