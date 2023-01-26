namespace WhoAskedBackend.Model.Messaging
{
    public class Message
    {
        public long Sender { get; set; }
        public long QueueId { get; set; }
        public DateTime? Sent { get; set; }
        public string? Mess { get; set; }


        public Message(string toStringMessage)
        {
            string?[] arr = toStringMessage.Split(";");
            this.Sender = Convert.ToInt32(arr[0]);
            this.QueueId = Convert.ToInt32(arr[1]);
            this.Sent = Convert.ToDateTime(arr[2]);
            this.Mess = arr[3];
        }

        public Message()
        {
        }

        public override string ToString()
        {
            return Sender + ";" + QueueId + ";" + Sent + ";" + Mess;
        }
    }
}