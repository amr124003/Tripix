using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripix.Entities;

namespace Tripix.Context.Config
{
    public class QuestionConfig : IEntityTypeConfiguration<Question>
    {
        public void Configure ( EntityTypeBuilder<Question> builder )
        {
            builder.ToTable("Questions");
            builder.HasKey(x => new { x.Id, x.question });
        }
    }
}
