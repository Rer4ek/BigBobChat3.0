using Microsoft.AspNetCore.SignalR;
using System.IO;
using System.Threading.Tasks;

namespace BigBobicChat
{
    public class ChatHub : Hub
    {
        public async Task Send(string username, string message)
        {
            await this.Clients.All.SendAsync(HubEvents.Receive, username, message);
        }

        public async Task Register(string login, string username, string password)
        {
            string path = $"accounts/{login}.txt";

            if (File.Exists(path))
            {
                await this.Clients.Caller.SendAsync(HubEvents.RegisterReceived, false);
            }
            else
            {
                using (StreamWriter sw = File.CreateText(path))
                {
                    await sw.WriteLineAsync(login);
                    await sw.WriteLineAsync(username);
                    await sw.WriteLineAsync(password);
                }

                await this.Clients.Caller.SendAsync(HubEvents.RegisterReceived, true);
            }
        }

        public async Task Login(string login, string password)
        {
            string path = $"accounts/{login}.txt";

            if (File.Exists(path))
            {
                using (StreamReader sr = File.OpenText(path))
                {
                    string? loginFromFile = await sr.ReadLineAsync();
                    string? usernameFromFile = await sr.ReadLineAsync();
                    string? passwordFromFile = await sr.ReadLineAsync();

                    if (password == passwordFromFile)
                    {
                        await this.Clients.Caller.SendAsync(HubEvents.LoginReceived, loginFromFile, usernameFromFile, true);
                        return;                    
                    }
                    
                }
            }
            await this.Clients.Caller.SendAsync(HubEvents.LoginReceived, "", "", false);
        }

    }
}
