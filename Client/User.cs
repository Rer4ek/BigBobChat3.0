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

    internal class User
    {
        static public User Singelton { get; set; }
        public bool IsConnected { get; private set; }
        public string Username { get; set; } = "Anonymous";
        public HubConnection Connection { get { return _connection; } }

        public event MessageReceivedHandler MessageReceived;


        private HubConnection _connection;

        private string _url = "https://192.168.1.113:7268/chat";

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

            _connection.On<string, string>(HubEvents.Receive, (user, message) =>
            {
                MessageReceived?.Invoke(user, message);
            });

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
