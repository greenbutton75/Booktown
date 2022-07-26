using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Identity.HealthChecks
{
    public class IdentityHealthCheck : IHealthCheck
    {
        //private readonly ISearchService _storageService;

        public IdentityHealthCheck(/*ISearchService storageService*/)
        {
            // _storageService = storageService;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            //var IsOK = _storageService.CheckHealth();
            //return new HealthCheckResult(IsOK ? HealthStatus.Healthy : HealthStatus.Unhealthy);
            return new HealthCheckResult(HealthStatus.Healthy);
        }
    }
}
