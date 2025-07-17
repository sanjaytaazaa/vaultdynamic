using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;

namespace VaultDynamicDbDemo
{
    public class DynamicNpgSqlHealthCheck : IHealthCheck
    {
        private readonly IConfiguration _configuration;

        public DynamicNpgSqlHealthCheck(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            var connectionString = $"Host=hippo-dev-primary-service.postgresql.svc.cluster.local;Port=5432;Database=hippo;Username={_configuration["db_username"]};Password={_configuration["db_password"]};";

            try
            {
                using var conn = new NpgsqlConnection(connectionString);
                await conn.OpenAsync(cancellationToken);
                return HealthCheckResult.Healthy("Database connection successful");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Database connection failed", ex);
            }
        }
    }

}
