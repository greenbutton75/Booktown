using Infrastructure.Core.Events;
using Infrastructure.MessageBrokers.Kafka;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Infrastructure.MessageBrokers
{
    public static class MessageBrokersExtensions
    {
        public static IServiceCollection AddMessageBroker(this IServiceCollection services, IConfiguration Configuration)
        {
            var options = new MessageBrokersOptions();
            Configuration.GetSection(nameof(MessageBrokersOptions)).Bind(options);
            services.Configure<MessageBrokersOptions>(Configuration.GetSection(nameof(MessageBrokersOptions)));

            return services.AddKafka(Configuration);
        }

        public static IApplicationBuilder UseSubscribeEvent<T>(this IApplicationBuilder app) where T : IEvent
        {
            app.ApplicationServices.GetRequiredService<IEventListener>().Subscribe<T>();

            return app;
        }

        public static IApplicationBuilder UseSubscribeEvent(this IApplicationBuilder app, Type type)
        {
            app.ApplicationServices.GetRequiredService<IEventListener>().Subscribe(type);

            return app;
        }
    }
}
