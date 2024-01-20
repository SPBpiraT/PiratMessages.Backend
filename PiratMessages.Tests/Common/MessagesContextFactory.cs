using Microsoft.EntityFrameworkCore;
using PiratMessages.Persistence;
using PiratMessages.Domain;

namespace PiratMessages.Tests.Common
{
    public class MessagesContextFactory
    {
        public static Guid UserAId = Guid.NewGuid();
        public static Guid UserBId = Guid.NewGuid();

        public static Guid NoteIdForDelete = Guid.NewGuid();
        public static Guid NoteIdForUpdate = Guid.NewGuid();

        public static MessagesDbContext Create()
        {
            var options = new DbContextOptionsBuilder<MessagesDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new MessagesDbContext(options);
            context.Database.EnsureCreated();

            context.Messages.AddRange(
                new Message
                {
                    CreationDate = DateTime.Today,
                    Details = "Details1",
                    EditDate = null,
                    Id = Guid.Parse("872F42FC-218B-4314-BD39-209E02B9ECC8"),
                    UserId = UserAId
                },

                new Message
                {
                    CreationDate = DateTime.Today,
                    Details = "Details2",
                    EditDate = null,
                    Id = Guid.Parse("6A651058-11F4-4A38-9A79-ACBB497B092C"),
                    UserId = UserBId
                },

                new Message
                {
                    CreationDate = DateTime.Today,
                    Details = "Details3",
                    EditDate = null,
                    Id = NoteIdForDelete,
                    UserId = UserAId
                },

                new Message
                {
                    CreationDate = DateTime.Today,
                    Details = "Details4",
                    EditDate = null,
                    Id = NoteIdForUpdate,
                    UserId = UserBId
                }
            );

            context.SaveChanges();
            return context;
        }

        public static void Destroy(MessagesDbContext context)
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }
    }
}
