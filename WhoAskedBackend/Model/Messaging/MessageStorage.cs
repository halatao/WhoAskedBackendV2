using System.Text.Json;

namespace WhoAskedBackend.Model.Messaging;

public class MessageStorage
{
    public long QueueId { get; set; }
    public List<Message>? _messages { get; set; }
    private readonly string _folder;
    private string _path;

    public MessageStorage(long queueId)
    {
        this.QueueId = queueId;
        _messages = new List<Message>();
        _path = "";
        _folder = Path.GetFullPath(Environment.CurrentDirectory + "\\Backup");
    }

    public void ImportQueue(long queueId)
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

    public void ExportQueue(long queueId)
    {
        _path = Path.Combine(_folder, queueId + ".json");
        var json = JsonSerializer.Serialize(_messages);
        if (!System.IO.Directory.Exists(_folder))
            System.IO.Directory.CreateDirectory(_folder);
        File.WriteAllText(_path, json);
    }

    public List<Message>? RetrieveLatestMessages(int amount)
    {
        ImportQueue(QueueId);
        if (_messages != null && _messages.Count != 0)
            return _messages.Count < amount ? _messages : _messages?.GetRange(_messages.Count - amount, amount);
        _messages?.Add(new Message {Mess = "You can now chat", QueueId = QueueId, Sender = 0, Sent = DateTime.Now});
        return _messages;
    }

    public void ReceivedMessage(Message message)
    {
        ImportQueue(message.QueueId);
        _messages?.Add(message);
        ExportQueue(message.QueueId);
    }
}