using Microsoft.Identity.Client;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace UserRetrival
{
    public class GraphApiService
    {
        public static async Task<List<User>> FetchUsersAsync(AuthenticationResult authResult)
        {
            var graphApiEndpoint = "https://graph.microsoft.com/v1.0/users";

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResult.AccessToken);

                var response = await httpClient.GetAsync(graphApiEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var users = JsonConvert.DeserializeObject<UserResponse>(content);

                    return users.Value;
                }
                else
                {
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
