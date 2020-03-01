using System;
using System.Threading.Tasks;
using Events;
using NServiceBus;
using NServiceBus.Logging;

namespace NotifyNewlyAddedGoldCustomers
{
    class Program
    {
        static async Task Main()
        {

            LogManager.Use<DefaultFactory>()
                .Level(LogLevel.Info);

            var endpointConfiguration = new EndpointConfiguration("NotifyNewlyAddedGoldCustomers");

            endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
            endpointConfiguration.Recoverability().Delayed(c => c.NumberOfRetries(0));

            var transport = endpointConfiguration.UseTransport<LearningTransport>();
            var persistence = endpointConfiguration.UsePersistence<InMemoryPersistence>();
            endpointConfiguration.SendFailedMessagesTo("error");
            endpointConfiguration.AuditProcessedMessagesTo("audit");
            endpointConfiguration.EnableInstallers();

            // Send heartbeats to ServiceControl for monitoring.
            endpointConfiguration.SendHeartbeatTo("Particular.ServiceControl");

            // start the endpoint
            var endpointInstance = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

            Console.WriteLine("Press ENTER to exit");

            var guid = Guid.NewGuid();

            while (true)
            {
                var key = Console.ReadKey();
                Console.WriteLine();

                if (key.Key == ConsoleKey.Enter)
                {
                    break;
                }
            }
            ;
            await endpointInstance.Stop()
                .ConfigureAwait(false);
        }
    }
}
