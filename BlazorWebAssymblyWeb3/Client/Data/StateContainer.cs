namespace BlazorWebAssymblyWeb3.Client.Data;

public class StateContainer
{
    private string? currentConnectedAddress;

    public string CurrentConnectedAddress
    {
        get => currentConnectedAddress ?? string.Empty;
        set
        {
            currentConnectedAddress = value;
            NotifyStateChanged();
        }
    }

    private int? currentChainId;

    public int? CurrentChainId
    {
        get => currentChainId ?? null;
        set
        {
            currentChainId = value;
            NotifyStateChanged();
        }
    }


	private double? fantomPrice;

	public double? FantomPrice
	{
		get => fantomPrice ?? null;
		set
		{
			fantomPrice = value;
			NotifyStateChanged();
		}
	}

	public event Action? OnChange;

    private void NotifyStateChanged() => OnChange?.Invoke();
}