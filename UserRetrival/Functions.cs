using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using static UserRetrival.GraphApiService;
using static UserRetrival.OktaApiService;

namespace UserRetrival
{
    public class Functions
    {
        private readonly IConfiguration _configuration;

        public Functions(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        [FunctionName("Data_Retrieving")]
        public async Task Data_Retrieving(
            [TimerTrigger("*/10 * * * * *")] TimerInfo myTimer)
        {
            try
            {
                Console.WriteLine($"C# Timer trigger function executed at: {DateTime.Now}");

                var scope = new[] { "https://graph.microsoft.com/.default" };
                var oktaDomain = _configuration["Okta:Domain"];
                var oktaApiToken = _configuration["Okta:AuthorizationHeader"];
                var connectionString = _configuration["Database:ConnectionString"];

                var authResult = await AzureADAuthentication.AuthenticateAsync(_configuration, scope);
                var oktaGroups = await OktaApiService.FetchGroupsAsync(oktaDomain, oktaApiToken);

                var users = await FetchUsersAsync(authResult);
                var groups = await FetchGroupsAsync(authResult);

                PrintUsers(users);
                PrintGroups(groups);


                PrintOktaUsers(oktaGroups);

                if (TestDatabaseConnection(connectionString))
                {
                    Console.WriteLine("Database connection successful.");
                    await DatabaseService.StoreUsersAsync(connectionString, users);
                    await DatabaseService.StoreGroupsAsync(connectionString, groups);
                    await DatabaseService.StoreOktaGroupsAsync(connectionString, oktaGroups);

                    Console.WriteLine($"Data processed at {DateTime.Now}");
                }
                else
                {
                    Console.WriteLine("Database connection failed.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing data: {ex.Message}");
            }
        }

        private void PrintUsers(List<User> users)
        {
            Console.WriteLine("Retrieved Users:");
            foreach (var user in users)
            {
                Console.WriteLine($"User ID: {user.Id}, DisplayName: {user.DisplayName}, Mail: {user.Mail}, IsActive: {user.AccountEnabled}");
            }
        }

        private void PrintGroups(List<Group> groups)
        {
            Console.WriteLine("Retrieved Groups:");
            foreach (var group in groups)
            {
                Console.WriteLine($"Group ID: {group.Id}, DisplayName: {group.DisplayName}, Mail: {group.Mail ?? "N/A"}, MailNickname: {group.MailNickname ?? "N/A"}");
            }
        }

        private void PrintOktaUsers(List<OktaGroup> oktaUsers)
        {
            Console.WriteLine("Retrieved Okta Users:");
            foreach (var user in oktaUsers)
            {
                Console.WriteLine($"Okta Group ID: {user.Id}, Okta GroupName: {user.Name}, Okta Type: {user.Type}");
            }
        }

        private bool TestDatabaseConnection(string connectionString)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error testing database connection: {ex.Message}");
                return false;
            }
        }
    }
}
