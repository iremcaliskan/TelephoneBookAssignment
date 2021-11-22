using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using TelephoneBookAssignment.Shared.Services.RabbitMQ;

namespace TelephoneBookAssignment.Services.RabbitMQ
{
    public class RabbitMQPublisher
    {
        private readonly RabbitMQClientService _rabbitMqClientService;

        public RabbitMQPublisher(RabbitMQClientService rabbitMqClientService)
        {
            _rabbitMqClientService = rabbitMqClientService;
        }

        public void Publish(CreateReportMessage createReportMessage)
        {
            var channel = _rabbitMqClientService.ConnectForPublisher();

            var bodyString = JsonSerializer.Serialize<CreateReportMessage>(value: createReportMessage);
            var bodyByte = Encoding.UTF8.GetBytes(s: bodyString);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true; // For permanent message

            channel.BasicPublish(exchange: RabbitMQClientService.ReportExchangeName, routingKey: RabbitMQClientService.ReportRoutingKey, basicProperties: properties, body: bodyByte);
        }
    }
}