using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;


namespace UserRetrival
{
    public class AzureADAuthentication
    {
        public static async Task<AuthenticationResult> AuthenticateAsync(IConfiguration configuration, string[] scopes)
        {
            var clientId = configuration["AzureAD:ClientId"];
            var tenantId = configuration["AzureAD:TenantId"];
            var clientSecret = configuration["AzureAD:ClientSecret"];
            var authority = $"https://login.microsoftonline.com/{tenantId}";

            var confidentialClientApplication = ConfidentialClientApplicationBuilder
                .Create(clientId)
                .WithClientSecret(clientSecret)
                .WithAuthority(new Uri(authority))
                .Build();

            return await confidentialClientApplication.AcquireTokenForClient(scopes)
                .ExecuteAsync();
        }
    }
}
