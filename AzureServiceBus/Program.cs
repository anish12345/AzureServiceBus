
using Azure.Messaging.ServiceBus;
using AzureServiceBus;
using Newtonsoft.Json;

string connectionString = "Endpoint=sb://anishservicebus.servicebus.windows.net/;SharedAccessKeyName=anishpolicy;SharedAccessKey=KvCEOy95D+Ia5lHWvObCGw1+OYvetwt2blx1D+d8DFI=;EntityPath=anishqueue";

string queueName = "anishqueue";

List<Order> orders = new List<Order>()
{
        new Order(){OrderID="01",Quantity=786,UnitPrice=9.99F},
        new Order(){OrderID="02",Quantity=200,UnitPrice=10.99F},
        new Order(){OrderID="03",Quantity=92,UnitPrice=8.99F}

};

await SendMessage(orders);


async Task SendMessage(List<Order> orders)
{
    ServiceBusClient serviceBusClient = new ServiceBusClient(connectionString);
    ServiceBusSender serviceBusSender = serviceBusClient.CreateSender(queueName);

    ServiceBusMessageBatch serviceBusMessageBatch = await serviceBusSender.CreateMessageBatchAsync();
    foreach (Order order in orders)
    {

        ServiceBusMessage serviceBusMessage = new ServiceBusMessage(JsonConvert.SerializeObject(order));
        serviceBusMessage.ContentType = "application/json";
        if (!serviceBusMessageBatch.TryAddMessage(
            serviceBusMessage))
        {
            throw new Exception("Error occured");
        }

    }
    Console.WriteLine("Sending messages");
    await serviceBusSender.SendMessagesAsync(serviceBusMessageBatch);
}
