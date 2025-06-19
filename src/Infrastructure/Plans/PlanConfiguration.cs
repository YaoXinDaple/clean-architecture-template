using Domain.Plans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Plans;
public class PlanConfiguration : IEntityTypeConfiguration<Plan>
{
    public void Configure(EntityTypeBuilder<Plan> builder)
    {
        builder.ToTable(nameof(Plan));
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(128);

        builder.OwnsOne(x => x.Elapse, elapse =>
        {
            elapse.Property(x => x.Start).IsRequired();
            elapse.Property(x => x.End).IsRequired();
            elapse.Property(x => x.Year).IsRequired(false);
        });
        builder.Navigation(x => x.Elapse).IsRequired(false);

        builder.Property(x => x.CompleteTime).IsRequired(false);

        builder.Property(x => x.CreateUser).IsRequired().HasMaxLength(64);
        builder.Property(x => x.CreationTime).IsRequired();
    }
}
