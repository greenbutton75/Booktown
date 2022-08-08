using Amazon.AspNetCore.Identity.Cognito;
using Amazon.Extensions.CognitoAuthentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Identity.HealthChecks
{
    public class IdentityHealthCheck : IHealthCheck
    {
        private readonly UserManager<CognitoUser> _userManager;
        private readonly CognitoUserPool _pool;

        public IdentityHealthCheck(UserManager<CognitoUser> userManager, CognitoUserPool pool)
        {
            _userManager = userManager;
            _pool = pool;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            //var IsOK = _storageService.CheckHealth();
            //return new HealthCheckResult(IsOK ? HealthStatus.Healthy : HealthStatus.Unhealthy);
            try
            {
                var user = _pool.GetUser("fakeuser");
                var cognitoUser = await _userManager.SetPhoneNumberAsync (user, "+12125199999"); // Id Cognito works it will return error in cognitoUser object
            }
            catch (Exception)
            {
                return new HealthCheckResult(HealthStatus.Unhealthy);
            }

            return new HealthCheckResult(HealthStatus.Healthy);
        }
    }
}
