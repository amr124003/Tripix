using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripix.Entities;

namespace Tripix.Context.Config
{
    public class CreditCardConfig : IEntityTypeConfiguration<CreditCard>
    {
        public void Configure ( EntityTypeBuilder<CreditCard> builder )
        {
            builder.ToTable("CreditCards");
            builder.HasIndex(x => x.CardNumber).IsUnique();
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.CardNumber)
                .HasAnnotation("RegularExpression", "^[0-9]{16}$")
                .HasMaxLength(19)
                .IsRequired()
                .IsUnicode(false);
            builder.Property(x => x.CardHolderName)
                .HasMaxLength(20)
                .IsRequired()
                .IsUnicode(false)
                .HasAnnotation("RegularExpression", "^[a-zA-Z ]+$");
            builder.Property(x => x.ExpiryDate)
                .IsRequired()
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasAnnotation("RegularExpression", "^(0[1-9]|1[0-2])\\/([0-9]{2})$");
            builder.Property(x => x.CVV)
                .IsRequired()
                .HasMaxLength(4)
                .IsUnicode(false)
                .HasAnnotation("RegularExpression", "^[0-9]{3,4}$");
        }
    }
}
