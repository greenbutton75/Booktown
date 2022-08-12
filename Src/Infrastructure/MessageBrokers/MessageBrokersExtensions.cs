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

                Debug.WriteLine("Adding Consumers");
                foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                {
                    var types = a.GetTypes()
                    .Where(mytype => mytype.FindInterfaces((x, _) => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IMyConsumer<>), true).Any());

                    //mytype.GetInterfaces().Contains(typeof(IMyConsumer))

                    foreach (var type in types)
                    {
                        x.AddConsumer(type); //.Endpoint( e => e.Name = "Events.Inventory:InventoryLoadEvent" /*type.GetInterfaces()[0].GetGenericArguments()[0].FullName*/);
                       Debug.WriteLine("AddConsumer - " + type.Name +" - "+ type.AssemblyQualifiedName +" ---> "+ type.GetInterfaces()[0].GetGenericArguments()[0].FullName);
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
