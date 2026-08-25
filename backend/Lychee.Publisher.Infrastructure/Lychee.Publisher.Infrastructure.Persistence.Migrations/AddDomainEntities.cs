using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;

namespace Lychee.Publisher.Infrastructure.Persistence.Migrations;

[DbContext(typeof(PublisherDbContext))]
[Migration("20260623191110_AddDomainEntities")]
public class AddDomainEntities : Migration
{
	protected override void Up(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.CreateTable("Payment", (ColumnsBuilder table) => new
		{
			Id = table.Column<Guid>("uniqueidentifier"),
			UserId = table.Column<Guid>("uniqueidentifier"),
			Provider = table.Column<string>("nvarchar(max)"),
			ProviderPaymentId = table.Column<string>("nvarchar(max)"),
			Amount = table.Column<decimal>("decimal(18,2)"),
			Currency = table.Column<string>("nvarchar(max)"),
			Status = table.Column<string>("nvarchar(max)"),
			CreatedAtUtc = table.Column<DateTimeOffset>("datetimeoffset"),
			UpdatedAtUtc = table.Column<DateTimeOffset>("datetimeoffset", null, null, rowVersion: false, null, nullable: true),
			CreatedByUserId = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true),
			UpdatedByUserId = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true)
		}, null, table =>
		{
			table.PrimaryKey("PK_Payment", x => x.Id);
			table.ForeignKey("FK_Payment_Users_UserId", x => x.UserId, "Users", "Id", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
		});
		migrationBuilder.CreateIndex("IX_Payment_UserId", "Payment", "UserId");
	}

	protected override void Down(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.DropTable("Payment");
	}

	protected override void BuildTargetModel(ModelBuilder modelBuilder)
	{
		modelBuilder.HasAnnotation("ProductVersion", "9.0.6").HasAnnotation("Relational:MaxIdentifierLength", 128);
		modelBuilder.UseIdentityColumns(1L);
		modelBuilder.Entity("Lychee.Publisher.Domain.Entities.Payment", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uniqueidentifier");
			b.Property<decimal>("Amount").HasColumnType("decimal(18,2)");
			b.Property<DateTimeOffset>("CreatedAtUtc").HasColumnType("datetimeoffset");
			b.Property<string>("CreatedByUserId").HasColumnType("nvarchar(max)");
			b.Property<string>("Currency").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<string>("Provider").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<string>("ProviderPaymentId").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<string>("Status").IsRequired().HasColumnType("nvarchar(max)");
			b.Property<DateTimeOffset?>("UpdatedAtUtc").HasColumnType("datetimeoffset");
			b.Property<string>("UpdatedByUserId").HasColumnType("nvarchar(max)");
			b.Property<Guid>("UserId").HasColumnType("uniqueidentifier");
			b.HasKey("Id");
			b.HasIndex("UserId");
			b.ToTable("Payment");
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
			b.Navigation("Payments");
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
