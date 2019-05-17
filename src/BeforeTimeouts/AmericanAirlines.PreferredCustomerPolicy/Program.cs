using System;
using System.Threading.Tasks;
using Messages;
using NServiceBus;
using NServiceBus.Logging;

namespace PreferredCustomerPolicy
{
    static class Program
    {
            static async Task Main()
            {

                LogManager.Use<DefaultFactory>()
                    .Level(LogLevel.Info);

                var endpointConfiguration = new EndpointConfiguration("PreferredCustomerPolicy");

                endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
                endpointConfiguration.Recoverability().Delayed(c => c.NumberOfRetries(0));

                var transport = endpointConfiguration.UseTransport<MsmqTransport>();
                transport.DisableDeadLetterQueueing();

                var routing = transport.Routing();

                routing.RegisterPublisher(typeof(CustomerWasBilled), "PreferredCustomerPolicy");
                routing.RegisterPublisher(typeof(FlightPlanWasAdded), "PreferredCustomerPolicy");

                var persistence = endpointConfiguration.UsePersistence<InMemoryPersistence>();
                endpointConfiguration.SendFailedMessagesTo("error");
                endpointConfiguration.AuditProcessedMessagesTo("audit");
                endpointConfiguration.EnableInstallers();

                endpointConfiguration.AuditSagaStateChanges("Particular.ServiceControl");

                var endpointInstance = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);
            
                Console.WriteLine("Press f to publish flight plan added.");
                Console.WriteLine("Press b to publish customer was billed added.");
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

                   
                    if (key.Key == ConsoleKey.F)
                    {
                        

                        Console.WriteLine($"Publishing flight plan for CustomerId id: {guid:N}");

                        var message = new FlightPlanWasAdded()
                        {
                            CustomerId = guid,
                            MilesFlown = 10000
                        };
                        await endpointInstance.Publish(message)
                            .ConfigureAwait(false);
                    }

                    if (key.Key == ConsoleKey.B)
                    {
                      
                        Console.WriteLine($"Publishing CustomerWasBilled for CustomerId id: {guid:N}");

                        var billedEvent = new CustomerWasBilled()
                        {
                            CustomerId = guid,
                             DollarsPaid = 2500
                        };
                        await endpointInstance.Publish(billedEvent)
                            .ConfigureAwait(false);
                    }
                }
            ;
                await endpointInstance.Stop()
                    .ConfigureAwait(false);
            }
        
    }
}
