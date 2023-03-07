using System;
using System.Collections.Generic;

namespace BlazorWebAssymblyWeb3.Server
{
    public partial class Profile
    {
        public Profile()
        {
            ProfileIdFolloweds = new HashSet<Profile>();
            ProfileIdFollowings = new HashSet<Profile>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public string Address { get; set; } = null!;
        public int? CollectionId { get; set; }
        public int? TokenId { get; set; }
        public string? Localisation { get; set; }
        public string? Activity { get; set; }
        public string? Link { get; set; }
        public string? TwitterHandle { get; set; }
        public string? InstagramNickname { get; set; }
        public string? Bio { get; set; }
        public DateTime? DateOfNickname { get; set; }
        public int AllExp { get; set; }
        public string? Gender { get; set; }
        public long? DiscordId { get; set; }
        public int Level { get; set; }

        public virtual Nft? Nft { get; set; }

        public virtual ICollection<Profile> ProfileIdFolloweds { get; set; }
        public virtual ICollection<Profile> ProfileIdFollowings { get; set; }
    }
}
