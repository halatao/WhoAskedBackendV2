using WhoAskedBackend.Model.Messaging;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace WhoAskedBackend.Services.Messaging
{
    public class MessageProvider : IMessageProvider
    {
        private readonly QueueProvider _queueProvider;
        private List<Message>? _messages;
        private readonly string _folder;
        private string _path;

        public bool Delivered { get; private set; }

        public MessageProvider(QueueProvider queueProvider)
        {
            this._queueProvider = queueProvider;
            _messages = new List<Message>();
            _path = "";
            _folder = Path.GetFullPath(Environment.CurrentDirectory + "\\Backup");
            MessageReceivedHandler.SetProvider(this);
        }

        public void ImportQueue(int queueId)
        {
            _path = Path.Combine(_folder, queueId + ".json");
            if (!System.IO.File.Exists(_path))
            {
                _messages?.Clear();
                return;
            }

            using var r = new StreamReader(_path);
            var json = r.ReadToEnd();
            _messages = JsonSerializer.Deserialize<List<Message>>(json);
        }

        public void ExportQueue(int queueId)
        {
            _path = Path.Combine(_folder, queueId + ".json");
            var json = JsonSerializer.Serialize(_messages);
            if (!System.IO.Directory.Exists(_folder))
                System.IO.Directory.CreateDirectory(_folder);
            File.WriteAllText(_path, json);
        }

        public List<Message>? RetrieveLatestMessages(int queueId, int amount)
        {
            ImportQueue(queueId);
            if (_messages == null || _messages.Count == 0)
            {
                _messages?.Add(new Message {Mess = "You can now chat", QueueId = queueId, Sender = 0});
                return _messages;
            }
            else if (_messages.Count < amount)
            {
                return _messages;
            }

            return _messages?.GetRange(_messages.Count - amount, amount);
        }

        public void QueueMessage(Message message)
        {
            Delivered = false;
            _queueProvider.BasicPublic(message);
        }

        public void ReceivedMessage(Message message)
        {
            ImportQueue(message.QueueId);
            _messages?.Add(message);
            ExportQueue(message.QueueId);
            Delivered = true;
        }
    }
}