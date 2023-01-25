using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client.Events;
using WhoAskedBackend.Model.Messaging;
using WhoAskedBackend.Services.Messaging;

namespace WhoAskedBackend.Services.Messaging
{
    public class MessageReceivedHandler
    {
        private static MessageReceivedHandler _instance = null;
        private static readonly object Padlock = new object();
        private static MessageProvider? _messageProvider;

        private MessageReceivedHandler()
        {
        }

        public static MessageReceivedHandler MessageReceived
        {
            get
            {
                if (_instance == null)
                {
                    lock (Padlock)
                    {
                        if (_instance == null)
                        {
                            _instance = new MessageReceivedHandler();
                        }
                    }
                }

                return _instance;
            }
        }

        public static void SetProvider(MessageProvider? messageProvider)
        {
            _messageProvider = messageProvider;
        }

        public static void OnMessageReceived(object? sender, BasicDeliverEventArgs e)
        {
            var message = new Message(System.Text.Encoding.UTF8.GetString(e.Body.ToArray()));
            _messageProvider?.ReceivedMessage(message);
        }
    }
}