using RabbitMQ;
using RabbitMQ.Messages;
using Validation.MainServer.Services;

namespace Validation.MainServer
{
    public class Startup
    {
        public static IConfiguration Configuration { get; set; }
        public Startup(IConfiguration configuration)
        {
            if(configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var builder = new ConfigurationBuilder().AddJsonFile("validatorsconfig.json");

            configuration = builder.Build();
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();
            services.AddSingleton<RabbitMQRPCSender<NSPMessageRequest, NSPMessageResponse>>(provider
                => new RabbitMQRPCSender<NSPMessageRequest, NSPMessageResponse>(Configuration.GetSection("NSPValidator"),
                provider.GetService<ILogger>()));
            services.AddSingleton<RabbitMQRPCSender<PassportMessageRequest, PassportMessageResponse>>(provider
                => new RabbitMQRPCSender<PassportMessageRequest, PassportMessageResponse>(Configuration.GetSection("PassportValidator"),
                provider.GetService<ILogger>()));

            services.AddSingleton<RabbitMQRPCSender<PhoneMessageRequest, PhoneMessageResponse>>(provider
                => new RabbitMQRPCSender<PhoneMessageRequest, PhoneMessageResponse>(Configuration.GetSection("PhoneValidator"),
                provider.GetService<ILogger>()));

            services.AddSingleton<GreeterService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<GreeterService>();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });

        }
    }
}
