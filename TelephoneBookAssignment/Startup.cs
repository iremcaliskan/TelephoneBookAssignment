using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;
using System;
using System.Reflection;
using TelephoneBookAssignment.Helpers;
using TelephoneBookAssignment.Jwt;
using TelephoneBookAssignment.Middlewares;
using TelephoneBookAssignment.Services.RabbitMQ;
using TelephoneBookAssignment.Services.SignalR.Hubs;
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

            services.AddSignalR();

            //services.AddCors(options =>
            //{
            //    options.AddPolicy("AllowOrigin", builder => builder.WithOrigins("http://localhost:3000"));
            //});
            
            services.AddSingleton<ITokenHelper, JwtHelper>();

            // For JWT
            var tokenOptions = Configuration.GetSection("TokenOptions").Get<TokenOptions>();

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = tokenOptions.Issuer,
                    ValidAudience = tokenOptions.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = SecurityKeyHelper.CreateSecurityKey(tokenOptions.SecurityKey)
                };
            });
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

            //app.UseCors(builder => builder.WithOrigins("http://localhost:3000").AllowAnyHeader());

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseCustomExceptionMiddleware();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ExcelHub>("/ExcelHub");

                endpoints.MapControllers();
            });
        }
    }
}