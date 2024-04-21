using Microsoft.AspNetCore.SignalR;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Text.Json;
using System.Data;

namespace BigBobicChat
{
    public class ChatHub : Hub
    {

        private Database _database = new Database();

        public ChatHub(Database database)
        {
            _database = database;
        }

        public async Task Send(MessageData message)
        {
            message.ID = await _database.AddMessageAsync(message);
            await this.Clients.All.SendAsync(HubEvents.Receive, message);
        }

        public async Task Register(string login, string username, string password)
        {
            bool answer = await _database.CreateAccountAsync(login, username, password);
            await this.Clients.Caller.SendAsync(HubEvents.RegisterReceived, answer);

        }

        public async Task DeleteMessage(MessageData messageData)
        {
            await _database.DeleteMessage(messageData);
            await this.Clients.All.SendAsync(HubEvents.DeleteMessageReceived, messageData);
        }

        public async Task Login(string login, string password)
        {
            UserData? userData = await _database.LoginAccountAsync(login, password);
            await this.Clients.Caller.SendAsync(HubEvents.LoginReceived, userData);
        }

        public async Task Connected(UserData userData)
        {
            await _database.AddUserOnineAsync(userData);
            await this.Clients.Others.SendAsync(HubEvents.ConnectedReceived, userData);

            List<UserData> users = await _database.GetAllUsersOnlineAsync();
            foreach (UserData user in users)
            {
                await this.Clients.Caller.SendAsync(HubEvents.ConnectedReceived, user);
            }

            List<MessageData> messages = await _database.GetAllMessagesAsync();
            foreach (MessageData message in messages)
            {
                await this.Clients.Caller.SendAsync(HubEvents.Receive, message);
            }
        }

        public async Task Disconnected(UserData userData)
        {
            await this.Clients.All.SendAsync(HubEvents.DisconnectedReceived, userData);
            await _database.DeleteUserOnlineAsync(userData);
        }

    }
}
