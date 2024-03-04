using Microsoft.Identity.Client;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace UserRetrival
{
    public class GraphApiService
    {
        private const string tenantId = "faa01fb7-5567-45f6-975c-e3218091e394";
        private const string clientId = "1a823e0a-6b7a-46de-8145-cd4a4374fe97";
        private const string clientSecret = "bmv8Q~S-51Sw-Kc4CSe5DPYpr3kLicMA~kSsbauH";
        private const string authority = $"https://login.microsoftonline.com/{tenantId}" + $"/v2.0";
        private const string graphScope = "https://graph.microsoft.com/.default";
        private const string graphEndpoint = "https://graph.microsoft.com/v1.0/users";
        private const string appId = "1a823e0a-6b7a-46de-8145-cd4a4374fe97";

        public async Task<List<User>> FetchUsersAsync()
        {
            var app = ConfidentialClientApplicationBuilder.Create(clientId)
           .WithClientSecret(clientSecret)
           .WithAuthority(new Uri(authority))
           .Build();

            var authResult = await app.AcquireTokenForClient(new[] { graphScope }).ExecuteAsync();

            var graphApiEndpoint = $"https://graph.microsoft.com/v1.0/applications/{appId}/appRoleAssignedTo";

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResult.AccessToken);

                var response = await httpClient.GetAsync(graphApiEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(content);
                    var users = JsonConvert.DeserializeObject<UserResponse>(content);

                    return users.Value;
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Content: {content}");
                    throw new InvalidOperationException($"Failed to fetch users from Microsoft Graph API. Status code: {response.StatusCode}");
                }
            }
        }

        public class UserResponse
        {
            public List<User> Value { get; set; }
        }

        public class User
        {
            public string Id { get; set; }
            public string DisplayName { get; set; }
            public string Mail { get; set; }
            public bool AccountEnabled { get; set; }

        }

        public static async Task<List<Group>> FetchGroupsAsync(AuthenticationResult authResult)
        {
            var graphApiEndpoint = "https://graph.microsoft.com/v1.0/groups";

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResult.AccessToken);

                var response = await httpClient.GetAsync(graphApiEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var groups = JsonConvert.DeserializeObject<GroupResponse>(content);

                    return groups.Value;
                }
                else
                {
                    throw new InvalidOperationException($"Failed to fetch groups from Microsoft Graph API. Status code: {response.StatusCode}");
                }
            }
        }

        public class GroupResponse
        {
            public List<Group> Value { get; set; }
        }

        public class Group
        {
            public string Id { get; set; }
            public string DisplayName { get; set; }
            public string Mail { get; set; }
            public string MailNickname { get; set; }
        }

    }
}
