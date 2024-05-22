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
        private string _sql = "Server=localhost;Port=5432;Database=Chat;User Id=postgres;Password=Moper220";

        public void CreateTables()
        {
            using (NpgsqlConnection npgsqlConnectrion = new NpgsqlConnection(_sql))
            {
                npgsqlConnectrion.Open();
                NpgsqlCommand npgsqlCommand = new NpgsqlCommand("CREATE TABLE IF NOT EXISTS public.users ( id_user serial PRIMARY KEY, login VARCHAR(32) NOT NULL, user_password VARCHAR(32) NOT NULL, username VARCHAR(32) NOT NULL, Icon VARCHAR(128)) TABLESPACE pg_default; ALTER TABLE IF EXISTS public.users OWNER to postgres;", npgsqlConnectrion);
                npgsqlCommand.ExecuteNonQuery();
                npgsqlCommand = new NpgsqlCommand("CREATE TABLE IF NOT EXISTS public.messages (id_message serial PRIMARY KEY, id_user serial REFERENCES users(id_user), text_message text, dispatch_time text) TABLESPACE pg_default; ALTER TABLE IF EXISTS public.messages OWNER to postgres;", npgsqlConnectrion);
                npgsqlCommand.ExecuteNonQuery();
                npgsqlCommand = new NpgsqlCommand("DROP TABLE IF EXISTS public.users_online; CREATE TABLE IF NOT EXISTS public.users_online(id_user_online serial PRIMARY KEY, id_user serial REFERENCES users(id_user)) TABLESPACE pg_default; ALTER TABLE IF EXISTS public.users_online OWNER to postgres;", npgsqlConnectrion);
                npgsqlCommand.ExecuteNonQuery();
            }
        }


        public async Task<UserData?> LoginAccountAsync(string login, string password)
        {
            using (NpgsqlConnection npgsqlConnectrion = new NpgsqlConnection(_sql))
            {
                await npgsqlConnectrion.OpenAsync();
                using (NpgsqlCommand npgsqlCommand = new NpgsqlCommand($"SELECT id_user, username FROM users WHERE login = '{login}' AND user_password = '{password}';", npgsqlConnectrion))
                {
                    NpgsqlDataReader npgsqlDataReader = await npgsqlCommand.ExecuteReaderAsync();
                    if (npgsqlDataReader.HasRows)
                    {
                        await npgsqlDataReader.ReadAsync();
                        UserData userData = new UserData(id: npgsqlDataReader.GetInt32(0), name: npgsqlDataReader.GetString(1));
                        return userData;
                    }
                }
            }
            return null;
        }


        public async Task<bool> CreateAccountAsync(string login, string name, string password)
        {
            using (NpgsqlConnection npgsqlConnectrion = new NpgsqlConnection(_sql))
            {
                await npgsqlConnectrion.OpenAsync();
                using (NpgsqlCommand npgsqlCommand = new NpgsqlCommand($"SELECT username FROM users WHERE login = '{login}';", npgsqlConnectrion))
                {
                    NpgsqlDataReader npgsqlDataReader = await npgsqlCommand.ExecuteReaderAsync();
                    if (!npgsqlDataReader.HasRows)
                    {
                        npgsqlDataReader.Close();
                        npgsqlCommand.CommandText = $"INSERT INTO public.users (login, user_password, username) VALUES ('{login}', '{password}', '{name}')";
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
                using (NpgsqlCommand npgsqlCommand = new NpgsqlCommand($"SELECT id_user FROM users_online WHERE id_user = '{userData.ID}' ", npgsqlConnectrion))
                {
                    NpgsqlDataReader npgsqlDataReader = await npgsqlCommand.ExecuteReaderAsync();
                    if (!npgsqlDataReader.HasRows)
                    {
                        npgsqlDataReader.Close();
                        npgsqlCommand.CommandText = $"INSERT INTO public.users_online (id_user) VALUES ('{userData.ID}')";
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
                NpgsqlCommand npgsqlCommand = new NpgsqlCommand($"DELETE FROM users_online WHERE id_user = '{userData.ID}'", npgsqlConnectrion);
                await npgsqlCommand.ExecuteNonQueryAsync();
            }
        }


        public async Task<List<UserData>> GetAllUsersOnlineAsync()
        {
            List<UserData> users = new List<UserData>();
            using (NpgsqlConnection npgsqlConnectrion = new NpgsqlConnection(_sql))
            {
                await npgsqlConnectrion.OpenAsync();
                using (NpgsqlCommand npgsqlCommand = new NpgsqlCommand("SELECT uo.id_user, u.username FROM users_online uo JOIN users u ON uo.id_user = u.id_user;", npgsqlConnectrion))
                {
                    NpgsqlDataReader npgsqlDataReader = await npgsqlCommand.ExecuteReaderAsync();
                    while (npgsqlDataReader.Read())
                    {
                        UserData userData = new UserData(id: npgsqlDataReader.GetInt32(0), name: npgsqlDataReader.GetString(1));
                        users.Add(userData);
                    }
                }
            }

            return users;
        }


        public async Task<int> AddMessageAsync(MessageData message)
        {
            using (NpgsqlConnection npgsqlConnectrion = new NpgsqlConnection(_sql))
            {
                await npgsqlConnectrion.OpenAsync();
                NpgsqlCommand npgsqlCommand = new NpgsqlCommand($"INSERT INTO public.messages (id_user, text_message, dispatch_time) VALUES ('{message.IDUser}', '{message.Text}', '{message.Time}') RETURNING id_message", npgsqlConnectrion);
                object? result = await npgsqlCommand.ExecuteScalarAsync();
                return (int)(result ?? 0);
            }
        }


        public async Task DeleteMessage(MessageData message)
        {
            using (NpgsqlConnection npgsqlConnectrion = new NpgsqlConnection(_sql))
            {
                await npgsqlConnectrion.OpenAsync();
                NpgsqlCommand npgsqlCommand = new NpgsqlCommand($"DELETE FROM messages WHERE id_message = '{message.ID}'", npgsqlConnectrion);
                await npgsqlCommand.ExecuteNonQueryAsync();
            }
        }


        public async Task<List<MessageData>> GetAllMessagesAsync()
        {
            List<MessageData> messages = new List<MessageData>();

            using (NpgsqlConnection npgsqlConnectrion = new NpgsqlConnection(_sql))
            {
                await npgsqlConnectrion.OpenAsync();
                using (NpgsqlCommand npgsqlCommand = new NpgsqlCommand("SELECT m.id_message, m.id_user, u.username, m.text_message, m.dispatch_time FROM messages m INNER JOIN users u ON m.id_user = u.id_user", npgsqlConnectrion))
                {
                    NpgsqlDataReader npgsqlDataReader = await npgsqlCommand.ExecuteReaderAsync();
                    while (npgsqlDataReader.Read())
                    {
                        int id = npgsqlDataReader.GetInt32(0);
                        int idUser = npgsqlDataReader.GetInt32(1);
                        string name = npgsqlDataReader.GetString(2);
                        string text = npgsqlDataReader.GetString(3);
                        string time = npgsqlDataReader.GetString(4);
                        MessageData messageData = new MessageData(id: id, idUser: idUser, name: name, time: time, text: text);
                        messages.Add(messageData);
                    }
                }
            }

            return messages;
        }



    }
}
