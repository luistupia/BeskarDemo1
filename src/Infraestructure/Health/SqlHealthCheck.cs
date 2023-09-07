using Infraestructure.Common.Interfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Infraestructure.Health;

public class SqlHealthCheck : IHealthCheck
{
    private readonly IDbConnectionFactory _connectionFactory;

    public SqlHealthCheck(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = new CancellationToken())
    {
        try
        {
            using var con = _connectionFactory.NorthwindDbConnection();
            using var cmd = con.CreateCommand();
            con.Open();
            cmd.CommandText = "Select 1";
            cmd.ExecuteScalar();
            con.Close();
            return Task.FromResult(HealthCheckResult.Healthy());
        }
        catch (Exception e)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy(exception: e));
        }
    }
}