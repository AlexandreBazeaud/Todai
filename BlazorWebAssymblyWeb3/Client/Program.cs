using BlazorWebAssymblyWeb3.Client;
using BlazorWebAssymblyWeb3.Client.Data;
using BlazorWebAssymblyWeb3.Client.Services;
using MetaMask.Blazor;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient<PaintswapService>(client =>
{
    client.BaseAddress = new Uri("https://api.paintswap.finance/v2/");
});
builder.Services.AddSingleton<SessionStorageService>();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddSingleton<MetaMaskService>();
builder.Services.AddSingleton<Helper>();
builder.Services.AddSingleton<StateContainer>();
builder.Services.AddSingleton<AlertMessageService>();
builder.Services.AddSingleton<NftKeyService>();

await builder.Build().RunAsync();
