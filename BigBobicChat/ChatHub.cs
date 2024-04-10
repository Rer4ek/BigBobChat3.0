using Microsoft.AspNetCore.SignalR;
using System.IO;
using System.Threading.Tasks;

namespace BigBobicChat
{
    public class ChatHub : Hub
    {
        Database database = new Database();

        public async Task Send(string username, string message)
        {
            await this.Clients.All.SendAsync(HubEvents.Receive, username, message);
        }

        public async Task Register(string login, string username, string password)
        {
            database.SqlAccount(login, username, password);
            await this.Clients.Caller.SendAsync(HubEvents.RegisterReceived, true);

        }

        public async Task Login(string login, string password)
        {
            string name = database.SqlAccount(login, "", password);

            if (name != "") 
            {
                await this.Clients.Caller.SendAsync(HubEvents.LoginReceived, login, name, true);
                return;
            }

            await this.Clients.Caller.SendAsync(HubEvents.LoginReceived, "", "", false);
        }

    }
}
