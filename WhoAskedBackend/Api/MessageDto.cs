namespace WhoAskedBackend.Api
{
    public class MessageDto
    {
        public int Sender { get; set; }
        public int QueueId { get; set; }
        public string? Mess { get; set; }
        public DateTime? Sent { get; set; }
    }
}