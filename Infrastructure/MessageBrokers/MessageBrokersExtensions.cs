using Infrastructure.Core.Events;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Infrastructure.MessageBrokers
{
    public static class MessageBrokersExtensions
    {
        public static IServiceCollection AddMessageBroker(this IServiceCollection services, IConfiguration Configuration)
        {
            /*
            var options = new MessageBrokersOptions();
            Configuration.GetSection(nameof(MessageBrokersOptions)).Bind(options);
            services.Configure<MessageBrokersOptions>(Configuration.GetSection(nameof(MessageBrokersOptions)));
            */
            return services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();

                foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                {
                    var types = a.GetTypes()
                    .Where(mytype => mytype.GetInterfaces().Contains(typeof(IConsumer)));

                    Debug.WriteLine("Adding Consumers");
                    foreach (var type in types)
                    {
                     if (type.Name.Contains ("InventoryLoadConsumer"))   x.AddConsumer(type);
                        if (type.Name.Contains("InventoryLoadConsumer")) Debug.WriteLine("AddConsumer - " + type.Name);
                    }
                }
                //    x.AddConsumer<InventoryLoadConsumer>();


                //x.UsingRabbitMq();
                x.UsingRabbitMq((brc, rbfc) =>
                {
                    rbfc.UseInMemoryOutbox();
                    rbfc.UseMessageRetry(r =>
                    {
                        r.Incremental(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
                    });
                    //rbfc.UseDelayedMessageScheduler();
                    rbfc.Host("localhost", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });
                    rbfc.ConfigureEndpoints(brc);
                });


            }).AddScoped<IEventListener, EventListener>();

        }
    }
}
