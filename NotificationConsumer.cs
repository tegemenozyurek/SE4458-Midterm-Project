using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

class NotificationConsumer
{
    private const string NotificationQueue = "NotificationQueue";

    static void Main(string[] args)
    {
        var factory = new ConnectionFactory()
        {
            Uri = new Uri("amqps://tiadikyh:quHV4JVkzGOMIKWhTmQ8HxYLMPXaQwLC@armadillo.rmq.cloudamqp.com/tiadikyh")
        };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(queue: NotificationQueue, durable: true, exclusive: false, autoDelete: false);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($"Notification: {message}");
        };

        channel.BasicConsume(queue: NotificationQueue, autoAck: true, consumer: consumer);

        Console.WriteLine("Listening for notifications...");
        Console.ReadLine();
    }
}
