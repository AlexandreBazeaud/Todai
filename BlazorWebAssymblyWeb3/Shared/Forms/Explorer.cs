using BlazorWebAssymblyWeb3.Server;
namespace BlazorWebAssymblyWeb3.Shared.Forms;

public class Explorer
{
    public record ExplorerPayload(List<Collection> Collections, int Count);

    public record ExplorerUsersPayload(List<Profile> Profiles, int Count);
}