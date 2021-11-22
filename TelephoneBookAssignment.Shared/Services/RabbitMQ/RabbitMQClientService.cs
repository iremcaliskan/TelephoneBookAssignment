using RabbitMQ.Client;
using System;
using TelephoneBookAssignment.Shared.Services.Logger;

namespace TelephoneBookAssignment.Shared.Services.RabbitMQ
{
    public class RabbitMQClientService : IDisposable
    {
        private readonly ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;

        public static string ReportExchangeName = "ReportDirectExchange";
        public static string ReportRoutingKey = "report-route";
        public static string ReportQueueName = "queue-report";

        private readonly ILoggerService _loggerService;

        public RabbitMQClientService(ConnectionFactory connectionFactory, ILoggerService loggerService)
        {
            _connectionFactory = connectionFactory;
            _loggerService = loggerService;
        }

        // Return a channel
        public IModel ConnectForPublisher()
        {
            _connection = _connectionFactory.CreateConnection();

            if (_channel is { IsOpen: true })
            {
                return _channel;
            }

            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(exchange: ReportExchangeName, type: ExchangeType.Direct, durable: true, autoDelete: false, arguments: null);

            _channel.QueueDeclare(queue: ReportQueueName, durable: true, exclusive: false, autoDelete: false, null);

            _channel.QueueBind(exchange: ReportExchangeName, routingKey: ReportRoutingKey, queue: ReportQueueName, arguments: null);

            _loggerService.Write("Connection is established with RabbitMQ");

            return _channel;
        }

        // Return a channel
        public IModel ConnectForSubscriber()
        {
            _connection = _connectionFactory.CreateConnection();

            if (_channel is { IsOpen: true })
            {
                return _channel;
            }

            _channel = _connection.CreateModel();


            _loggerService.Write("Connection is established with RabbitMQ");

            return _channel;
        }

        public void Dispose()
        {// Connections will be closed
            // _x? means if x is not null close/dispose the _x
            _channel?.Close();
            _channel?.Dispose();

            _connection?.Close();
            _connection?.Dispose();

            _loggerService.Write("RabbitMQ connection is failed...");
        }
    }
}