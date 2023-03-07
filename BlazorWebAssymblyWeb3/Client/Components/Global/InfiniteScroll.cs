namespace BlazorWebAssymblyWeb3.Client.Components.Global;

public sealed class InfiniteScrollingItemsProviderRequest
{
    public InfiniteScrollingItemsProviderRequest(int startIndex, CancellationToken cancellationToken)
    {
        StartIndex = startIndex;
        CancellationToken = cancellationToken;
    }

    public int StartIndex { get; }
    public CancellationToken CancellationToken { get; }
}

public delegate Task<IEnumerable<int>> ItemsProvider(InfiniteScrollingItemsProviderRequest request);