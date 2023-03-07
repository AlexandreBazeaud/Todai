using BlazorWebAssymblyWeb3.Server.Data;
using BlazorWebAssymblyWeb3.Server.Services;
using Microsoft.EntityFrameworkCore;
using Nethereum.Web3;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews().AddNewtonsoftJson(options =>
{
	options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
});
builder.Services.AddRazorPages();
var config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(AppContext.BaseDirectory +"config.json"));
if (config is null) throw new Exception("No config file found");

Helper.DiscordLoginSecret = config.DiscordLoginSecret;

builder.Services.AddSingleton<SignerHelper>();
builder.Services.AddSingleton<MarketplaceService>();
builder.Services.AddHttpClient();
builder.Services.AddHttpClient("moralis", x =>
{
	x.DefaultRequestHeaders.Add("X-API-Key", config.MoralisApiKey);
});
builder.Services.AddDbContext<YokaiToolsContext>(options =>
{
	options.UseSqlServer(config.ConnectionString);
});
builder.Services.AddSingleton(new Web3("https://rpc.ftm.tools/"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseWebAssemblyDebugging();
}
else
{
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToPage("/_Host");
//app.MapFallbackToFile("index.html");

app.Run();
