using MassTransit;

namespace VaultDynamicDbDemo
{
    public record HelloMessage(string Text);
    public class HelloMessageConsumer : IConsumer<HelloMessage>
    {
        public Task Consume(ConsumeContext<HelloMessage> context)
        {
            Console.WriteLine($"Received message: {context.Message.Text}");
            return Task.CompletedTask;
        }
    }
    public class HelloPublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public HelloPublisher(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task SendHello(string text)
        {
            await _publishEndpoint.Publish(new HelloMessage(text));
            Console.WriteLine("Published message: " + text);
        }
    }

}