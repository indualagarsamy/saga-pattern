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
                    .Level(LogLevel.Error);

                var endpointConfiguration = new EndpointConfiguration("PreferredCustomerPolicy");

                endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
                endpointConfiguration.Recoverability().Delayed(c => c.NumberOfRetries(0));

                var transport = endpointConfiguration.UseTransport<MsmqTransport>();
                transport.DisableDeadLetterQueueing();

                var routing = transport.Routing();

                routing.RegisterPublisher(typeof(CustomerHasPaid), "PreferredCustomerPolicy");
                routing.RegisterPublisher(typeof(SegmentWasFlown), "PreferredCustomerPolicy");

                var persistence = endpointConfiguration.UsePersistence<InMemoryPersistence>();
                endpointConfiguration.SendFailedMessagesTo("error");
                endpointConfiguration.AuditProcessedMessagesTo("audit");
                endpointConfiguration.EnableInstallers();

                endpointConfiguration.AuditSagaStateChanges("Particular.ServiceControl");

                var endpointInstance = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);
            
                Console.WriteLine("Press f to publish SegmentWasFlown event.");
                Console.WriteLine("Press p to publish CustomerHasPaid event.");
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
                        

                        Console.WriteLine($"Publishing SegmentWasFlown event for CustomerId id: {guid:N}");

                        var message = new SegmentWasFlown()
                        {
                            CustomerId = guid,
                            MilesFlown = 10000
                        };
                        await endpointInstance.Publish(message)
                            .ConfigureAwait(false);
                    }

                    if (key.Key == ConsoleKey.P)
                    {
                      
                        Console.WriteLine($"Publishing CustomerHasPaid event for CustomerId id: {guid:N}");

                        var billedEvent = new CustomerHasPaid()
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
