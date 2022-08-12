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
            services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();

                Debug.WriteLine("Adding Consumers");
                foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                {
                    var types = a.GetTypes()
                        .Where(mytype =>
                            mytype.FindInterfaces(
                                (t, _) => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IMyConsumer<>),
                                true).Any());
                    
                    foreach (var type in types)
                    {
                        x.AddConsumer(type); 
                    }
                }

                x.UsingRabbitMq((brc, rbfc) =>
                {
                    rbfc.Host("localhost", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    rbfc.UseInMemoryOutbox();
                    rbfc.UseMessageRetry(r =>
                    {
                        r.Incremental(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
                    });
                    //rbfc.UseDelayedMessageScheduler();
                    rbfc.ConfigureEndpoints(brc);
                });
            });
            
            services.AddScoped<IEventListener, EventListener>();

            return services;
        }
    }
}
