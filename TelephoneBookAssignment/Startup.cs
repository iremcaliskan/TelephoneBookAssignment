using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;
using System;
using System.Reflection;
using TelephoneBookAssignment.Middlewares;
using TelephoneBookAssignment.Services.RabbitMQ;
using TelephoneBookAssignment.Shared.DataAccess.MongoDb;
using TelephoneBookAssignment.Shared.Services.Logger;
using TelephoneBookAssignment.Shared.Services.RabbitMQ;

namespace TelephoneBookAssignment
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TelephoneBookAssignment", Version = "v1" });
            });

            services.Configure<MongoSettings>(opt =>
            {
                opt.ConnectionString = Configuration.GetSection("MongoSettings:ConnectionString").Value;
                opt.DatabaseName = Configuration.GetSection("MongoSettings:DatabaseName").Value;
            });

            services.AddSingleton<IMongoTelephoneBookDbContext, MongoTelephoneBookDbContext>();

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddSingleton<ILoggerService, ConsoleLoggerService>();

            // RabbitMQ connection
            services.AddSingleton(x => new ConnectionFactory()
            {
                Uri = new Uri(Configuration.GetConnectionString("RabbitMQ")),
                DispatchConsumersAsync = true
            });

            // Services added to DI Container
            services.AddSingleton<RabbitMQClientService>();

            services.AddSingleton<RabbitMQPublisher>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TelephoneBookAssignment v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseCustomExceptionMiddleware();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}