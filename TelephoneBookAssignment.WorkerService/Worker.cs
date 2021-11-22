using ClosedXML.Excel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TelephoneBookAssignment.Shared.DataAccess.MongoDb;
using TelephoneBookAssignment.Shared.Entities;
using TelephoneBookAssignment.Shared.Services.Logger;
using TelephoneBookAssignment.Shared.Services.RabbitMQ;

namespace TelephoneBookAssignment.WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider; // Context is not singleton, is scoped
        private readonly ILoggerService _loggerService;
        private readonly RabbitMQClientService _rabbitMQClientService;
        private IModel _channel;

        public Worker(RabbitMQClientService rabbitMqClientService, ILoggerService loggerService, IServiceProvider serviceProvider)
        {
            _rabbitMQClientService = rabbitMqClientService;
            _loggerService = loggerService;
            _serviceProvider = serviceProvider;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _channel = _rabbitMQClientService.ConnectForSubscriber();

            // Send each Subscriber one message
            _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            return base.StartAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);

            _channel.BasicConsume(queue: RabbitMQClientService.ReportQueueName, autoAck: false, consumer: consumer);

            consumer.Received += Consumer_Received;

            return Task.CompletedTask;
        }

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs @event)
        {
            await Task.Delay(5000); // For Observing

            var createExcelMessage = JsonSerializer.Deserialize<CreateReportMessage>(Encoding.UTF8.GetString(@event.Body.ToArray()));

            using var memoryStream = new MemoryStream();
            var workBook = new XLWorkbook();
            var dataSet = new DataSet();
            dataSet.Tables.Add(GetTable("contact"));
            workBook.Worksheets.Add(dataSet);
            workBook.SaveAs(memoryStream);

            MultipartFormDataContent multipartFormDataContent = new();
            multipartFormDataContent.Add(new ByteArrayContent(content: memoryStream.ToArray()), name: "file", fileName: Guid.NewGuid().ToString() + ".xlsx");

            // Must attention on port number, if build is made on Kestrel the port will change as 5001
            var baseUrl = "https://localhost:44321/api/reports/uploadExcel";
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.PostAsync(requestUri: $"{baseUrl}?fileId={createExcelMessage.ObjectId.ToString()}", content: multipartFormDataContent);

                if (response.IsSuccessStatusCode)
                {
                    _channel.BasicAck(@event.DeliveryTag, false);
                    _loggerService.Write($"File ( Id: {createExcelMessage.ObjectId.ToString()}) was created");
                }
            }
        }

        private DataTable GetTable(string tableName)
        {
            List<Contact> contacts;
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<IMongoTelephoneBookDbContext>();
                var collection = context.GetCollection<Contact>(nameof(Contact));
                contacts = collection.Find(x => true).SortBy(x => x.Name).ToList();
            }

            // Memory table
            DataTable table = new() { TableName = tableName };
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Last Name", typeof(string));
            table.Columns.Add("Firm", typeof(string));
            table.Columns.Add("Phone Number", typeof(string));
            table.Columns.Add("Mail", typeof(string));
            table.Columns.Add("Location", typeof(string));

            contacts.ForEach(x =>
            {
                table.Rows.Add(x.Name, x.LastName, x.Firm, x.Information.PhoneNumber, x.Information.Mail, x.Information.Location);
            });

            return table;
        }
    }
}