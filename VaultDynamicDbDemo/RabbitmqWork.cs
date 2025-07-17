using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace VaultDynamicDbDemo
{
    public record HelloMessage(string Text);
    public class HelloPublisher
    {
        private readonly RabbitMqConfigService _configService;

        public HelloPublisher(RabbitMqConfigService configService)
        {
            _configService = configService;
        }

        public void SendHello(string text)
        {
            var (user, pass, host) = _configService.GetCredentials();

            var factory = new ConnectionFactory
            {
                HostName = host,
                UserName = user,
                Password = pass,
                VirtualHost = "/"
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare("hello-queue", durable: true, exclusive: false, autoDelete: false);

            var message = new HelloMessage(text);
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            channel.BasicPublish(exchange: "",
                                 routingKey: "hello-queue",
                                 basicProperties: null,
                                 body: body);

            Console.WriteLine("Published: " + text);
        }
    }

    //---------------------------------------------

    public class HelloConsumer
    {
        private readonly RabbitMqConfigService _configService;

        public HelloConsumer(RabbitMqConfigService configService)
        {
            _configService = configService;
        }

        public void StartConsuming()
        {
            var (user, pass, host) = _configService.GetCredentials();

            var factory = new ConnectionFactory
            {
                HostName = host,
                UserName = user,
                Password = pass,
                VirtualHost = "/"
            };

            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.QueueDeclare("hello-queue", durable: true, exclusive: false, autoDelete: false);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = JsonSerializer.Deserialize<HelloMessage>(Encoding.UTF8.GetString(body));
                Console.WriteLine("Received: " + message?.Text);
            };

            channel.BasicConsume(queue: "hello-queue",
                                 autoAck: true,
                                 consumer: consumer);

            Console.WriteLine("Waiting for messages...");
        }
    }

}