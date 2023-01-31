using System.Text.Json;

namespace WhoAskedBackend.Model.Messaging;

public class MessageStorage
{
    public long QueueId { get; set; }
    private List<Message>? _messages;
    private readonly string _folder;
    private string _path;
    private long _lastBackup;

    public MessageStorage(long queueId)
    {
        this.QueueId = queueId;
        _messages = new List<Message>();
        _path = "";
        _folder = Path.GetFullPath(Environment.CurrentDirectory + "\\Backup");
        ImportQueue(queueId);
        _lastBackup = _messages.Count();
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

    public void ExportQueue()
    {
        _path = Path.Combine(_folder, QueueId + ".json");
        var json = JsonSerializer.Serialize(_messages);
        if (!System.IO.Directory.Exists(_folder))
            System.IO.Directory.CreateDirectory(_folder);
        File.WriteAllText(_path, json);
    }

    public List<Message>? RetrieveLatestMessages(int amount)
    {
        if (_messages != null && _messages.Count != 0)
            return
                _messages; //return _messages.Count < amount ? _messages : _messages?.GetRange(_messages.Count - amount, amount);
        return new List<Message>
        {
            new Message {Mess = "You can now chat", QueueId = QueueId, Sender = 0, Sent = DateTime.UtcNow}
        };
    }

    public void ReceivedMessage(Message message)
    {
        _messages?.Add(message);
        if (_lastBackup + 5 != _messages!.Count) return;
        _lastBackup = _messages!.Count;
        ExportQueue();
    }
}