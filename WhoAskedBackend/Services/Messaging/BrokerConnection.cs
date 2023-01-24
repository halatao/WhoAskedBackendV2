using System.Net.NetworkInformation;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;

namespace WhoAskedBackend.Services.Messaging
{
    public class BrokerConnection
    {
        public IModel? Channel { get; private set; }
        public string? Exchange;

        public BrokerConnection()
        {
            Exchange = "exchange";
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
                VirtualHost = "/"
            };
            var connection = factory.CreateConnection();
            Channel = connection.CreateModel();
            Channel.ExchangeDeclare(Exchange, ExchangeType.Direct);
        }
    }
}