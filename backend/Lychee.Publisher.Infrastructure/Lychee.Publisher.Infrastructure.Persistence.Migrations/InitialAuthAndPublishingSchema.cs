using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;

namespace Lychee.Publisher.Infrastructure.Persistence.Migrations;

[DbContext(typeof(PublisherDbContext))]
[Migration("20260623185029_InitialAuthAndPublishingSchema")]
public class InitialAuthAndPublishingSchema : Migration
{
	protected override void Up(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.CreateTable("Users", delegate(ColumnsBuilder table)
		{
			OperationBuilder<AddColumnOperation> id = table.Column<Guid>("uniqueidentifier");
			int? maxLength = 320;
			OperationBuilder<AddColumnOperation> email = table.Column<string>("nvarchar(320)", null, maxLength);
			maxLength = 512;
			OperationBuilder<AddColumnOperation> passwordHash = table.Column<string>("nvarchar(512)", null, maxLength);
			maxLength = 160;
			OperationBuilder<AddColumnOperation> displayName = table.Column<string>("nvarchar(160)", null, maxLength, rowVersion: false, null, nullable: true);
			OperationBuilder<AddColumnOperation> isEmailVerified = table.Column<bool>("bit");
			OperationBuilder<AddColumnOperation> subscriptionTier = table.Column<int>("int");
			maxLength = 64;
			return new
			{
				Id = id,
				Email = email,
				PasswordHash = passwordHash,
				DisplayName = displayName,
				IsEmailVerified = isEmailVerified,
				SubscriptionTier = subscriptionTier,
				Role = table.Column<string>("nvarchar(64)", null, maxLength),
				LastLoginAtUtc = table.Column<DateTimeOffset>("datetimeoffset", null, null, rowVersion: false, null, nullable: true),
				CreatedAtUtc = table.Column<DateTimeOffset>("datetimeoffset"),
				UpdatedAtUtc = table.Column<DateTimeOffset>("datetimeoffset", null, null, rowVersion: false, null, nullable: true),
				CreatedByUserId = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true),
				UpdatedByUserId = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true)
			};
		}, null, table =>
		{
			table.PrimaryKey("PK_Users", x => x.Id);
		});
		migrationBuilder.CreateTable("RefreshTokens", delegate(ColumnsBuilder table)
		{
			OperationBuilder<AddColumnOperation> id = table.Column<Guid>("uniqueidentifier");
			OperationBuilder<AddColumnOperation> userId = table.Column<Guid>("uniqueidentifier");
			int? maxLength = 128;
			OperationBuilder<AddColumnOperation> tokenHash = table.Column<string>("nvarchar(128)", null, maxLength);
			OperationBuilder<AddColumnOperation> expiresAtUtc = table.Column<DateTimeOffset>("datetimeoffset");
			OperationBuilder<AddColumnOperation> revokedAtUtc = table.Column<DateTimeOffset>("datetimeoffset", null, null, rowVersion: false, null, nullable: true);
			maxLength = 128;
			OperationBuilder<AddColumnOperation> replacedByTokenHash = table.Column<string>("nvarchar(128)", null, maxLength, rowVersion: false, null, nullable: true);
			maxLength = 64;
			OperationBuilder<AddColumnOperation> createdByIp = table.Column<string>("nvarchar(64)", null, maxLength, rowVersion: false, null, nullable: true);
			maxLength = 64;
			return new
			{
				Id = id,
				UserId = userId,
				TokenHash = tokenHash,
				ExpiresAtUtc = expiresAtUtc,
				RevokedAtUtc = revokedAtUtc,
				ReplacedByTokenHash = replacedByTokenHash,
				CreatedByIp = createdByIp,
				RevokedByIp = table.Column<string>("nvarchar(64)", null, maxLength, rowVersion: false, null, nullable: true),
				CreatedAtUtc = table.Column<DateTimeOffset>("datetimeoffset"),
				UpdatedAtUtc = table.Column<DateTimeOffset>("datetimeoffset", null, null, rowVersion: false, null, nullable: true),
				CreatedByUserId = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true),
				UpdatedByUserId = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true)
			};
		}, null, table =>
		{
			table.PrimaryKey("PK_RefreshTokens", x => x.Id);
			table.ForeignKey("FK_RefreshTokens_Users_UserId", x => x.UserId, "Users", "Id", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
		});
		migrationBuilder.CreateTable("SocialAccounts", delegate(ColumnsBuilder table)
		{
			OperationBuilder<AddColumnOperation> id = table.Column<Guid>("uniqueidentifier");
			OperationBuilder<AddColumnOperation> userId = table.Column<Guid>("uniqueidentifier");
			OperationBuilder<AddColumnOperation> platform = table.Column<int>("int");
			int? maxLength = 256;
			OperationBuilder<AddColumnOperation> externalAccountId = table.Column<string>("nvarchar(256)", null, maxLength);
			maxLength = 160;
			OperationBuilder<AddColumnOperation> displayName = table.Column<string>("nvarchar(160)", null, maxLength);
			maxLength = 160;
			return new
			{
				Id = id,
				UserId = userId,
				Platform = platform,
				ExternalAccountId = externalAccountId,
				DisplayName = displayName,
				ChannelName = table.Column<string>("nvarchar(160)", null, maxLength, rowVersion: false, null, nullable: true),
				IsActive = table.Column<bool>("bit"),
				CreatedAtUtc = table.Column<DateTimeOffset>("datetimeoffset"),
				UpdatedAtUtc = table.Column<DateTimeOffset>("datetimeoffset", null, null, rowVersion: false, null, nullable: true),
				CreatedByUserId = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true),
				UpdatedByUserId = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true)
			};
		}, null, table =>
		{
			table.PrimaryKey("PK_SocialAccounts", x => x.Id);
			table.ForeignKey("FK_SocialAccounts_Users_UserId", x => x.UserId, "Users", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
		});
		migrationBuilder.CreateTable("Videos", delegate(ColumnsBuilder table)
		{
			OperationBuilder<AddColumnOperation> id = table.Column<Guid>("uniqueidentifier");
			OperationBuilder<AddColumnOperation> userId = table.Column<Guid>("uniqueidentifier");
			int? maxLength = 32;
			OperationBuilder<AddColumnOperation> sourceType = table.Column<string>("nvarchar(32)", null, maxLength);
			maxLength = 2048;
			OperationBuilder<AddColumnOperation> sourceUrl = table.Column<string>("nvarchar(2048)", null, maxLength, rowVersion: false, null, nullable: true);
			maxLength = 2048;
			OperationBuilder<AddColumnOperation> originalFileUri = table.Column<string>("nvarchar(2048)", null, maxLength);
			maxLength = 2048;
			OperationBuilder<AddColumnOperation> thumbnailUri = table.Column<string>("nvarchar(2048)", null, maxLength, rowVersion: false, null, nullable: true);
			maxLength = 280;
			return new
			{
				Id = id,
				UserId = userId,
				SourceType = sourceType,
				SourceUrl = sourceUrl,
				OriginalFileUri = originalFileUri,
				ThumbnailUri = thumbnailUri,
				Title = table.Column<string>("nvarchar(280)", null, maxLength, rowVersion: false, null, nullable: true),
				Duration = table.Column<TimeSpan>("time", null, null, rowVersion: false, null, nullable: true),
				Status = table.Column<int>("int"),
				CreatedAtUtc = table.Column<DateTimeOffset>("datetimeoffset"),
				UpdatedAtUtc = table.Column<DateTimeOffset>("datetimeoffset", null, null, rowVersion: false, null, nullable: true),
				CreatedByUserId = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true),
				UpdatedByUserId = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true)
			};
		}, null, table =>
		{
			table.PrimaryKey("PK_Videos", x => x.Id);
			table.ForeignKey("FK_Videos_Users_UserId", x => x.UserId, "Users", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
		});
		migrationBuilder.CreateTable("Shorts", delegate(ColumnsBuilder table)
		{
			OperationBuilder<AddColumnOperation> id = table.Column<Guid>("uniqueidentifier");
			OperationBuilder<AddColumnOperation> videoId = table.Column<Guid>("uniqueidentifier");
			int? maxLength = 2048;
			OperationBuilder<AddColumnOperation> outputUri = table.Column<string>("nvarchar(2048)", null, maxLength);
			maxLength = 280;
			OperationBuilder<AddColumnOperation> title = table.Column<string>("nvarchar(280)", null, maxLength, rowVersion: false, null, nullable: true);
			OperationBuilder<AddColumnOperation> description = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true);
			maxLength = 1024;
			OperationBuilder<AddColumnOperation> hashtags = table.Column<string>("nvarchar(1024)", null, maxLength, rowVersion: false, null, nullable: true);
			OperationBuilder<AddColumnOperation> captionsJson = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true);
			maxLength = 5;
			int? scale = 4;
			return new
			{
				Id = id,
				VideoId = videoId,
				OutputUri = outputUri,
				Title = title,
				Description = description,
				Hashtags = hashtags,
				CaptionsJson = captionsJson,
				ViralityScore = table.Column<decimal>("decimal(5,4)", null, null, rowVersion: false, null, nullable: false, null, null, null, null, null, null, maxLength, scale),
				StartTime = table.Column<TimeSpan>("time"),
				EndTime = table.Column<TimeSpan>("time"),
				Status = table.Column<int>("int"),
				CreatedAtUtc = table.Column<DateTimeOffset>("datetimeoffset"),
				UpdatedAtUtc = table.Column<DateTimeOffset>("datetimeoffset", null, null, rowVersion: false, null, nullable: true),
				CreatedByUserId = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true),
				UpdatedByUserId = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true)
			};
		}, null, table =>
		{
			table.PrimaryKey("PK_Shorts", x => x.Id);
			table.ForeignKey("FK_Shorts_Videos_VideoId", x => x.VideoId, "Videos", "Id", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
		});
		migrationBuilder.CreateTable("Schedules", delegate(ColumnsBuilder table)
		{
			OperationBuilder<AddColumnOperation> id = table.Column<Guid>("uniqueidentifier");
			OperationBuilder<AddColumnOperation> shortClipId = table.Column<Guid>("uniqueidentifier");
			OperationBuilder<AddColumnOperation> socialAccountId = table.Column<Guid>("uniqueidentifier");
			OperationBuilder<AddColumnOperation> platform = table.Column<int>("int");
			OperationBuilder<AddColumnOperation> publishAtUtc = table.Column<DateTimeOffset>("datetimeoffset");
			OperationBuilder<AddColumnOperation> status = table.Column<int>("int");
			int? maxLength = 256;
			OperationBuilder<AddColumnOperation> externalPostId = table.Column<string>("nvarchar(256)", null, maxLength, rowVersion: false, null, nullable: true);
			maxLength = 1024;
			return new
			{
				Id = id,
				ShortClipId = shortClipId,
				SocialAccountId = socialAccountId,
				Platform = platform,
				PublishAtUtc = publishAtUtc,
				Status = status,
				ExternalPostId = externalPostId,
				FailureReason = table.Column<string>("nvarchar(1024)", null, maxLength, rowVersion: false, null, nullable: true),
				CreatedAtUtc = table.Column<DateTimeOffset>("datetimeoffset"),
				UpdatedAtUtc = table.Column<DateTimeOffset>("datetimeoffset", null, null, rowVersion: false, null, nullable: true),
				CreatedByUserId = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true),
				UpdatedByUserId = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true)
			};
		}, null, table =>
		{
			table.PrimaryKey("PK_Schedules", x => x.Id);
			table.ForeignKey("FK_Schedules_Shorts_ShortClipId", x => x.ShortClipId, "Shorts", "Id", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
			table.ForeignKey("FK_Schedules_SocialAccounts_SocialAccountId", x => x.SocialAccountId, "SocialAccounts", "Id", null, ReferentialAction.NoAction, ReferentialAction.Restrict);
		});
		migrationBuilder.CreateIndex("IX_RefreshTokens_TokenHash", "RefreshTokens", "TokenHash", null, unique: true);
		migrationBuilder.CreateIndex("IX_RefreshTokens_UserId_ExpiresAtUtc", "RefreshTokens", new string[2] { "UserId", "ExpiresAtUtc" });
		migrationBuilder.CreateIndex("IX_Schedules_PublishAtUtc_Status", "Schedules", new string[2] { "PublishAtUtc", "Status" });
		migrationBuilder.CreateIndex("IX_Schedules_ShortClipId", "Schedules", "ShortClipId");
		migrationBuilder.CreateIndex("IX_Schedules_SocialAccountId", "Schedules", "SocialAccountId");
		migrationBuilder.CreateIndex("IX_Shorts_VideoId_Status", "Shorts", new string[2] { "VideoId", "Status" });
		migrationBuilder.CreateIndex("IX_SocialAccounts_UserId_Platform_ExternalAccountId", "SocialAccounts", new string[3] { "UserId", "Platform", "ExternalAccountId" }, null, unique: true);
		migrationBuilder.CreateIndex("IX_Users_Email", "Users", "Email", null, unique: true);
		migrationBuilder.CreateIndex("IX_Videos_UserId_Status", "Videos", new string[2] { "UserId", "Status" });
	}

	protected override void Down(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.DropTable("RefreshTokens");
		migrationBuilder.DropTable("Schedules");
		migrationBuilder.DropTable("Shorts");
		migrationBuilder.DropTable("SocialAccounts");
		migrationBuilder.DropTable("Videos");
		migrationBuilder.DropTable("Users");
	}

	protected override void BuildTargetModel(ModelBuilder modelBuilder)
	{
		modelBuilder.HasAnnotation("ProductVersion", "9.0.6").HasAnnotation("Relational:MaxIdentifierLength", 128);
		modelBuilder.UseIdentityColumns(1L);
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
		modelBuilder.Entity("Lychee.Publisher.Domain.Entities.Video", delegate(EntityTypeBuilder b)
		{
			b.HasOne("Lychee.Publisher.Domain.Entities.User", "User").WithMany("Videos").HasForeignKey("UserId")
				.OnDelete(DeleteBehavior.Restrict)
				.IsRequired();
			b.Navigation("User");
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
			b.Navigation("RefreshTokens");
			b.Navigation("SocialAccounts");
			b.Navigation("Videos");
		});
		modelBuilder.Entity("Lychee.Publisher.Domain.Entities.Video", delegate(EntityTypeBuilder b)
		{
			b.Navigation("Shorts");
		});
	}
}
