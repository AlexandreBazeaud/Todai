using Microsoft.JSInterop;

namespace BlazorWebAssymblyWeb3.Client.Services;

public class SessionStorageService
{
    private readonly IJSRuntime _jSRuntime;
    
    public SessionStorageService(IJSRuntime jSRuntime)
    {
        _jSRuntime = jSRuntime;
    }
    
    public ValueTask<bool> ContainKeyAsync(string key, CancellationToken? cancellationToken = null)
        => _jSRuntime.InvokeAsync<bool>("sessionStorage.hasOwnProperty", cancellationToken ?? CancellationToken.None, key);
    public ValueTask SetItemAsync(string key, string data, CancellationToken? cancellationToken = null)
        => _jSRuntime.InvokeVoidAsync("sessionStorage.setItem", cancellationToken ?? CancellationToken.None, key, data);
    public ValueTask<string> GetItemAsync(string key, CancellationToken? cancellationToken = null)
        => _jSRuntime.InvokeAsync<string>("sessionStorage.getItem", cancellationToken ?? CancellationToken.None, key);
    public ValueTask RemoveItemAsync(string key, CancellationToken? cancellationToken = null)
        => _jSRuntime.InvokeVoidAsync("sessionStorage.removeItem", cancellationToken ?? CancellationToken.None, key);
}