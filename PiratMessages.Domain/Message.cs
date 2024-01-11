namespace PiratMessages.Domain
{
    public class Message
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid DestinationUserId { get; set; }
        public string Details { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? EditDate { get; set; }
    }
}
