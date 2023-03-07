using BlazorWebAssymblyWeb3.Shared.Forms;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorWebAssymblyWeb3.Server;

public partial class Collection
{
    [NotMapped] 
    public List<int> Favorites { get; set; }
    
    [NotMapped]
    public int ActualSupply { get; set; }

    [NotMapped]
    public CollectionStats Stats { get; set; }
}