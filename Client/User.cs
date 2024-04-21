using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Client
{
    public delegate void MessageReceivedHandler(MessageData messageData);

    public delegate void RegisterReceivedHandler(bool answer);

    public delegate void LoginReceivedHandler(UserData userData);

    public delegate void ConnectedReceivedHandler(UserData userData);

    public delegate void DisconnectedReceivedHandler(UserData userData);

    public delegate void DeleteMessageReceivedHandler(MessageData messageData);

    internal class User
    {
        static public User Singelton { get; set; }

        public event MessageReceivedHandler MessageReceived;
        public event RegisterReceivedHandler RegisterReceived;
        public event LoginReceivedHandler LoginReceived;
        public event ConnectedReceivedHandler ConnectedReceived;
        public event DisconnectedReceivedHandler DisconnectedReceived;
        public event DeleteMessageReceivedHandler DeleteMessageReceived;

        public bool IsConnected { get; private set; }
        public string Username { get { return _userData.Name; } set { _userData.Name = value; } }
        public int ID { get { return _userData.ID; } set { _userData.ID = value; } }
        public UserData UserData { get { return _userData; } private set { _userData = value; } }

        public HubConnection Connection { get { return _connection; } }
        private HubConnection _connection;
        private UserData _userData = new UserData();
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
            _connection.On<MessageData>(HubEvents.Receive, (messageData) =>
            {
                MessageReceived?.Invoke(messageData);
            });

            _connection.On<bool>(HubEvents.RegisterReceived, (answer) =>
            {
                RegisterReceived?.Invoke(answer);
            });

            _connection.On<UserData>(HubEvents.LoginReceived, (userData) =>
            {
                LoginReceived?.Invoke(userData);
            });

            _connection.On<UserData>(HubEvents.ConnectedReceived, (userData) =>
            {
                ConnectedReceived?.Invoke(userData);
            });

            _connection.On<UserData>(HubEvents.DisconnectedReceived, (userData) =>
            {
                DisconnectedReceived?.Invoke(userData);
            });

            _connection.On<MessageData>(HubEvents.DeleteMessageReceived, (messageData) =>
            {
                DeleteMessageReceived?.Invoke(messageData);
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

        public async void ChatConnected()
        {
            await _connection.InvokeAsync(HubEvents.Connected, _userData);
        }

        public async void ChatDisconected()
        {
            await _connection.InvokeAsync(HubEvents.Disconnected, _userData);
            await Disconnect();
        }
    }
}
