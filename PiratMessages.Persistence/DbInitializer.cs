namespace PiratMessages.Persistence
{
    public class DbInitializer
    {
        public static void Initialize(MessagesDbContext context)
        {
            context.Database.EnsureCreated();
        }
    }
}
