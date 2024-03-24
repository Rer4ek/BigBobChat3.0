using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http;

namespace Client
{

    public partial class MainWindow : Window
    {
        HubConnection connection;
        public MainWindow()
        {
            InitializeComponent();

            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            connection = new HubConnectionBuilder()
                .WithUrl("https://192.168.1.113:7268/chat", options =>
                {
                    options.HttpMessageHandlerFactory = _ => handler;
                })
                .Build();

            connection.On<string, string>("Receive", (user, message) =>
            {
                Dispatcher.Invoke(() =>
                {
                    var newMessage = $"{user}: {message}";
                    MessageListBox.Items.Insert(0, newMessage);
                });
            });
        }

        private async void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await connection.InvokeAsync("Send", NameField.Text, Message.Text);
            }
        }

        private async void Connect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await connection.StartAsync();
                Connect.IsEnabled = false;
                await connection.InvokeAsync("Send", NameField.Text, " - Подключился");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection fail");
            }
        }

    }
}
