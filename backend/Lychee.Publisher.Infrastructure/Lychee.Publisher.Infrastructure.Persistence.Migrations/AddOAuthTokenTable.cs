using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;

namespace Lychee.Publisher.Infrastructure.Persistence.Migrations;

[DbContext(typeof(PublisherDbContext))]
[Migration("20260623204028_AddOAuthTokenTable")]
public class AddOAuthTokenTable : Migration
{
	protected override void Up(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.CreateTable("OAuthTokens", (ColumnsBuilder table) => new
		{
			Id = table.Column<Guid>("uniqueidentifier"),
			SocialAccountId = table.Column<Guid>("uniqueidentifier"),
			AccessTokenCipherText = table.Column<string>("nvarchar(max)"),
			RefreshTokenCipherText = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true),
			ExpiresAtUtc = table.Column<DateTimeOffset>("datetimeoffset", null, null, rowVersion: false, null, nullable: true),
			CreatedAtUtc = table.Column<DateTimeOffset>("datetimeoffset"),
			UpdatedAtUtc = table.Column<DateTimeOffset>("datetimeoffset", null, null, rowVersion: false, null, nullable: true),
			CreatedByUserId = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true),
			UpdatedByUserId = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true)
		}, null, table =>
		{
			table.PrimaryKey("PK_OAuthTokens", x => x.Id);
			table.ForeignKey("FK_OAuthTokens_SocialAccounts_SocialAccountId", x => x.SocialAccountId, "SocialAccounts", "Id", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
		});
		migrationBuilder.CreateIndex("IX_OAuthTokens_SocialAccountId", "OAuthTokens", "SocialAccountId", null, unique: true);
	}

	protected override void Down(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.DropTable("OAuthTokens");
	}

	protected override void BuildTargetModel(ModelBuilder modelBuilder)
	{
		modelBuilder.HasAnnotation("ProductVersion", "9.0.6").HasAnnotation("Relational:MaxIdentifierLength", 128);
		modelBuilder.UseIdentityColumns(1L);
		modelBuilder.Entity("Lychee.Publisher.Domain.Entities.AuditLog", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uniqueidentifier");
			b.Property<string>("Action").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<DateTimeOffset>("CreatedAtUtc").HasColumnType("datetimeoffset");
			b.Property<string>("CreatedByUserId").HasColumnType("nvarchar(max)");
			b.Property<string>("EntityId").HasColumnType("nvarchar(max)");
			b.Property<string>("EntityName").HasColumnType("nvarchar(max)");
			b.Property<string>("MetadataJson").HasColumnType("nvarchar(max)");
			b.Property<DateTimeOffset?>("UpdatedAtUtc").HasColumnType("datetimeoffset");
			b.Property<string>("UpdatedByUserId").HasColumnType("nvarchar(max)");
			b.Property<Guid?>("UserId").HasColumnType("uniqueidentifier");
			b.HasKey("Id");
			b.ToTable("AuditLogs");
		});
		modelBuilder.Entity("Lychee.Publisher.Domain.Entities.EmailVerificationToken", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uniqueidentifier");
			b.Property<DateTimeOffset>("CreatedAtUtc").HasColumnType("datetimeoffset");
			b.Property<string>("CreatedByIp").HasMaxLength(64).HasColumnType("nvarchar(64)");
			b.Property<string>("CreatedByUserId").HasColumnType("nvarchar(max)");
			b.Property<DateTimeOffset>("ExpiresAtUtc").HasColumnType("datetimeoffset");
			b.Property<string>("TokenHash").IsRequired().HasMaxLength(128)
				.HasColumnType("nvarchar(128)");
			b.Property<DateTimeOffset?>("UpdatedAtUtc").HasColumnType("datetimeoffset");
			b.Property<string>("UpdatedByUserId").HasColumnType("nvarchar(max)");
			b.Property<DateTimeOffset?>("UsedAtUtc").HasColumnType("datetimeoffset");
			b.Property<Guid>("UserId").HasColumnType("uniqueidentifier");
			b.HasKey("Id");
			b.HasIndex("TokenHash").IsUnique();
			b.HasIndex("UserId", "ExpiresAtUtc");
			b.ToTable("EmailVerificationTokens", (string?)null);
		});
		modelBuilder.Entity("Lychee.Publisher.Domain.Entities.OAuthToken", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uniqueidentifier");
			b.Property<string>("AccessTokenCipherText").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<DateTimeOffset>("CreatedAtUtc").HasColumnType("datetimeoffset");
			b.Property<string>("CreatedByUserId").HasColumnType("nvarchar(max)");
			b.Property<DateTimeOffset?>("ExpiresAtUtc").HasColumnType("datetimeoffset");
			b.Property<string>("RefreshTokenCipherText").HasColumnType("nvarchar(max)");
			b.Property<Guid>("SocialAccountId").HasColumnType("uniqueidentifier");
			b.Property<DateTimeOffset?>("UpdatedAtUtc").HasColumnType("datetimeoffset");
			b.Property<string>("UpdatedByUserId").HasColumnType("nvarchar(max)");
			b.HasKey("Id");
			b.HasIndex("SocialAccountId").IsUnique();
			b.ToTable("OAuthTokens", (string?)null);
		});
		modelBuilder.Entity("Lychee.Publisher.Domain.Entities.PasswordResetToken", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uniqueidentifier");
			b.Property<DateTimeOffset>("CreatedAtUtc").HasColumnType("datetimeoffset");
			b.Property<string>("CreatedByIp").HasMaxLength(64).HasColumnType("nvarchar(64)");
			b.Property<string>("CreatedByUserId").HasColumnType("nvarchar(max)");
			b.Property<DateTimeOffset>("ExpiresAtUtc").HasColumnType("datetimeoffset");
			b.Property<string>("TokenHash").IsRequired().HasMaxLength(128)
				.HasColumnType("nvarchar(128)");
			b.Property<DateTimeOffset?>("UpdatedAtUtc").HasColumnType("datetimeoffset");
			b.Property<string>("UpdatedByUserId").HasColumnType("nvarchar(max)");
			b.Property<DateTimeOffset?>("UsedAtUtc").HasColumnType("datetimeoffset");
			b.Property<Guid>("UserId").HasColumnType("uniqueidentifier");
			b.HasKey("Id");
			b.HasIndex("TokenHash").IsUnique();
			b.HasIndex("UserId", "ExpiresAtUtc");
			b.ToTable("PasswordResetTokens", (string?)null);
		});
		modelBuilder.Entity("Lychee.Publisher.Domain.Entities.Payment", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uniqueidentifier");
			b.Property<decimal>("Amount").HasColumnType("decimal(18,2)");
			b.Property<DateTimeOffset>("CreatedAtUtc").HasColumnType("datetimeoffset");
			b.Property<string>("CreatedByUserId").HasColumnType("nvarchar(max)");
			b.Property<string>("Currency").IsRequired().HasMaxLength(10)
				.HasColumnType("nvarchar(10)");
			b.Property<string>("Provider").IsRequired().HasMaxLength(50)
				.HasColumnType("nvarchar(50)");
			b.Property<string>("ProviderPaymentId").IsRequired().HasMaxLength(100)
				.HasColumnType("nvarchar(100)");
			b.Property<string>("Status").IsRequired().HasMaxLength(50)
				.HasColumnType("nvarchar(50)");
			b.Property<DateTimeOffset?>("UpdatedAtUtc").HasColumnType("datetimeoffset");
			b.Property<string>("UpdatedByUserId").HasColumnType("nvarchar(max)");
			b.Property<Guid>("UserId").HasColumnType("uniqueidentifier");
			b.HasKey("Id");
			b.HasIndex("UserId");
			b.ToTable("Payments");
		});
		modelBuilder.Entity("Lychee.Publisher.Domain.Entities.Permission", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uniqueidentifier");
			b.Property<DateTimeOffset>("CreatedAtUtc").HasColumnType("datetimeoffset");
			b.Property<string>("Description").HasMaxLength(280).HasColumnType("nvarchar(280)");
			b.Property<string>("Name").IsRequired().HasMaxLength(120)
				.HasColumnType("nvarchar(120)");
			b.HasKey("Id");
			b.HasIndex("Name").IsUnique();
			b.ToTable("Permissions", (string?)null);
			b.HasData(new
			{
				Id = new Guid("33333333-3333-3333-3333-333333333333"),
				CreatedAtUtc = new DateTimeOffset(new DateTime(2026, 6, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
				Description = "Create and process videos.",
				Name = "videos.manage"
			}, new
			{
				Id = new Guid("44444444-4444-4444-4444-444444444444"),
				CreatedAtUtc = new DateTimeOffset(new DateTime(2026, 6, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
				Description = "Create and manage publishing schedules.",
				Name = "schedules.manage"
			}, new
			{
				Id = new Guid("55555555-5555-5555-5555-555555555555"),
				CreatedAtUtc = new DateTimeOffset(new DateTime(2026, 6, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
				Description = "Access administrative features.",
				Name = "admin.manage"
			});
		});
		modelBuilder.Entity("Lychee.Publisher.Domain.Entities.PublishingSchedule", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uniqueidentifier");
			b.Property<DateTimeOffset>("CreatedAtUtc").HasColumnType("datetimeoffset");
			b.Property<string>("CreatedByUserId").HasColumnType("nvarchar(max)");
			b.Property<string>("ExternalPostId").HasMaxLength(256).HasColumnType("nvarchar(256)");
			b.Property<string>("FailureReason").HasMaxLength(1024).HasColumnType("nvarchar(1024)");
			b.Property<int>("Platform").HasColumnType("int");
			b.Property<DateTimeOffset>("PublishAtUtc").HasColumnType("datetimeoffset");
			b.Property<Guid>("ShortClipId").HasColumnType("uniqueidentifier");
			b.Property<Guid>("SocialAccountId").HasColumnType("uniqueidentifier");
			b.Property<int>("Status").HasColumnType("int");
			b.Property<DateTimeOffset?>("UpdatedAtUtc").HasColumnType("datetimeoffset");
			b.Property<string>("UpdatedByUserId").HasColumnType("nvarchar(max)");
			b.HasKey("Id");
			b.HasIndex("ShortClipId");
			b.HasIndex("SocialAccountId");
			b.HasIndex("PublishAtUtc", "Status");
			b.ToTable("Schedules", (string?)null);
		});
		modelBuilder.Entity("Lychee.Publisher.Domain.Entities.RefreshToken", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uniqueidentifier");
			b.Property<DateTimeOffset>("CreatedAtUtc").HasColumnType("datetimeoffset");
			b.Property<string>("CreatedByIp").HasMaxLength(64).HasColumnType("nvarchar(64)");
			b.Property<string>("CreatedByUserId").HasColumnType("nvarchar(max)");
			b.Property<DateTimeOffset>("ExpiresAtUtc").HasColumnType("datetimeoffset");
			b.Property<string>("ReplacedByTokenHash").HasMaxLength(128).HasColumnType("nvarchar(128)");
			b.Property<DateTimeOffset?>("RevokedAtUtc").HasColumnType("datetimeoffset");
			b.Property<string>("RevokedByIp").HasMaxLength(64).HasColumnType("nvarchar(64)");
			b.Property<string>("TokenHash").IsRequired().HasMaxLength(128)
				.HasColumnType("nvarchar(128)");
			b.Property<DateTimeOffset?>("UpdatedAtUtc").HasColumnType("datetimeoffset");
			b.Property<string>("UpdatedByUserId").HasColumnType("nvarchar(max)");
			b.Property<Guid>("UserId").HasColumnType("uniqueidentifier");
			b.HasKey("Id");
			b.HasIndex("TokenHash").IsUnique();
			b.HasIndex("UserId", "ExpiresAtUtc");
			b.ToTable("RefreshTokens", (string?)null);
		});
		modelBuilder.Entity("Lychee.Publisher.Domain.Entities.Role", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uniqueidentifier");
			b.Property<DateTimeOffset>("CreatedAtUtc").HasColumnType("datetimeoffset");
			b.Property<string>("Name").IsRequired().HasMaxLength(64)
				.HasColumnType("nvarchar(64)");
			b.HasKey("Id");
			b.HasIndex("Name").IsUnique();
			b.ToTable("Roles", (string?)null);
			b.HasData(new
			{
				Id = new Guid("11111111-1111-1111-1111-111111111111"),
				CreatedAtUtc = new DateTimeOffset(new DateTime(2026, 6, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
				Name = "User"
			}, new
			{
				Id = new Guid("22222222-2222-2222-2222-222222222222"),
				CreatedAtUtc = new DateTimeOffset(new DateTime(2026, 6, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
				Name = "Admin"
			});
		});
		modelBuilder.Entity("Lychee.Publisher.Domain.Entities.RolePermission", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("RoleId").HasColumnType("uniqueidentifier");
			b.Property<Guid>("PermissionId").HasColumnType("uniqueidentifier");
			b.HasKey("RoleId", "PermissionId");
			b.HasIndex("PermissionId");
			b.ToTable("RolePermissions", (string?)null);
			b.HasData(new
			{
				RoleId = new Guid("11111111-1111-1111-1111-111111111111"),
				PermissionId = new Guid("33333333-3333-3333-3333-333333333333")
			}, new
			{
				RoleId = new Guid("11111111-1111-1111-1111-111111111111"),
				PermissionId = new Guid("44444444-4444-4444-4444-444444444444")
			}, new
			{
				RoleId = new Guid("22222222-2222-2222-2222-222222222222"),
				PermissionId = new Guid("33333333-3333-3333-3333-333333333333")
			}, new
			{
				RoleId = new Guid("22222222-2222-2222-2222-222222222222"),
				PermissionId = new Guid("44444444-4444-4444-4444-444444444444")
			}, new
			{
				RoleId = new Guid("22222222-2222-2222-2222-222222222222"),
				PermissionId = new Guid("55555555-5555-5555-5555-555555555555")
			});
		});
		modelBuilder.Entity("Lychee.Publisher.Domain.Entities.ShortClip", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uniqueidentifier");
			b.Property<string>("CaptionsJson").HasColumnType("nvarchar(max)");
			b.Property<DateTimeOffset>("CreatedAtUtc").HasColumnType("datetimeoffset");
			b.Property<string>("CreatedByUserId").HasColumnType("nvarchar(max)");
			b.Property<string>("Description").HasColumnType("nvarchar(max)");
			b.Property<TimeSpan>("EndTime").HasColumnType("time");
			b.Property<string>("Hashtags").HasMaxLength(1024).HasColumnType("nvarchar(1024)");
			b.Property<string>("OutputUri").IsRequired().HasMaxLength(2048)
				.HasColumnType("nvarchar(2048)");
			b.Property<TimeSpan>("StartTime").HasColumnType("time");
			b.Property<int>("Status").HasColumnType("int");
			b.Property<string>("Title").HasMaxLength(280).HasColumnType("nvarchar(280)");
			b.Property<DateTimeOffset?>("UpdatedAtUtc").HasColumnType("datetimeoffset");
			b.Property<string>("UpdatedByUserId").HasColumnType("nvarchar(max)");
			b.Property<Guid>("VideoId").HasColumnType("uniqueidentifier");
			b.Property<decimal>("ViralityScore").HasPrecision(5, 4).HasColumnType("decimal(5,4)");
			b.HasKey("Id");
			b.HasIndex("VideoId", "Status");
			b.ToTable("Shorts", (string?)null);
		});
		modelBuilder.Entity("Lychee.Publisher.Domain.Entities.SocialAccount", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uniqueidentifier");
			b.Property<string>("ChannelName").HasMaxLength(160).HasColumnType("nvarchar(160)");
			b.Property<DateTimeOffset>("CreatedAtUtc").HasColumnType("datetimeoffset");
			b.Property<string>("CreatedByUserId").HasColumnType("nvarchar(max)");
			b.Property<string>("DisplayName").IsRequired().HasMaxLength(160)
				.HasColumnType("nvarchar(160)");
			b.Property<string>("ExternalAccountId").IsRequired().HasMaxLength(256)
				.HasColumnType("nvarchar(256)");
			b.Property<bool>("IsActive").HasColumnType("bit");
			b.Property<int>("Platform").HasColumnType("int");
			b.Property<DateTimeOffset?>("UpdatedAtUtc").HasColumnType("datetimeoffset");
			b.Property<string>("UpdatedByUserId").HasColumnType("nvarchar(max)");
			b.Property<Guid>("UserId").HasColumnType("uniqueidentifier");
			b.HasKey("Id");
			b.HasIndex("UserId", "Platform", "ExternalAccountId").IsUnique();
			b.ToTable("SocialAccounts", (string?)null);
		});
		modelBuilder.Entity("Lychee.Publisher.Domain.Entities.SubscriptionPlan", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uniqueidentifier");
			b.Property<DateTimeOffset>("CreatedAtUtc").HasColumnType("datetimeoffset");
			b.Property<decimal>("MonthlyPrice").HasColumnType("decimal(18,2)");
			b.Property<int>("MonthlyVideoLimit").HasColumnType("int");
			b.Property<string>("Name").IsRequired().HasMaxLength(100)
				.HasColumnType("nvarchar(100)");
			b.Property<int>("Tier").HasColumnType("int");
			b.Property<decimal>("YearlyPrice").HasColumnType("decimal(18,2)");
			b.HasKey("Id");
			b.ToTable("SubscriptionPlans");
			b.HasData(new
			{
				Id = new Guid("11111111-1111-1111-1111-111111111111"),
				CreatedAtUtc = new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
				MonthlyPrice = 0.00m,
				MonthlyVideoLimit = 5,
				Name = "Free",
				Tier = 1,
				YearlyPrice = 0.00m
			}, new
			{
				Id = new Guid("22222222-2222-2222-2222-222222222222"),
				CreatedAtUtc = new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
				MonthlyPrice = 499.00m,
				MonthlyVideoLimit = 50,
				Name = "Pro",
				Tier = 2,
				YearlyPrice = 4990.00m
			}, new
			{
				Id = new Guid("33333333-3333-3333-3333-333333333333"),
				CreatedAtUtc = new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
				MonthlyPrice = 1999.00m,
				MonthlyVideoLimit = 200,
				Name = "Agency",
				Tier = 3,
				YearlyPrice = 19990.00m
			}, new
			{
				Id = new Guid("44444444-4444-4444-4444-444444444444"),
				CreatedAtUtc = new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
				MonthlyPrice = 9999.00m,
				MonthlyVideoLimit = 5000,
				Name = "Enterprise",
				Tier = 4,
				YearlyPrice = 99990.00m
			});
		});
		modelBuilder.Entity("Lychee.Publisher.Domain.Entities.User", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uniqueidentifier");
			b.Property<DateTimeOffset>("CreatedAtUtc").HasColumnType("datetimeoffset");
			b.Property<string>("CreatedByUserId").HasColumnType("nvarchar(max)");
			b.Property<string>("DisplayName").HasMaxLength(160).HasColumnType("nvarchar(160)");
			b.Property<string>("Email").IsRequired().HasMaxLength(320)
				.HasColumnType("nvarchar(320)");
			b.Property<bool>("IsEmailVerified").HasColumnType("bit");
			b.Property<DateTimeOffset?>("LastLoginAtUtc").HasColumnType("datetimeoffset");
			b.Property<string>("PasswordHash").IsRequired().HasMaxLength(512)
				.HasColumnType("nvarchar(512)");
			b.Property<string>("Role").IsRequired().HasMaxLength(64)
				.HasColumnType("nvarchar(64)");
			b.Property<int>("SubscriptionTier").HasColumnType("int");
			b.Property<DateTimeOffset?>("UpdatedAtUtc").HasColumnType("datetimeoffset");
			b.Property<string>("UpdatedByUserId").HasColumnType("nvarchar(max)");
			b.HasKey("Id");
			b.HasIndex("Email").IsUnique();
			b.ToTable("Users", (string?)null);
		});
		modelBuilder.Entity("Lychee.Publisher.Domain.Entities.UserSession", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uniqueidentifier");
			b.Property<DateTimeOffset>("CreatedAtUtc").HasColumnType("datetimeoffset");
			b.Property<string>("CreatedByUserId").HasColumnType("nvarchar(max)");
			b.Property<string>("IpAddress").HasMaxLength(64).HasColumnType("nvarchar(64)");
			b.Property<DateTimeOffset>("LastSeenAtUtc").HasColumnType("datetimeoffset");
			b.Property<DateTimeOffset?>("RevokedAtUtc").HasColumnType("datetimeoffset");
			b.Property<DateTimeOffset?>("UpdatedAtUtc").HasColumnType("datetimeoffset");
			b.Property<string>("UpdatedByUserId").HasColumnType("nvarchar(max)");
			b.Property<string>("UserAgent").HasMaxLength(512).HasColumnType("nvarchar(512)");
			b.Property<Guid>("UserId").HasColumnType("uniqueidentifier");
			b.HasKey("Id");
			b.HasIndex("UserId", "LastSeenAtUtc");
			b.ToTable("UserSessions", (string?)null);
		});
		modelBuilder.Entity("Lychee.Publisher.Domain.Entities.Video", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uniqueidentifier");
			b.Property<DateTimeOffset>("CreatedAtUtc").HasColumnType("datetimeoffset");
			b.Property<string>("CreatedByUserId").HasColumnType("nvarchar(max)");
			b.Property<TimeSpan?>("Duration").HasColumnType("time");
			b.Property<string>("OriginalFileUri").IsRequired().HasMaxLength(2048)
				.HasColumnType("nvarchar(2048)");
			b.Property<string>("SourceType").IsRequired().HasMaxLength(32)
				.HasColumnType("nvarchar(32)");
			b.Property<string>("SourceUrl").HasMaxLength(2048).HasColumnType("nvarchar(2048)");
			b.Property<int>("Status").HasColumnType("int");
			b.Property<string>("ThumbnailUri").HasMaxLength(2048).HasColumnType("nvarchar(2048)");
			b.Property<string>("Title").HasMaxLength(280).HasColumnType("nvarchar(280)");
			b.Property<DateTimeOffset?>("UpdatedAtUtc").HasColumnType("datetimeoffset");
			b.Property<string>("UpdatedByUserId").HasColumnType("nvarchar(max)");
			b.Property<Guid>("UserId").HasColumnType("uniqueidentifier");
			b.HasKey("Id");
			b.HasIndex("UserId", "Status");
			b.ToTable("Videos", (string?)null);
		});
		modelBuilder.Entity("Lychee.Publisher.Domain.Entities.EmailVerificationToken", delegate(EntityTypeBuilder b)
		{
			b.HasOne("Lychee.Publisher.Domain.Entities.User", "User").WithMany("EmailVerificationTokens").HasForeignKey("UserId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
			b.Navigation("User");
		});
		modelBuilder.Entity("Lychee.Publisher.Domain.Entities.OAuthToken", delegate(EntityTypeBuilder b)
		{
			b.HasOne("Lychee.Publisher.Domain.Entities.SocialAccount", "SocialAccount").WithOne().HasForeignKey("Lychee.Publisher.Domain.Entities.OAuthToken", "SocialAccountId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
			b.Navigation("SocialAccount");
		});
		modelBuilder.Entity("Lychee.Publisher.Domain.Entities.PasswordResetToken", delegate(EntityTypeBuilder b)
		{
			b.HasOne("Lychee.Publisher.Domain.Entities.User", "User").WithMany("PasswordResetTokens").HasForeignKey("UserId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
			b.Navigation("User");
		});
		modelBuilder.Entity("Lychee.Publisher.Domain.Entities.Payment", delegate(EntityTypeBuilder b)
		{
			b.HasOne("Lychee.Publisher.Domain.Entities.User", "User").WithMany("Payments").HasForeignKey("UserId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
			b.Navigation("User");
		});
		modelBuilder.Entity("Lychee.Publisher.Domain.Entities.PublishingSchedule", delegate(EntityTypeBuilder b)
		{
			b.HasOne("Lychee.Publisher.Domain.Entities.ShortClip", "ShortClip").WithMany("Schedules").HasForeignKey("ShortClipId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
			b.HasOne("Lychee.Publisher.Domain.Entities.SocialAccount", "SocialAccount").WithMany("Schedules").HasForeignKey("SocialAccountId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.Navigation("ShortClip");
			b.Navigation("SocialAccount");
		});
		modelBuilder.Entity("Lychee.Publisher.Domain.Entities.RefreshToken", delegate(EntityTypeBuilder b)
		{
			b.HasOne("Lychee.Publisher.Domain.Entities.User", "User").WithMany("RefreshTokens").HasForeignKey("UserId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
			b.Navigation("User");
		});
		modelBuilder.Entity("Lychee.Publisher.Domain.Entities.RolePermission", delegate(EntityTypeBuilder b)
		{
			b.HasOne("Lychee.Publisher.Domain.Entities.Permission", "Permission").WithMany("RolePermissions").HasForeignKey("PermissionId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
			b.HasOne("Lychee.Publisher.Domain.Entities.Role", "Role").WithMany("RolePermissions").HasForeignKey("RoleId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
			b.Navigation("Permission");
			b.Navigation("Role");
		});
		modelBuilder.Entity("Lychee.Publisher.Domain.Entities.ShortClip", delegate(EntityTypeBuilder b)
		{
			b.HasOne("Lychee.Publisher.Domain.Entities.Video", "Video").WithMany("Shorts").HasForeignKey("VideoId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
			b.Navigation("Video");
		});
		modelBuilder.Entity("Lychee.Publisher.Domain.Entities.SocialAccount", delegate(EntityTypeBuilder b)
		{
			b.HasOne("Lychee.Publisher.Domain.Entities.User", "User").WithMany("SocialAccounts").HasForeignKey("UserId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.Navigation("User");
		});
		modelBuilder.Entity("Lychee.Publisher.Domain.Entities.UserSession", delegate(EntityTypeBuilder b)
		{
			b.HasOne("Lychee.Publisher.Domain.Entities.User", "User").WithMany("Sessions").HasForeignKey("UserId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired();
			b.Navigation("User");
		});
		modelBuilder.Entity("Lychee.Publisher.Domain.Entities.Video", delegate(EntityTypeBuilder b)
		{
			b.HasOne("Lychee.Publisher.Domain.Entities.User", "User").WithMany("Videos").HasForeignKey("UserId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.Navigation("User");
		});
		modelBuilder.Entity("Lychee.Publisher.Domain.Entities.Permission", delegate(EntityTypeBuilder b)
		{
			b.Navigation("RolePermissions");
		});
		modelBuilder.Entity("Lychee.Publisher.Domain.Entities.Role", delegate(EntityTypeBuilder b)
		{
			b.Navigation("RolePermissions");
		});
		modelBuilder.Entity("Lychee.Publisher.Domain.Entities.ShortClip", delegate(EntityTypeBuilder b)
		{
			b.Navigation("Schedules");
		});
		modelBuilder.Entity("Lychee.Publisher.Domain.Entities.SocialAccount", delegate(EntityTypeBuilder b)
		{
			b.Navigation("Schedules");
		});
		modelBuilder.Entity("Lychee.Publisher.Domain.Entities.User", delegate(EntityTypeBuilder b)
		{
			b.Navigation("EmailVerificationTokens");
			b.Navigation("PasswordResetTokens");
			b.Navigation("Payments");
			b.Navigation("RefreshTokens");
			b.Navigation("Sessions");
			b.Navigation("SocialAccounts");
			b.Navigation("Videos");
		});
		modelBuilder.Entity("Lychee.Publisher.Domain.Entities.Video", delegate(EntityTypeBuilder b)
		{
			b.Navigation("Shorts");
		});
	}
}
