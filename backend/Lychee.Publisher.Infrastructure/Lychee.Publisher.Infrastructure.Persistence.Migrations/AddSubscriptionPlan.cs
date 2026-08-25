using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;

namespace Lychee.Publisher.Infrastructure.Persistence.Migrations;

[DbContext(typeof(PublisherDbContext))]
[Migration("20260623201252_AddSubscriptionPlan")]
public class AddSubscriptionPlan : Migration
{
	protected override void Up(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.DropForeignKey("FK_Payment_Users_UserId", "Payment");
		migrationBuilder.DropPrimaryKey("PK_Payment", "Payment");
		migrationBuilder.RenameTable("Payment", null, "Payments");
		migrationBuilder.RenameIndex("IX_Payment_UserId", "IX_Payments_UserId", "Payments");
		migrationBuilder.AddPrimaryKey("PK_Payments", "Payments", "Id");
		migrationBuilder.CreateTable("AuditLogs", (ColumnsBuilder table) => new
		{
			Id = table.Column<Guid>("uniqueidentifier"),
			UserId = table.Column<Guid>("uniqueidentifier", null, null, rowVersion: false, null, nullable: true),
			Action = table.Column<string>("nvarchar(max)"),
			EntityName = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true),
			EntityId = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true),
			MetadataJson = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true),
			CreatedAtUtc = table.Column<DateTimeOffset>("datetimeoffset"),
			UpdatedAtUtc = table.Column<DateTimeOffset>("datetimeoffset", null, null, rowVersion: false, null, nullable: true),
			CreatedByUserId = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true),
			UpdatedByUserId = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true)
		}, null, table =>
		{
			table.PrimaryKey("PK_AuditLogs", x => x.Id);
		});
		migrationBuilder.CreateTable("EmailVerificationTokens", delegate(ColumnsBuilder table)
		{
			OperationBuilder<AddColumnOperation> id = table.Column<Guid>("uniqueidentifier");
			OperationBuilder<AddColumnOperation> userId = table.Column<Guid>("uniqueidentifier");
			int? maxLength = 128;
			OperationBuilder<AddColumnOperation> tokenHash = table.Column<string>("nvarchar(128)", null, maxLength);
			OperationBuilder<AddColumnOperation> expiresAtUtc = table.Column<DateTimeOffset>("datetimeoffset");
			OperationBuilder<AddColumnOperation> usedAtUtc = table.Column<DateTimeOffset>("datetimeoffset", null, null, rowVersion: false, null, nullable: true);
			maxLength = 64;
			return new
			{
				Id = id,
				UserId = userId,
				TokenHash = tokenHash,
				ExpiresAtUtc = expiresAtUtc,
				UsedAtUtc = usedAtUtc,
				CreatedByIp = table.Column<string>("nvarchar(64)", null, maxLength, rowVersion: false, null, nullable: true),
				CreatedAtUtc = table.Column<DateTimeOffset>("datetimeoffset"),
				UpdatedAtUtc = table.Column<DateTimeOffset>("datetimeoffset", null, null, rowVersion: false, null, nullable: true),
				CreatedByUserId = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true),
				UpdatedByUserId = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true)
			};
		}, null, table =>
		{
			table.PrimaryKey("PK_EmailVerificationTokens", x => x.Id);
			table.ForeignKey("FK_EmailVerificationTokens_Users_UserId", x => x.UserId, "Users", "Id", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
		});
		migrationBuilder.CreateTable("PasswordResetTokens", delegate(ColumnsBuilder table)
		{
			OperationBuilder<AddColumnOperation> id = table.Column<Guid>("uniqueidentifier");
			OperationBuilder<AddColumnOperation> userId = table.Column<Guid>("uniqueidentifier");
			int? maxLength = 128;
			OperationBuilder<AddColumnOperation> tokenHash = table.Column<string>("nvarchar(128)", null, maxLength);
			OperationBuilder<AddColumnOperation> expiresAtUtc = table.Column<DateTimeOffset>("datetimeoffset");
			OperationBuilder<AddColumnOperation> usedAtUtc = table.Column<DateTimeOffset>("datetimeoffset", null, null, rowVersion: false, null, nullable: true);
			maxLength = 64;
			return new
			{
				Id = id,
				UserId = userId,
				TokenHash = tokenHash,
				ExpiresAtUtc = expiresAtUtc,
				UsedAtUtc = usedAtUtc,
				CreatedByIp = table.Column<string>("nvarchar(64)", null, maxLength, rowVersion: false, null, nullable: true),
				CreatedAtUtc = table.Column<DateTimeOffset>("datetimeoffset"),
				UpdatedAtUtc = table.Column<DateTimeOffset>("datetimeoffset", null, null, rowVersion: false, null, nullable: true),
				CreatedByUserId = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true),
				UpdatedByUserId = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true)
			};
		}, null, table =>
		{
			table.PrimaryKey("PK_PasswordResetTokens", x => x.Id);
			table.ForeignKey("FK_PasswordResetTokens_Users_UserId", x => x.UserId, "Users", "Id", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
		});
		migrationBuilder.CreateTable("Permissions", delegate(ColumnsBuilder table)
		{
			OperationBuilder<AddColumnOperation> id = table.Column<Guid>("uniqueidentifier");
			int? maxLength = 120;
			OperationBuilder<AddColumnOperation> name = table.Column<string>("nvarchar(120)", null, maxLength);
			maxLength = 280;
			return new
			{
				Id = id,
				Name = name,
				Description = table.Column<string>("nvarchar(280)", null, maxLength, rowVersion: false, null, nullable: true),
				CreatedAtUtc = table.Column<DateTimeOffset>("datetimeoffset")
			};
		}, null, table =>
		{
			table.PrimaryKey("PK_Permissions", x => x.Id);
		});
		migrationBuilder.CreateTable("Roles", delegate(ColumnsBuilder table)
		{
			OperationBuilder<AddColumnOperation> id = table.Column<Guid>("uniqueidentifier");
			int? maxLength = 64;
			return new
			{
				Id = id,
				Name = table.Column<string>("nvarchar(64)", null, maxLength),
				CreatedAtUtc = table.Column<DateTimeOffset>("datetimeoffset")
			};
		}, null, table =>
		{
			table.PrimaryKey("PK_Roles", x => x.Id);
		});
		migrationBuilder.CreateTable("SubscriptionPlans", delegate(ColumnsBuilder table)
		{
			OperationBuilder<AddColumnOperation> id = table.Column<Guid>("uniqueidentifier");
			OperationBuilder<AddColumnOperation> tier = table.Column<int>("int");
			int? maxLength = 100;
			return new
			{
				Id = id,
				Tier = tier,
				Name = table.Column<string>("nvarchar(100)", null, maxLength),
				MonthlyPrice = table.Column<decimal>("decimal(18,2)"),
				YearlyPrice = table.Column<decimal>("decimal(18,2)"),
				MonthlyVideoLimit = table.Column<int>("int"),
				CreatedAtUtc = table.Column<DateTimeOffset>("datetimeoffset")
			};
		}, null, table =>
		{
			table.PrimaryKey("PK_SubscriptionPlans", x => x.Id);
		});
		migrationBuilder.CreateTable("UserSessions", delegate(ColumnsBuilder table)
		{
			OperationBuilder<AddColumnOperation> id = table.Column<Guid>("uniqueidentifier");
			OperationBuilder<AddColumnOperation> userId = table.Column<Guid>("uniqueidentifier");
			int? maxLength = 64;
			OperationBuilder<AddColumnOperation> ipAddress = table.Column<string>("nvarchar(64)", null, maxLength, rowVersion: false, null, nullable: true);
			maxLength = 512;
			return new
			{
				Id = id,
				UserId = userId,
				IpAddress = ipAddress,
				UserAgent = table.Column<string>("nvarchar(512)", null, maxLength, rowVersion: false, null, nullable: true),
				LastSeenAtUtc = table.Column<DateTimeOffset>("datetimeoffset"),
				RevokedAtUtc = table.Column<DateTimeOffset>("datetimeoffset", null, null, rowVersion: false, null, nullable: true),
				CreatedAtUtc = table.Column<DateTimeOffset>("datetimeoffset"),
				UpdatedAtUtc = table.Column<DateTimeOffset>("datetimeoffset", null, null, rowVersion: false, null, nullable: true),
				CreatedByUserId = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true),
				UpdatedByUserId = table.Column<string>("nvarchar(max)", null, null, rowVersion: false, null, nullable: true)
			};
		}, null, table =>
		{
			table.PrimaryKey("PK_UserSessions", x => x.Id);
			table.ForeignKey("FK_UserSessions_Users_UserId", x => x.UserId, "Users", "Id", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
		});
		migrationBuilder.CreateTable("RolePermissions", (ColumnsBuilder table) => new
		{
			RoleId = table.Column<Guid>("uniqueidentifier"),
			PermissionId = table.Column<Guid>("uniqueidentifier")
		}, null, table =>
		{
			table.PrimaryKey("PK_RolePermissions", x => new { x.RoleId, x.PermissionId });
			table.ForeignKey("FK_RolePermissions_Permissions_PermissionId", x => x.PermissionId, "Permissions", "Id", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
			table.ForeignKey("FK_RolePermissions_Roles_RoleId", x => x.RoleId, "Roles", "Id", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
		});
		migrationBuilder.InsertData("Permissions", new string[4] { "Id", "CreatedAtUtc", "Description", "Name" }, new object[3, 4]
		{
			{
				new Guid("33333333-3333-3333-3333-333333333333"),
				new DateTimeOffset(new DateTime(2026, 6, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
				"Create and process videos.",
				"videos.manage"
			},
			{
				new Guid("44444444-4444-4444-4444-444444444444"),
				new DateTimeOffset(new DateTime(2026, 6, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
				"Create and manage publishing schedules.",
				"schedules.manage"
			},
			{
				new Guid("55555555-5555-5555-5555-555555555555"),
				new DateTimeOffset(new DateTime(2026, 6, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
				"Access administrative features.",
				"admin.manage"
			}
		});
		migrationBuilder.InsertData("Roles", new string[3] { "Id", "CreatedAtUtc", "Name" }, new object[2, 3]
		{
			{
				new Guid("11111111-1111-1111-1111-111111111111"),
				new DateTimeOffset(new DateTime(2026, 6, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
				"User"
			},
			{
				new Guid("22222222-2222-2222-2222-222222222222"),
				new DateTimeOffset(new DateTime(2026, 6, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
				"Admin"
			}
		});
		migrationBuilder.InsertData("SubscriptionPlans", new string[7] { "Id", "CreatedAtUtc", "MonthlyPrice", "MonthlyVideoLimit", "Name", "Tier", "YearlyPrice" }, new object[4, 7]
		{
			{
				new Guid("11111111-1111-1111-1111-111111111111"),
				new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
				0.00m,
				5,
				"Free",
				1,
				0.00m
			},
			{
				new Guid("22222222-2222-2222-2222-222222222222"),
				new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
				499.00m,
				50,
				"Pro",
				2,
				4990.00m
			},
			{
				new Guid("33333333-3333-3333-3333-333333333333"),
				new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
				1999.00m,
				200,
				"Agency",
				3,
				19990.00m
			},
			{
				new Guid("44444444-4444-4444-4444-444444444444"),
				new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
				9999.00m,
				5000,
				"Enterprise",
				4,
				99990.00m
			}
		});
		migrationBuilder.InsertData("RolePermissions", new string[2] { "PermissionId", "RoleId" }, new object[5, 2]
		{
			{
				new Guid("33333333-3333-3333-3333-333333333333"),
				new Guid("11111111-1111-1111-1111-111111111111")
			},
			{
				new Guid("44444444-4444-4444-4444-444444444444"),
				new Guid("11111111-1111-1111-1111-111111111111")
			},
			{
				new Guid("33333333-3333-3333-3333-333333333333"),
				new Guid("22222222-2222-2222-2222-222222222222")
			},
			{
				new Guid("44444444-4444-4444-4444-444444444444"),
				new Guid("22222222-2222-2222-2222-222222222222")
			},
			{
				new Guid("55555555-5555-5555-5555-555555555555"),
				new Guid("22222222-2222-2222-2222-222222222222")
			}
		});
		migrationBuilder.CreateIndex("IX_EmailVerificationTokens_TokenHash", "EmailVerificationTokens", "TokenHash", null, unique: true);
		migrationBuilder.CreateIndex("IX_EmailVerificationTokens_UserId_ExpiresAtUtc", "EmailVerificationTokens", new string[2] { "UserId", "ExpiresAtUtc" });
		migrationBuilder.CreateIndex("IX_PasswordResetTokens_TokenHash", "PasswordResetTokens", "TokenHash", null, unique: true);
		migrationBuilder.CreateIndex("IX_PasswordResetTokens_UserId_ExpiresAtUtc", "PasswordResetTokens", new string[2] { "UserId", "ExpiresAtUtc" });
		migrationBuilder.CreateIndex("IX_Permissions_Name", "Permissions", "Name", null, unique: true);
		migrationBuilder.CreateIndex("IX_RolePermissions_PermissionId", "RolePermissions", "PermissionId");
		migrationBuilder.CreateIndex("IX_Roles_Name", "Roles", "Name", null, unique: true);
		migrationBuilder.CreateIndex("IX_UserSessions_UserId_LastSeenAtUtc", "UserSessions", new string[2] { "UserId", "LastSeenAtUtc" });
		migrationBuilder.AddForeignKey("FK_Payments_Users_UserId", "Payments", "UserId", "Users", null, null, "Id", ReferentialAction.NoAction, ReferentialAction.Cascade);
	}

	protected override void Down(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.DropForeignKey("FK_Payments_Users_UserId", "Payments");
		migrationBuilder.DropTable("AuditLogs");
		migrationBuilder.DropTable("EmailVerificationTokens");
		migrationBuilder.DropTable("PasswordResetTokens");
		migrationBuilder.DropTable("RolePermissions");
		migrationBuilder.DropTable("SubscriptionPlans");
		migrationBuilder.DropTable("UserSessions");
		migrationBuilder.DropTable("Permissions");
		migrationBuilder.DropTable("Roles");
		migrationBuilder.DropPrimaryKey("PK_Payments", "Payments");
		migrationBuilder.RenameTable("Payments", null, "Payment");
		migrationBuilder.RenameIndex("IX_Payments_UserId", "IX_Payment_UserId", "Payment");
		migrationBuilder.AddPrimaryKey("PK_Payment", "Payment", "Id");
		migrationBuilder.AddForeignKey("FK_Payment_Users_UserId", "Payment", "UserId", "Users", null, null, "Id", ReferentialAction.NoAction, ReferentialAction.Cascade);
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
