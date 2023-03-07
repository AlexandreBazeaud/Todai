namespace BlazorWebAssymblyWeb3.Client.Data;

public static class Constant
{
    public static string YOKAIADDRESS = "0x59c7b16369422959eeb218c7270e3b5132cb1f28";
    public static Dictionary<int, ChainData> HandledChains = new ()
    {
        {250,new ChainData(250, "Fantom","0x6c926e3723E1B33A294a1F1042D3FE8444A2C368")},
        {4002, new ChainData(4002,"FantomTestNet", "0x1a5A1B97a74A8385F5F772Ae720469582d4B9f34")}
    };
    public static string DISCORDCONNECT = "https://discordapp.com/api/oauth2/authorize?response_type=code&client_id=923646316419088425&scope=identify&state=15773059ghq9183habn&redirect_uri=https%3A%2F%2Flocalhost%3A7103%2Fdiscordlink";
}

public record ChainData(int ChainId, string Name, string BatcherAddress);
