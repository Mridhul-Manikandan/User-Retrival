﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using static UserRetrival.GraphApiService;

namespace UserRetrival
{
    public class DatabaseService
    {
        public static async Task StoreUsersAsync(string connectionString, List<User> users)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                foreach (var user in users)
                {
                    await InsertUserAsync(connection, user);
                }
            }
        }

        private static async Task InsertUserAsync(SqlConnection connection, User user)
        {
            using (var command = new SqlCommand(
                "INSERT INTO Users (UserId, UserName, MailId, IsActive, Access_Provider) " +
                "VALUES (@UserId, @UserName, @MailId, @IsActive, @AccessProvider)", connection))
            {
                command.Parameters.AddWithValue("@UserId", user.Id);
                command.Parameters.AddWithValue("@UserName", user.DisplayName);
                command.Parameters.AddWithValue("@MailId", user.Mail ?? "");
                command.Parameters.AddWithValue("@IsActive", user.AccountEnabled);
                command.Parameters.AddWithValue("@AccessProvider", "Microsoft Graph");

                await command.ExecuteNonQueryAsync();
            }
        }

        public static async Task StoreGroupsAsync(string connectionString, List<Group> groups)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                foreach (var group in groups)
                {
                    await InsertGroupAsync(connection, group);
                }
            }
        }

        private static async Task InsertGroupAsync(SqlConnection connection, Group group)
        {
            using (var command = new SqlCommand(
                "INSERT INTO Groups (GroupId, GroupName, MailId, MailNickname, Access_Provider) " +
                "VALUES (@GroupId, @GroupName, @MailId, @MailNickname, @AccessProvider)", connection))
            {
                command.Parameters.AddWithValue("@GroupId", group.Id);
                command.Parameters.AddWithValue("@GroupName", group.DisplayName);
                command.Parameters.AddWithValue("@MailId", group.Mail ?? "-");
                command.Parameters.AddWithValue("@MailNickname", group.MailNickname ?? "-");
                command.Parameters.AddWithValue("@AccessProvider", "Microsoft Graph");

                await command.ExecuteNonQueryAsync();
            }
        }

    }
}
