﻿using System;
using System.Threading.Tasks;
using NServiceBus;

class Program
{

    static void Main()
    {
        AsyncMain().GetAwaiter().GetResult();
    }

    static async Task AsyncMain()
    {
        Console.Title = "Samples.Azure.StoragePersistence.Client";
        var endpointConfiguration = new EndpointConfiguration("Samples.Azure.StoragePersistence.Client");
        endpointConfiguration.UseSerialization<JsonSerializer>();
        endpointConfiguration.EnableInstallers();
        endpointConfiguration.UsePersistence<InMemoryPersistence>();
        endpointConfiguration.SendFailedMessagesTo("error");
        var transport = endpointConfiguration.UseTransport<LearningTransport>();
        transport.Routing().RouteToEndpoint(typeof(StartOrder), "Samples.Azure.StoragePersistence.Server");

        var endpointInstance = await Endpoint.Start(endpointConfiguration)
            .ConfigureAwait(false);
        Console.WriteLine("Press 'enter' to send a StartOrder messages");
        Console.WriteLine("Press any other key to exit");

        while (true)
        {
            var key = Console.ReadKey();
            Console.WriteLine();

            if (key.Key != ConsoleKey.Enter)
            {
                break;
            }

            var orderId = Guid.NewGuid();
            var startOrder = new StartOrder
            {
                OrderId = orderId
            };
            await endpointInstance.Send(startOrder)
                .ConfigureAwait(false);
            Console.WriteLine($"StartOrder Message sent with OrderId {orderId}");
        }
        await endpointInstance.Stop()
            .ConfigureAwait(false);
    }
}