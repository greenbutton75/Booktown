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
        public static IServiceCollection AddMessageBroker(this IServiceCollection services, IConfiguration Configuration, TopologyConfigurator topologyConfigurator)
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
                /*
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
                */
                x.UsingRabbitMq((context, cfg) => cfg.ConfigureEndpoints(context));

                x.AddRider(rider =>
                {
                    //rider.AddConsumer<VideoCreatedEventConsumer>();

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
                            rider.AddConsumer(type);
                        }
                    }


                    Debug.WriteLine("Adding Producers");
                    topologyConfigurator.ConfigureProducers(rider);


                    rider.UsingKafka((context, k) =>
                    {
                        k.Host("localhost:9092");



                        /*----------------------------------------------*/
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
                                var innertype = type.GetGenericArguments()[0];
                                k.TopicEndpoint<innertype>(nameof(innertype), GetUniqueName(nameof(innertype)), e =>
                                {
                                    // e.AutoOffsetReset = AutoOffsetReset.Latest;
                                    //e.ConcurrencyLimit = 3;
                                    e.CheckpointInterval = TimeSpan.FromSeconds(10);
                                    e.ConfigureConsumer(context, type);

                                    e.CreateIfMissing(t =>
                                    {
                                        //t.NumPartitions = 2; //number of partitions
                                        //t.ReplicationFactor = 1; //number of replicas
                                    });
                                });
                            }
                        }




                        /*----------------------------------------------*/




                    });
                });

            });
            
            services.AddScoped<IEventListener, EventListener>();

            return services;
        }
        private static string GetUniqueName(string eventName)
        {
            string callingAssembly = Assembly.GetCallingAssembly().GetName().Name;
            return $"{callingAssembly}.{eventName}";
        }
    }
}
