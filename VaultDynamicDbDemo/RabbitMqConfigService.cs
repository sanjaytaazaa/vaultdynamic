namespace VaultDynamicDbDemo
{
    public class RabbitMqConfigService
    {
        private readonly IConfiguration _config;

        public RabbitMqConfigService(IConfiguration config)
        {
            _config = config;
        }

        public string GetConnectionString()
        {
            var user = _config["rabbit_username"];
            var pass = _config["rabbit_password"];
            var host = "rabbitmq-shared.rabbitmq-shared.svc.cluster.local";
            return $"amqp://{user}:{pass}@{host}/";
        }
    }

}
