using AV.Engine.Core.Interfaces;
using AV.Engine.Persistence.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace AV.Engine.DI
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAVEngineServices(this IServiceCollection services)
        {
            //services.AddLogging(builder => builder.AddConsole().AddDebug());

            services.AddSingleton<IEventStore, InMemoryEventStore>();
            services.AddSingleton<IThreatSimulator, ThreatSimulator>();
            services.AddTransient<IScanEngine, MockScanEngine>();
            services.AddSingleton<IAVEngine, AVEngine>();

            return services;
        }
    }
}
