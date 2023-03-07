using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using BlazorWebAssymblyWeb3.Server;

namespace BlazorWebAssymblyWeb3.Server.Data
{
    public partial class YokaiToolsContext : DbContext
    {
        public YokaiToolsContext()
        {
        }

        public YokaiToolsContext(DbContextOptions<YokaiToolsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AssetListing> AssetListings { get; set; } = null!;
        public virtual DbSet<Attribute> Attributes { get; set; } = null!;
        public virtual DbSet<Attributeoption> Attributeoptions { get; set; } = null!;
        public virtual DbSet<AttributesType> AttributesTypes { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Collection> Collections { get; set; } = null!;
        public virtual DbSet<CollectionLinkCategory> CollectionLinkCategories { get; set; } = null!;
        public virtual DbSet<CollectionLinkKeyword> CollectionLinkKeywords { get; set; } = null!;
        public virtual DbSet<CollectionStorageType> CollectionStorageTypes { get; set; } = null!;
        public virtual DbSet<DiscordMetaMaskLink> DiscordMetaMaskLinks { get; set; } = null!;
        public virtual DbSet<Favorite> Favorites { get; set; } = null!;
        public virtual DbSet<FavoritedCollection> FavoritedCollections { get; set; } = null!;
        public virtual DbSet<Keyword> Keywords { get; set; } = null!;
        public virtual DbSet<Level> Levels { get; set; } = null!;
        public virtual DbSet<Nft> Nfts { get; set; } = null!;
        public virtual DbSet<Notification> Notifications { get; set; } = null!;
        public virtual DbSet<NotificationId> NotificationIds { get; set; } = null!;
        public virtual DbSet<OfferListed> OfferListeds { get; set; } = null!;
        public virtual DbSet<OrderFulfilledHistory> OrderFulfilledHistories { get; set; } = null!;
        public virtual DbSet<Profile> Profiles { get; set; } = null!;
        public virtual DbSet<Rarity> Rarities { get; set; } = null!;
        public virtual DbSet<RewardRarity> RewardRarities { get; set; } = null!;
        public virtual DbSet<RewardType> RewardTypes { get; set; } = null!;
        public virtual DbSet<RewardValue> RewardValues { get; set; } = null!;
        public virtual DbSet<SwapHistory> SwapHistories { get; set; } = null!;
        public virtual DbSet<SwapOffer> SwapOffers { get; set; } = null!;
        public virtual DbSet<ToVerifyCollection> ToVerifyCollections { get; set; } = null!;
        public virtual DbSet<ToVerifyCollectionLinkKeyword> ToVerifyCollectionLinkKeywords { get; set; } = null!;
        public virtual DbSet<YokaiToolsGeneralChestCount> YokaiToolsGeneralChestCounts { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AssetListing>(entity =>
            {
                entity.ToTable("AssetListings", "YokaiToolsGeneral");

                entity.HasIndex(e => e.OrderHash, "AssetListings_OrderHash_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.OrderJson, "AssetListings_OrderJson_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.Signature, "AssetListings_Signature_uindex")
                    .IsUnique();

                entity.Property(e => e.CollectionAddress)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CreationDate).HasColumnType("datetime");

                entity.Property(e => e.ExpirationDate).HasColumnType("datetime");

                entity.Property(e => e.Offerer)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.OrderHash)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.OrderJson)
                    .HasMaxLength(2000)
                    .IsUnicode(false);

                entity.Property(e => e.Price)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Signature)
                    .HasMaxLength(200)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Attribute>(entity =>
            {
                entity.HasKey(e => new { e.CollectionId, e.TokenId, e.AttributeTypeId })
                    .HasName("Attributes_pk_2");

                entity.ToTable("Attributes", "YokaiToolsGeneral");

                entity.HasIndex(e => new { e.AttributeTypeId, e.TokenId }, "Attributes_AttributeTypeId_TokenId_uindex")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasOne(d => d.AttributeType)
                    .WithMany(p => p.Attributes)
                    .HasForeignKey(d => d.AttributeTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Attributes_AttributesTypes_Id_fk");

                entity.HasOne(d => d.AttributeTypeOption)
                    .WithMany(p => p.Attributes)
                    .HasForeignKey(d => d.AttributeTypeOptionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Attributes_Attributeoptions_Id_fk");

                entity.HasOne(d => d.Nft)
                    .WithMany(p => p.Attributes)
                    .HasForeignKey(d => new { d.CollectionId, d.TokenId })
                    .HasConstraintName("Attributes_Nft_CollectionId_TokenId_fk");
            });

            modelBuilder.Entity<Attributeoption>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("Attributeoptions_pk")
                    .IsClustered(false);

                entity.ToTable("Attributeoptions", "YokaiToolsGeneral");

                entity.Property(e => e.OptionValue)
                    .HasMaxLength(1500)
                    .IsUnicode(false);

                entity.HasOne(d => d.AttributeType)
                    .WithMany(p => p.Attributeoptions)
                    .HasForeignKey(d => d.AttributeTypeId)
                    .HasConstraintName("Attributeoptions_AttributesTypes_Id_fk");
            });

            modelBuilder.Entity<AttributesType>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("AttributesTypes_pk")
                    .IsClustered(false);

                entity.ToTable("AttributesTypes", "YokaiToolsGeneral");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ScoreNoCount)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Type)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Collection)
                    .WithMany(p => p.AttributesTypes)
                    .HasForeignKey(d => d.CollectionId)
                    .HasConstraintName("AttributesTypes_Collections_Id_fk");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Categories", "YokaiToolsGeneral");

                entity.HasIndex(e => e.Name, "Categories_Name_uindex")
                    .IsUnique();

                entity.Property(e => e.Name)
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Collection>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("Collections_pk")
                    .IsClustered(false);

                entity.ToTable("Collections", "YokaiToolsGeneral");

                entity.HasIndex(e => e.Address, "Collections_Address_uindex")
                    .IsUnique();

                entity.Property(e => e.Address)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Banner)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Description)
                    .HasMaxLength(1500)
                    .IsUnicode(false);

                entity.Property(e => e.Discord)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Finite)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsWhitelisted).HasDefaultValueSql("((0))");

                entity.Property(e => e.MintLink)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.NftKeyAlias)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.OwnerAddress)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ProfilePicture)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.ReleaseDate).HasColumnType("date");

                entity.Property(e => e.Slice).HasDefaultValueSql("((5))");

                entity.Property(e => e.Twitter)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Website)
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.HasOne(d => d.StorageTypeNavigation)
                    .WithMany(p => p.Collections)
                    .HasForeignKey(d => d.StorageType)
                    .HasConstraintName("Collections_CollectionStorageType_id_fk");
            });

            modelBuilder.Entity<CollectionLinkCategory>(entity =>
            {
                entity.ToTable("Collection_link_Categories", "YokaiToolsGeneral");

                entity.HasIndex(e => e.Id, "Collection_link_Categories_Id_uindex")
                    .IsUnique();

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.CollectionLinkCategories)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("Collection_link_Categories_Categories_Id_fk");

                entity.HasOne(d => d.Collection)
                    .WithMany(p => p.CollectionLinkCategories)
                    .HasForeignKey(d => d.CollectionId)
                    .HasConstraintName("Collection_link_Categories_Collections_Id_fk");
            });

            modelBuilder.Entity<CollectionLinkKeyword>(entity =>
            {
                entity.ToTable("Collection_link_keywords", "YokaiToolsGeneral");

                entity.Property(e => e.KeywordName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Collection)
                    .WithMany(p => p.CollectionLinkKeywords)
                    .HasForeignKey(d => d.CollectionId)
                    .HasConstraintName("Collection_link_keywords_Collections_Id_fk");
            });

            modelBuilder.Entity<CollectionStorageType>(entity =>
            {
                entity.ToTable("CollectionStorageType", "YokaiToolsGeneral");

                entity.HasIndex(e => e.Name, "CollectionStorageType_Name_uindex")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(30)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<DiscordMetaMaskLink>(entity =>
            {
                entity.HasKey(e => e.DiscordId)
                    .HasName("DiscordMetaMaskLinks_pk");

                entity.ToTable("DiscordMetaMaskLinks", "YokaiToolsGeneral");

                entity.HasIndex(e => e.Address, "DiscordMetaMaskLinks_Address_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.DiscordId, "DiscordMetaMaskLinks_DiscordId_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.Guid, "DiscordMetaMaskLinks_Guid_uindex")
                    .IsUnique();

                entity.Property(e => e.DiscordId)
                    .HasMaxLength(18)
                    .IsUnicode(false);

                entity.Property(e => e.Address)
                    .HasMaxLength(42)
                    .IsUnicode(false);

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Guid)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.LastUpdate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Favorite>(entity =>
            {
                entity.HasKey(e => new { e.CollectionId, e.TokenId, e.WalletAddress })
                    .HasName("Favorites_pk");

                entity.ToTable("Favorites", "YokaiToolsGeneral");

                entity.Property(e => e.WalletAddress)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Since).HasColumnType("datetime");

                entity.HasOne(d => d.Nft)
                    .WithMany(p => p.Favorites)
                    .HasForeignKey(d => new { d.CollectionId, d.TokenId })
                    .HasConstraintName("Favorites_Nft_CollectionId_TokenId_fk");
            });

            modelBuilder.Entity<FavoritedCollection>(entity =>
            {
                entity.HasKey(e => new { e.WalletAddress, e.CollectionId })
                    .HasName("FavoritedCollection_pk");

                entity.ToTable("FavoritedCollection", "YokaiToolsGeneral");

                entity.Property(e => e.WalletAddress)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Since).HasColumnType("datetime");

                entity.HasOne(d => d.Collection)
                    .WithMany(p => p.FavoritedCollections)
                    .HasForeignKey(d => d.CollectionId)
                    .HasConstraintName("FavoritedCollection_Collections_Id_fk");
            });

            modelBuilder.Entity<Keyword>(entity =>
            {
                entity.ToTable("Keywords", "YokaiToolsGeneral");

                entity.HasIndex(e => e.Keyword1, "Keywords_Keyword_uindex")
                    .IsUnique();

                entity.Property(e => e.Keyword1)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("Keyword");
            });

            modelBuilder.Entity<Level>(entity =>
            {
                entity.HasKey(e => e.Level1)
                    .HasName("pklevel");

                entity.ToTable("Levels", "YokaiToolsGeneral");

                entity.Property(e => e.Level1)
                    .ValueGeneratedNever()
                    .HasColumnName("Level");
            });

            modelBuilder.Entity<Nft>(entity =>
            {
                entity.HasKey(e => new { e.CollectionId, e.TokenId })
                    .HasName("Nft_pk");

                entity.ToTable("Nft", "YokaiToolsGeneral");

                entity.Property(e => e.Name)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.HasOne(d => d.Collection)
                    .WithMany(p => p.Nfts)
                    .HasForeignKey(d => d.CollectionId)
                    .HasConstraintName("Nft_Collections_Id_fk");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("Notification", "YokaiToolsGeneral");

                entity.Property(e => e.Address)
                    .HasMaxLength(42)
                    .IsUnicode(false);

                entity.Property(e => e.Data)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.HasOne(d => d.Event)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.EventId)
                    .HasConstraintName("foreign_key_name");
            });

            modelBuilder.Entity<NotificationId>(entity =>
            {
                entity.ToTable("NotificationId", "YokaiToolsGeneral");

                entity.Property(e => e.Message)
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.Nom)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<OfferListed>(entity =>
            {
                entity.ToTable("OfferListed", "YokaiToolsGeneral");

                entity.HasIndex(e => e.OrderHash, "table_name_OrderHash_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.OrderJson, "table_name_OrderJson_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.Signature, "table_name_Signature_uindex")
                    .IsUnique();

                entity.Property(e => e.CreationDate).HasColumnType("datetime");

                entity.Property(e => e.ExpirationDate).HasColumnType("datetime");

                entity.Property(e => e.Offerer)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.OrderHash)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.OrderJson)
                    .HasMaxLength(2000)
                    .IsUnicode(false);

                entity.Property(e => e.Price)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Receiver)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Signature)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.HasOne(d => d.Collection)
                    .WithMany(p => p.OfferListeds)
                    .HasForeignKey(d => d.CollectionId)
                    .HasConstraintName("OfferListed_Collections_Id_fk");

                entity.HasOne(d => d.Nft)
                    .WithMany(p => p.OfferListeds)
                    .HasForeignKey(d => new { d.CollectionId, d.TokenId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("OfferListed_Nft_CollectionId_TokenId_fk");
            });

            modelBuilder.Entity<OrderFulfilledHistory>(entity =>
            {
                entity.ToTable("OrderFulfilledHistory", "YokaiToolsGeneral");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.OrderJson)
                    .HasMaxLength(2000)
                    .IsUnicode(false);

                entity.Property(e => e.PriceWei)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Seller)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Buyer)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Collection)
                    .WithMany(p => p.OrderFulfilledHistories)
                    .HasForeignKey(d => d.CollectionId)
                    .HasConstraintName("OrderFulfilledHistories_Collections_fk");

                entity.HasOne(d => d.Nft)
                    .WithMany(p => p.OrderFulfilledHistories)
                    .HasForeignKey(d => new { d.CollectionId, d.TokenId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("OrderFulfilledHistory_Nft_CollectionId_TokenId_fk");
            });

            modelBuilder.Entity<Profile>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("Profiles_pk")
                    .IsClustered(false);

                entity.ToTable("Profiles", "YokaiToolsGeneral");

                entity.HasIndex(e => e.Address, "Profiles_Address_uindex")
                    .IsUnique();

                entity.HasIndex(e => new { e.CollectionId, e.TokenId }, "idx_collectionId_tokenId_notnull")
                    .IsUnique()
                    .HasFilter("([TokenId] IS NOT NULL)");

                entity.HasIndex(e => e.Name, "idx_name_notnull")
                    .IsUnique()
                    .HasFilter("([Name] IS NOT NULL)");

                entity.Property(e => e.Activity)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.Address)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Bio)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.DateOfNickname).HasColumnType("datetime");

                entity.Property(e => e.Gender)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.InstagramNickname)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Link)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Localisation)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TwitterHandle)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.HasOne(d => d.Nft)
                    .WithOne(p => p.Profile)
                    .HasForeignKey<Profile>(d => new { d.CollectionId, d.TokenId })
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("Profiles_Nft_CollectionId_TokenId_fk");

                entity.HasMany(d => d.ProfileIdFolloweds)
                    .WithMany(p => p.ProfileIdFollowings)
                    .UsingEntity<Dictionary<string, object>>(
                        "Following",
                        l => l.HasOne<Profile>().WithMany().HasForeignKey("ProfileIdFollowed").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("Following_Profiles_Id_fk_2"),
                        r => r.HasOne<Profile>().WithMany().HasForeignKey("ProfileIdFollowing").HasConstraintName("Following_Profiles_Id_fk"),
                        j =>
                        {
                            j.HasKey("ProfileIdFollowing", "ProfileIdFollowed").HasName("Following_pk");

                            j.ToTable("Following", "YokaiToolsGeneral");
                        });

                entity.HasMany(d => d.ProfileIdFollowings)
                    .WithMany(p => p.ProfileIdFolloweds)
                    .UsingEntity<Dictionary<string, object>>(
                        "Following",
                        l => l.HasOne<Profile>().WithMany().HasForeignKey("ProfileIdFollowing").HasConstraintName("Following_Profiles_Id_fk"),
                        r => r.HasOne<Profile>().WithMany().HasForeignKey("ProfileIdFollowed").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("Following_Profiles_Id_fk_2"),
                        j =>
                        {
                            j.HasKey("ProfileIdFollowing", "ProfileIdFollowed").HasName("Following_pk");

                            j.ToTable("Following", "YokaiToolsGeneral");
                        });
            });

            modelBuilder.Entity<Rarity>(entity =>
            {
                entity.HasKey(e => new { e.CollectionId, e.TokenId })
                    .HasName("Rarity_pk_2");

                entity.ToTable("Rarity", "YokaiToolsGeneral");

                entity.HasOne(d => d.Nft)
                    .WithOne(p => p.Rarity)
                    .HasForeignKey<Rarity>(d => new { d.CollectionId, d.TokenId })
                    .HasConstraintName("Rarity_Nft_CollectionId_TokenId_fk");
            });

            modelBuilder.Entity<RewardRarity>(entity =>
            {
                entity.ToTable("RewardRarity", "YokaiToolsGeneral");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<RewardType>(entity =>
            {
                entity.ToTable("RewardType", "YokaiToolsGeneral");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<RewardValue>(entity =>
            {
                entity.ToTable("RewardValue", "YokaiToolsGeneral");

                entity.Property(e => e.EarnerAddress)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.NftAddress)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Value).HasColumnType("numeric(18, 0)");

                entity.HasOne(d => d.RewardRarityNavigation)
                    .WithMany(p => p.RewardValues)
                    .HasForeignKey(d => d.RewardRarity)
                    .HasConstraintName("RewardValue_Rewardarity_link");

                entity.HasOne(d => d.RewardTypeNavigation)
                    .WithMany(p => p.RewardValues)
                    .HasForeignKey(d => d.RewardType)
                    .HasConstraintName("RewardValue_RewardType_link");
            });

            modelBuilder.Entity<SwapHistory>(entity =>
            {
                entity.ToTable("SwapHistory", "YokaiToolsGeneral");

                entity.Property(e => e.DateOfAcceptation).HasColumnType("datetime");

                entity.Property(e => e.OfferCollection)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Offerer)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.OwnerOfTargeted)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TargetCollection)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<SwapOffer>(entity =>
            {
                entity.ToTable("SwapOffers", "YokaiToolsGeneral");

                entity.HasIndex(e => new { e.TargetCollection, e.TargetTokenId, e.OfferCollection, e.OfferTokenId }, "SwapOffers_TargetCollection_TargetTokenId_OfferCollection_OfferTokenId_uindex")
                    .IsUnique();

                entity.Property(e => e.EndTimestamp).HasColumnName("endTimestamp");

                entity.Property(e => e.OfferCollection)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Offerer)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.OwnerOfTargeted)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TargetCollection)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ToVerifyCollection>(entity =>
            {
                entity.ToTable("ToVerifyCollection", "YokaiToolsGeneral");

                entity.HasIndex(e => e.Address, "ToVerifyCollection_Address_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.Name, "ToVerifyCollection_Name_uindex")
                    .IsUnique();

                entity.Property(e => e.Address)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.AddressOfLister)
                    .HasMaxLength(42)
                    .IsUnicode(false);

                entity.Property(e => e.Banner)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Description)
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.Keywords)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.MintLink)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.ProfilePicture)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ReleaseDate).HasColumnType("date");

                entity.Property(e => e.Twitter)
                    .HasMaxLength(40)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ToVerifyCollectionLinkKeyword>(entity =>
            {
                entity.ToTable("ToVerifyCollection_link_keywords", "YokaiToolsGeneral");

                entity.HasIndex(e => new { e.ToVerifyCollectionId, e.KeywordId }, "ToVerifyCollection_link_keywords_ToVerifyCollection_KeywordId_uindex")
                    .IsUnique();

                entity.HasOne(d => d.Keyword)
                    .WithMany(p => p.ToVerifyCollectionLinkKeywords)
                    .HasForeignKey(d => d.KeywordId)
                    .HasConstraintName("ToVerifyCollection_link_keywords_Keywords_Id_fk");

                entity.HasOne(d => d.ToVerifyCollection)
                    .WithMany(p => p.ToVerifyCollectionLinkKeywords)
                    .HasForeignKey(d => d.ToVerifyCollectionId)
                    .HasConstraintName("ToVerifyCollection_link_keywords_ToVerifyCollection_Id_fk");
            });

            modelBuilder.Entity<YokaiToolsGeneralChestCount>(entity =>
            {
                entity.HasKey(e => new { e.Edition, e.RarityId })
                    .HasName("PK__YokaiToo__6315ED601BFC99D2");

                entity.ToTable("YokaiToolsGeneral.ChestCount");

                entity.HasOne(d => d.Rarity)
                    .WithMany(p => p.YokaiToolsGeneralChestCounts)
                    .HasForeignKey(d => d.RarityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__YokaiToolsGeneral.RewardRarity");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
