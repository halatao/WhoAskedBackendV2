namespace WhoAskedBackend.Api
{
    public class MessageDto
    {
        public long Sender { get; set; }
        public long QueueId { get; set; }
        public string? Mess { get; set; }
        public DateTime? Sent { get; set; }
    }
}