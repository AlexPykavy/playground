using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Demo.AlternatingHealthCheck.HealthChecks;

public class AlternatingHealthCheck : IHealthCheck
{
    private static int Counter = 0;

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var isHealthy = (Counter / 10) % 2 == 0;

        Interlocked.Increment(ref Counter);

        if (isHealthy)
        {
            return Task.FromResult(
                HealthCheckResult.Healthy("A healthy result."));
        }

        return Task.FromResult(
            HealthCheckResult.Unhealthy("An unhealthy result."));
    }
}