using Microsoft.Extensions.Diagnostics.HealthChecks;
using RabbitMQ.Client;

namespace VaultDynamicDbDemo
{
    public class RabbitMqHealthCheck : IHealthCheck
    {
        private readonly RabbitMqConfigService _configService;

        public RabbitMqHealthCheck(RabbitMqConfigService configService)
        {
            _configService = configService;
        }
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,CancellationToken cancellationToken = default)
        {
            try
            {
                var (user, pass, host) = _configService.GetCredentials();

                var factory = new ConnectionFactory
                {
                    HostName = host,
                    UserName = user,
                    Password = pass,
                    VirtualHost = "/",
                    RequestedConnectionTimeout = TimeSpan.FromSeconds(5)
                };

                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();

                // Try passive check to see if queue exists (optional)
                channel.QueueDeclarePassive("hello-queue");

                return Task.FromResult(HealthCheckResult.Healthy("RabbitMQ is reachable"));
            }
            catch (Exception ex)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy("RabbitMQ unreachable", ex));
            }
        }
    }
}