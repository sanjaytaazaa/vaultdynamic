using Microsoft.Extensions.Diagnostics.HealthChecks;
using MassTransit;

namespace VaultDynamicDbDemo
{
    //public class DynamicRabbitMqSqlHealthCheck : IHealthCheck
    //{
    //    private readonly IBusHealth _busHealth;

    //    public DynamicRabbitMqSqlHealthCheck(IBusHealth busHealth)
    //    {
    //        _busHealth = busHealth;
    //    }

    //    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    //    {
    //        var healthResult = _busHealth.CheckHealth();

    //        if (healthResult.Status == HealthStatus.Healthy)
    //        {
    //            return Task.FromResult(HealthCheckResult.Healthy("MassTransit bus is healthy"));
    //        }

    //        return Task.FromResult(HealthCheckResult.Unhealthy("MassTransit bus is unhealthy"));
    //    }
    //}
}