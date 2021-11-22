using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using TelephoneBookAssignment.Shared.DataAccess.MongoDb;
using TelephoneBookAssignment.Shared.Services.Logger;
using TelephoneBookAssignment.Shared.Services.RabbitMQ;

namespace TelephoneBookAssignment.WorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        // DI Container Support
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration configuration = hostContext.Configuration; // Take configuration from Host content

                    services.Configure<MongoSettings>(opt =>
                    {
                        opt.ConnectionString = configuration.GetConnectionString("MongoSettings:ConnectionString");
                        opt.DatabaseName = configuration.GetConnectionString("MongoSettings:DatabaseName");
                    });

                    services.AddSingleton<IMongoTelephoneBookDbContext, MongoTelephoneBookDbContext>();

                    // RabbitMQ connection
                    services.AddSingleton(x => new ConnectionFactory()
                    {
                        Uri = new Uri(configuration.GetConnectionString("RabbitMQ")),
                        DispatchConsumersAsync = true
                    });

                    services.AddSingleton<RabbitMQClientService>();

                    services.AddSingleton<ILoggerService, ConsoleLoggerService>();

                    services.AddHostedService<Worker>();
                });
    }
}