using Microsoft.EntityFrameworkCore;
using PiratMessages.Domain;

namespace PiratMessages.Application.Interfaces
{
    public interface IMessagesDbContext
    {
        DbSet<Message> Messages { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
