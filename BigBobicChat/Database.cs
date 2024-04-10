using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Npgsql;
using System.Xml.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace BigBobicChat
{
    public class Database : DbContext
    {
        string sql = "Server=localhost;Port=5432;Database=Chat;User Id=postgres;Password=Moper220";
       // public DbSet<User> Users { get; set; } = null!;

        /*public ApplicationContext()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=usersdb;Username=postgres;Password=пароль_от_postgres");
        }*/


        public string SqlAccount(string login, string name, string password)
        {
            NpgsqlConnection npgsqlConnectrion = new NpgsqlConnection(sql);
            NpgsqlCommand npgsqlCommand = new NpgsqlCommand();
            npgsqlConnectrion.Open();

            npgsqlCommand.Connection = npgsqlConnectrion;
            npgsqlCommand.CommandType = System.Data.CommandType.Text;

            if (name == "")
            {
                name = SqlLogInAccount(login, password, npgsqlCommand);
            }
            else if (name != "")
            {
                SqlCreateAccount(login, name, password, npgsqlCommand);
            }

            npgsqlCommand.Dispose();
            npgsqlConnectrion.Close();

            return name;
        }

        public string SqlLogInAccount(string login, string password, NpgsqlCommand npgsqlCommand)
        {
            string name = "";
            npgsqlCommand.CommandText = $"SELECT \"Name\" FROM users\r\nWHERE \"Login\" = '{login}' AND \"Password\" = '{password}';";
            NpgsqlDataReader npgsqlDataReader = npgsqlCommand.ExecuteReader();
            if (npgsqlDataReader.HasRows)
            {
                npgsqlDataReader.Read();
                name = npgsqlDataReader.GetString(0);
            }
            npgsqlDataReader.Close();
            return name;
        }

        public void SqlCreateAccount(string login, string name, string password, NpgsqlCommand npgsqlCommand)
        {
            npgsqlCommand.CommandText = $"SELECT \"Name\" FROM users\r\nWHERE \"Login\" = '{login}';";

            NpgsqlDataReader npgsqlDataReader = npgsqlCommand.ExecuteReader();
            if (!npgsqlDataReader.HasRows)
            {
                npgsqlCommand.CommandText = $"INSERT INTO public.users (\r\n\t\"Login\", \r\n\t\"Password\", " +
                    $"\r\n\t\"Name\"\r\n)\r\nVALUES \r\n('{login}', '{password}', '{name}')";
                npgsqlCommand.ExecuteNonQuery();
            }
            npgsqlDataReader.Close();
        }
    }
}
