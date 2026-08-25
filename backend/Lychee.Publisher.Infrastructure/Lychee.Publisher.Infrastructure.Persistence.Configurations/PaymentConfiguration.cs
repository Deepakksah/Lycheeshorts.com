using Lychee.Publisher.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lychee.Publisher.Infrastructure.Persistence.Configurations;

public sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
	public void Configure(EntityTypeBuilder<Payment> builder)
	{
		builder.HasKey((Payment p) => p.Id);
		builder.Property((Payment p) => p.Provider).HasMaxLength(50).IsRequired();
		builder.Property((Payment p) => p.ProviderPaymentId).HasMaxLength(100).IsRequired();
		builder.Property((Payment p) => p.Amount).HasColumnType("decimal(18,2)").IsRequired();
		builder.Property((Payment p) => p.Currency).HasMaxLength(10).IsRequired();
		builder.Property((Payment p) => p.Status).HasMaxLength(50).IsRequired();
		builder.HasOne((Payment p) => p.User).WithMany((User u) => u.Payments).HasForeignKey((Payment p) => p.UserId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
