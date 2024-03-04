using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;


namespace UserRetrival
{
    public class AzureADAuthentication
    {
        public static async Task<AuthenticationResult> AuthenticateAsync(IConfiguration configuration, string[] scopes)
        {
            var tenantId = "faa01fb7-5567-45f6-975c-e3218091e394";
            var clientId = "1a823e0a-6b7a-46de-8145-cd4a4374fe97";
            var clientSecret = "bmv8Q~S-51Sw-Kc4CSe5DPYpr3kLicMA~kSsbauH";
            var authority = $"https://login.microsoftonline.com/{tenantId}" + $"/v2.0";
            var graphScope = "https://graph.microsoft.com/.default";
            var graphEndpoint = "https://graph.microsoft.com/v1.0/users";
            var appId = "1a823e0a-6b7a-46de-8145-cd4a4374fe97";

            var app = ConfidentialClientApplicationBuilder.Create(clientId)
                .WithClientSecret(clientSecret)
                .WithAuthority(new Uri(authority))
                .Build();

            return await app.AcquireTokenForClient(new[] { graphScope }).ExecuteAsync();
        }
    }
}
