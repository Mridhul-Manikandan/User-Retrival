using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using static UserRetrival.OktaApiService;

namespace UserRetrival
{
    public class OktaApiService
    {
        public static async Task<List<OktaUser>> FetchUsersAsync(string oktaDomain, string AuthorizationHeader)
        {
            var oktaApiEndpoint = $"{oktaDomain}/api/v1/groups";

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("SSWS", AuthorizationHeader);

                var response = await httpClient.GetAsync(oktaApiEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var oktaUsers = JsonConvert.DeserializeObject<List<OktaUser>>(content);

                    var oktaUser = oktaUsers.Select(user => new OktaUser
                    {
                        Id = user.Id,
                        Name = user.Profile?.FirstName,
                        Email = user.Profile?.Email
                    }).ToList();

                    return oktaUser;
                }
                else
                {
                    throw new InvalidOperationException($"Failed to fetch users from Okta API. Status code: {response.StatusCode}");
                }
            }
        }

        public static async Task<List<OktaGroup>> FetchGroupsAsync(string oktaDomain, string AuthorizationHeader)
        {
            var oktaApiEndpoint = $"{oktaDomain}/api/v1/groups";

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("SSWS", AuthorizationHeader);

                var response = await httpClient.GetAsync(oktaApiEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var oktaGroups = JsonConvert.DeserializeObject<List<OktaGroup>>(content);

                    var oktaGroup = oktaGroups.Select(group => new OktaGroup
                    {
                        Id = group.Id,
                        Name = group.Profile?.Name,
                        Type = group.Type
                    }).ToList();

                    return oktaGroup;
                }
                else
                {
                    throw new InvalidOperationException($"Failed to fetch groups from Okta API. Status code: {response.StatusCode}");
                }
            }
        }

        public class OktaUser
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public OktaUserProfile Profile { get; set; }
        }
        public class OktaUserProfile
        {
            public string FirstName { get; set; }
            public string Email { get; set; }
        }

        public class OktaGroup
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
            public OktaGroupProfile Profile { get; set; }
        }

        public class OktaGroupProfile
        {
            public string Name { get; set; }
            public string Description { get; set; }
        }
    }
}
