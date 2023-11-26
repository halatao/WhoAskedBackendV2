using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace WhoAskedBackend.Services.WebSocket
{
    public class WebSocketHandler
    {
        private readonly ConcurrentDictionary<string, QueueWebSocket> _sockets = new();

        public string AddSocket(long queueId, System.Net.WebSockets.WebSocket socket)
        {
            var socketId = GenerateConnectionId();
            _sockets.TryAdd(socketId, new QueueWebSocket {QueueId = queueId, Connection = socket});
            return socketId;
        }

        public async Task RemoveSocket(string id)
        {
            if (_sockets.TryRemove(id, out var socket))
            {
                await socket.Connection?.CloseAsync(WebSocketCloseStatus.NormalClosure,
                    "Connection closed by QueueWebSocket", default)!;
            }
        }

        public async Task SendToQueueUsers(long queueId)
        {
            _sockets.Where(q => q.Value.QueueId == queueId).ToList().ForEach(async q =>
            {
                await SendAsync(q.Key, "New message");
            });
        }

        public async Task SendAsync(string id, string message)
        {
            var socket = _sockets[id].Connection;
            if (socket != null && socket.State != WebSocketState.Open)
                return;

            var buffer = Encoding.UTF8.GetBytes(message);
            await socket!.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), WebSocketMessageType.Text, true,
                default);
        }

        private string GenerateConnectionId()
        {
            return Guid.NewGuid().ToString("N");
        }
    }

    class QueueWebSocket
    {
        public System.Net.WebSockets.WebSocket? Connection { get; set; }
        public long? QueueId { get; set; }
    }
}