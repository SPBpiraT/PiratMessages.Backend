using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PiratMessages.Domain;

namespace PiratMessages.Persistence.EntityTypeConfiguration
{
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.HasKey(message => message.Id);
            builder.HasIndex(message => message.Id).IsUnique();
        }
    }
}
