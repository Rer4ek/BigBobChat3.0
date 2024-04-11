using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Npgsql;
using System.Collections.Generic;
using System.Xml.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace BigBobicChat
{
    public class Database : DbContext
    {
        private string _sql = "Server=localhost;Port=5432;Database=Chat;User Id=postgres;Password=123123";

        public async Task<string> LoginAccountAsync(string login, string password)
        {
            using (NpgsqlConnection npgsqlConnectrion = new NpgsqlConnection(_sql))
            {
                await npgsqlConnectrion.OpenAsync();
                using (NpgsqlCommand npgsqlCommand = new NpgsqlCommand($"SELECT \"Name\" FROM users\nWHERE \"Login\" = '{login}' AND \"Password\" = '{password}';", npgsqlConnectrion))
                {
                    NpgsqlDataReader npgsqlDataReader = await npgsqlCommand.ExecuteReaderAsync();
                    if (npgsqlDataReader.HasRows)
                    {
                        npgsqlDataReader.Read();
                        return npgsqlDataReader.GetString(0);
                    }
                }
            }
            return string.Empty;
        }

        public async Task<bool> CreateAccountAsync(string login, string name, string password)
        {
            using (NpgsqlConnection npgsqlConnectrion = new NpgsqlConnection(_sql))
            {
                await npgsqlConnectrion.OpenAsync();
                using (NpgsqlCommand npgsqlCommand = new NpgsqlCommand($"SELECT \"Name\" FROM users\nWHERE \"Login\" = '{login}';", npgsqlConnectrion))
                {
                    NpgsqlDataReader npgsqlDataReader = await npgsqlCommand.ExecuteReaderAsync();
                    if (!npgsqlDataReader.HasRows)
                    {
                        npgsqlDataReader.Close();
                        npgsqlCommand.CommandText = $"INSERT INTO public.users (\n\t\"Login\", \n\t\"Password\", " +
                            $"\n\t\"Name\"\n)\nVALUES \n('{login}', '{password}', '{name}')";
                        await npgsqlCommand.ExecuteNonQueryAsync();
                        return true;
                    }
                }
            }
            return false;
        }

        public async Task AddUserOnineAsync(UserData userData)
        {
            using (NpgsqlConnection npgsqlConnectrion = new NpgsqlConnection(_sql))
            {
                await npgsqlConnectrion.OpenAsync();
                using (NpgsqlCommand npgsqlCommand = new NpgsqlCommand($"SELECT \"Login\" FROM users_online WHERE \"Login\" = '{userData.Login}' ", npgsqlConnectrion))
                {
                    NpgsqlDataReader npgsqlDataReader = await npgsqlCommand.ExecuteReaderAsync();
                    if (!npgsqlDataReader.HasRows)
                    {
                        npgsqlDataReader.Close();
                        npgsqlCommand.CommandText = $"INSERT INTO public.users_online (\n\t\"Login\", \n\t\"Name\"" +
                            $")\nVALUES \n('{userData.Login}', '{userData.Name}')";
                        await npgsqlCommand.ExecuteNonQueryAsync();
                    }
                }
            }
        }

        public async Task DeleteUserOnlineAsync(UserData userData)
        {
            using (NpgsqlConnection npgsqlConnectrion = new NpgsqlConnection(_sql))
            {
                await npgsqlConnectrion.OpenAsync();
                NpgsqlCommand npgsqlCommand = new NpgsqlCommand($"DELETE FROM users_online WHERE \"Login\" = '{userData.Login}'", npgsqlConnectrion);
                await npgsqlCommand.ExecuteNonQueryAsync();
            }
        }


        public async Task<List<UserData>> GetAllUsersOnlineAsync()
        {
            List<UserData> users = new List<UserData>();
            using (NpgsqlConnection npgsqlConnectrion = new NpgsqlConnection(_sql))
            {
                await npgsqlConnectrion.OpenAsync();
                using (NpgsqlCommand npgsqlCommand = new NpgsqlCommand("SELECT \"Login\", \"Name\" FROM users_online", npgsqlConnectrion))
                {
                    NpgsqlDataReader npgsqlDataReader = await npgsqlCommand.ExecuteReaderAsync();
                    while (npgsqlDataReader.Read())
                    {
                        UserData userData = new UserData("0", npgsqlDataReader.GetString(1), npgsqlDataReader.GetString(0));
                        users.Add(userData);
                    }
                }
            }

            return users;
        }


        public async Task<string> AddMessageAsync(MessageData message)
        {
            using (NpgsqlConnection npgsqlConnectrion = new NpgsqlConnection(_sql))
            {
                await npgsqlConnectrion.OpenAsync();
                NpgsqlCommand npgsqlCommand = new NpgsqlCommand($"INSERT INTO public.messages (UserLogin, MessageText, TimeSent) VALUES ('{message.Login}', '{message.Text}', '{message.Time}') RETURNING MessageID", npgsqlConnectrion);
                object? result = await npgsqlCommand.ExecuteScalarAsync();
                return result?.ToString() ?? string.Empty;
            }
        }


        public async Task<List<MessageData>> GetAllMessagesAsync()
        {
            List<MessageData> messages = new List<MessageData>();

            using (NpgsqlConnection npgsqlConnectrion = new NpgsqlConnection(_sql))
            {
                await npgsqlConnectrion.OpenAsync();
                using (NpgsqlCommand npgsqlCommand = new NpgsqlCommand("SELECT m.MessageID, m.UserLogin, u.\"Name\", m.MessageText, m.TimeSent FROM messages m INNER JOIN users u ON m.UserLogin = u.\"Login\"", npgsqlConnectrion))
                {
                    NpgsqlDataReader npgsqlDataReader = await npgsqlCommand.ExecuteReaderAsync();
                    while (npgsqlDataReader.Read())
                    {
                        string id = npgsqlDataReader.GetInt32(0).ToString();
                        string login = npgsqlDataReader.GetString(1);
                        string name = npgsqlDataReader.GetString(2);
                        string text = npgsqlDataReader.GetString(3);
                        string time = npgsqlDataReader.GetString(4);
                        MessageData messageData = new MessageData(id, name, login, time, text);
                        messages.Add(messageData);
                    }
                }
            }

            return messages;
        }



    }
}
