using Microsoft.EntityFrameworkCore;
using WhoAskedBackend.Data;
using WhoAskedBackend.Model;
using WhoAskedBackend.Model.Messaging;
using WhoAskedBackend.Services.ContextServices;

namespace WhoAskedBackend.Services.Messaging
{
    public class MessageProvider
    {
        private readonly QueueProvider _queueProvider;
        private readonly QueueStorage _queueStorage;
        private readonly ActiveUsersService _activeUsersService;
        private readonly WhoAskedContext _context;


        public bool Delivered { get; private set; }

        public MessageProvider(QueueProvider queueProvider, QueueStorage queueStorage,
            ActiveUsersService activeUsersService, WhoAskedContext context)
        {
            this._queueProvider = queueProvider;
            _queueStorage = queueStorage;
            _activeUsersService = activeUsersService;
            _context = context;
            MessageReceivedHandler.SetProvider(this);
        }


        public void QueueMessage(Message message)
        {
            Delivered = false;
            _queueProvider.BasicPublic(message);
        }

        public async Task ReceivedMessage(Message message)
        {
            var active = _activeUsersService.GetActiveUsers();

            var queue = await _context.Queue!.Include(q => q.Users).ThenInclude(q => q.User)
                .FirstAsync(q => q.QueueId == message.QueueId);
            var inactive = new List<User>();
            foreach (var user in queue.Users)
            {
                if (active.Contains(user.User?.UserName!)) continue;
                if (user.User != null) inactive.Add(user.User);
            }

            foreach (var user in inactive)
            {
                var usr = await _context.UserInQueue!.FirstAsync(q =>
                    q.QueueId == message.QueueId && q.UserId == user.UserId);
                usr.Seen = false;
                _context.UserInQueue?.Update(usr);

                await _context.SaveChangesAsync();
            }

            _queueStorage.Queues.Find(q => q.QueueId == message.QueueId)?.ReceivedMessage(message);
            Delivered = true;
        }

        public List<Message>? RetrieveLatestMessages(long queueId, int amount)
        {
            return _queueStorage.Queues.Find(q => q.QueueId == queueId)?.RetrieveLatestMessages(amount)!;
        }

        public Message RetrieveLatestMessage(long queueId)
        {
            return _queueStorage.Queues.Find(q => q.QueueId == queueId)!.RetrieveLatestMessages(1)!.Last();
        }
    }
}