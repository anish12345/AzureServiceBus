
using Azure.Messaging.ServiceBus;
using AzureServiceBus;
using Microsoft.Azure.Amqp.Framing;
using Newtonsoft.Json;

string connectionString = "Endpoint=sb://anishservicebus.servicebus.windows.net/;SharedAccessKeyName=anishpolicy;SharedAccessKey=KvCEOy95D+Ia5lHWvObCGw1+OYvetwt2blx1D+d8DFI=;EntityPath=anishqueue";

string queueName = "anishqueue";

string[] Importance = new string[] { "High", "Medium", "Low" };

List<Order> orders = new List<Order>()
{
        new Order(){OrderID="01",Quantity=786,UnitPrice=9.99F},
        new Order(){OrderID="02",Quantity=200,UnitPrice=10.99F},
        new Order(){OrderID="03",Quantity=92,UnitPrice=8.99F}

};

await SendMessage(orders);
// await PeekMessages();
//await ReciveMessages();

//await GetProperties();


async Task SendMessage(List<Order> orders)
{
    ServiceBusClient serviceBusClient = new ServiceBusClient(connectionString);
    ServiceBusSender serviceBusSender = serviceBusClient.CreateSender(queueName);

    ServiceBusMessageBatch serviceBusMessageBatch = await serviceBusSender.CreateMessageBatchAsync();
    int i = 0;
    foreach (Order order in orders)
    {

        ServiceBusMessage serviceBusMessage = new ServiceBusMessage(JsonConvert.SerializeObject(order));
        serviceBusMessage.ContentType = "application/json";
       // serviceBusMessage.MessageId = messageId.ToString(); used for duplicate message detection
        //serviceBusMessage.TimeToLive = TimeSpan.FromSeconds(30);
        //serviceBusMessage.ApplicationProperties.Add("Importance", Importance[i]);
        i++;
        if (!serviceBusMessageBatch.TryAddMessage(
            serviceBusMessage))
        {
            throw new Exception("Error occured");
        }

    }
    Console.WriteLine("Sending messages");
    await serviceBusSender.SendMessagesAsync(serviceBusMessageBatch);
    await serviceBusSender.DisposeAsync();
    await serviceBusClient.DisposeAsync();
}

async Task PeekMessages()
{
    ServiceBusClient serviceBusClient = new ServiceBusClient(connectionString);
    ServiceBusReceiver serviceBusReceiver = serviceBusClient.CreateReceiver(queueName,
        new ServiceBusReceiverOptions() { ReceiveMode = ServiceBusReceiveMode.PeekLock });

    IAsyncEnumerable<ServiceBusReceivedMessage> messages = serviceBusReceiver.ReceiveMessagesAsync();

    await foreach (ServiceBusReceivedMessage message in messages)
    {
        Order order = JsonConvert.DeserializeObject<Order>(message.Body.ToString());
        Console.WriteLine("Order Id {0}", order.OrderID);
        Console.WriteLine("Quantity {0}", order.Quantity);
        Console.WriteLine("Unit Price {0}", order.UnitPrice);

    }
}

async Task ReciveMessages()
{
    ServiceBusClient serviceBusClient = new ServiceBusClient(connectionString);
    ServiceBusReceiver serviceBusReceiver = serviceBusClient.CreateReceiver(queueName,
        new ServiceBusReceiverOptions() { ReceiveMode = ServiceBusReceiveMode.ReceiveAndDelete });

    IAsyncEnumerable<ServiceBusReceivedMessage> messages = serviceBusReceiver.ReceiveMessagesAsync();

    await foreach (ServiceBusReceivedMessage message in messages)
    {
        Order order = JsonConvert.DeserializeObject<Order>(message.Body.ToString());
        Console.WriteLine("Order Id {0}", order.OrderID);
        Console.WriteLine("Quantity {0}", order.Quantity);
        Console.WriteLine("Unit Price {0}", order.UnitPrice);

    }
}

async Task GetProperties()
{
    ServiceBusClient serviceBusClient = new ServiceBusClient(connectionString);
    ServiceBusReceiver serviceBusReceiver = serviceBusClient.CreateReceiver(queueName,
        new ServiceBusReceiverOptions() { ReceiveMode = ServiceBusReceiveMode.PeekLock });

    IAsyncEnumerable<ServiceBusReceivedMessage> messages = serviceBusReceiver.ReceiveMessagesAsync();
    await foreach (ServiceBusReceivedMessage message in messages)
    {
        Console.WriteLine("Sequence number {0}", message.SequenceNumber);
        Console.WriteLine("Message Id {0}", message.MessageId);
        Console.WriteLine("Message Importance {0}", message.ApplicationProperties["Importance"]);
    }
}
