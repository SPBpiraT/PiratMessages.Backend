using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using PiratMessages.Domain;
using PiratMessages.Application.Interfaces;
using PiratMessages.Persistence.EntityTypeConfiguration;

namespace PiratMessages.Persistence
{
    public class MessagesDbContext : IdentityDbContext<IdentityUser>, IMessagesDbContext 
    {
        public DbSet<Message> Messages { get; set; }
        public DbSet<IdentityUser> Users { get; set; }

        public MessagesDbContext(DbContextOptions<MessagesDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new MessageConfiguration());
            base.OnModelCreating(builder);
        }
    }
}
