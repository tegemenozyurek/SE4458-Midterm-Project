using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text;

namespace API_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private const string PaymentQueue = "PaymentQueue";
        private const string NotificationQueue = "NotificationQueue";

        [HttpPost("send-payment")]
        public IActionResult SendPayment([FromBody] PaymentRequest payment)
        {
            if (payment == null || string.IsNullOrEmpty(payment.CardNo))
                return BadRequest("Invalid payment details.");

            var factory = new ConnectionFactory()
            {
                Uri = new Uri("amqps://tiadikyh:quHV4JVkzGOMIKWhTmQ8HxYLMPXaQwLC@armadillo.rmq.cloudamqp.com/tiadikyh")
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            // Payment Queue Declare
            channel.QueueDeclare(PaymentQueue, durable: true, exclusive: false, autoDelete: false);

            var message = System.Text.Json.JsonSerializer.Serialize(payment);
            var body = Encoding.UTF8.GetBytes(message);

            // Send Payment Message
            channel.BasicPublish(exchange: "", routingKey: PaymentQueue, basicProperties: null, body: body);

            return Ok("Payment sent to queue successfully.");
        }

        [HttpPost("process-payment")]
        public IActionResult ProcessPayment()
        {
            var factory = new ConnectionFactory()
            {
                Uri = new Uri("amqps://tiadikyh:quHV4JVkzGOMIKWhTmQ8HxYLMPXaQwLC@armadillo.rmq.cloudamqp.com/tiadikyh")
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            // Consume Payment Queue
            var consumer = new RabbitMQ.Client.Events.EventingBasicConsumer(channel);
            channel.QueueDeclare(PaymentQueue, durable: true, exclusive: false, autoDelete: false);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Processing Payment: {message}");

                // After processing, send notification
                channel.QueueDeclare(NotificationQueue, durable: true, exclusive: false, autoDelete: false);
                var notificationMessage = "Your payment has been received.";
                var notificationBody = Encoding.UTF8.GetBytes(notificationMessage);

                channel.BasicPublish(exchange: "", routingKey: NotificationQueue, basicProperties: null, body: notificationBody);
            };

            channel.BasicConsume(queue: PaymentQueue, autoAck: true, consumer: consumer);

            return Ok("Payment processing started.");
        }
    }

    public class PaymentRequest
    {
        public string User { get; set; }
        public string PaymentType { get; set; }
        public string CardNo { get; set; }
    }
}
