using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public delegate void MessageReceivedHandler(string user, string message);

    public delegate void RegisterReceivedHandler(bool answer);

    public delegate void LoginReceivedHandler(string login, string username, bool correct);

    internal class User
    {
        static public User Singelton { get; set; }

        public event MessageReceivedHandler MessageReceived;
        public event RegisterReceivedHandler RegisterReceived;
        public event LoginReceivedHandler LoginReceived;

        public bool IsConnected { get; private set; }
        public string Username { get; set; } = "Anonymous";
        public string Login { get; set; } = "Anonymous";

        public HubConnection Connection { get { return _connection; } }
        private HubConnection _connection;
        private string _url = "https://localhost:7268/chat";

        public User()
        {
            Singelton = this;
        }

        async public Task Connect()
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            _connection = new HubConnectionBuilder()
                .WithUrl(_url, options => { options.HttpMessageHandlerFactory = _ => handler; })
                .Build();

            CommunicationMethods();

            try
            {
                await _connection.StartAsync();
                IsConnected = true;
            }
            catch (Exception)
            {
                IsConnected = false;
            }
        }

        public void CommunicationMethods()
        {
            _connection.On<string, string>(HubEvents.Receive, (user, message) =>
            {
                MessageReceived?.Invoke(user, message);
            });

            _connection.On<bool>(HubEvents.RegisterReceived, (answer) =>
            {
                RegisterReceived?.Invoke(answer);
            });

            _connection.On<string, string, bool>(HubEvents.LoginReceived, (login, username, correct) =>
            {
                LoginReceived?.Invoke(login, username, correct);
            });
        }

        public async Task Disconnect()
        {
            if (_connection != null)
            {
                await _connection.StopAsync();
                IsConnected = false;
            }
        }
    }
}
