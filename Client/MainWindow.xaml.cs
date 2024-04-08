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
using System.Data.Common;
using System.Collections.ObjectModel;

namespace Client
{

    public partial class MainWindow : Window
    {
        private User _user;

        public MainWindow()
        {
            InitializeComponent();

            _user = User.Singelton;
            _user.MessageReceived += User_MessageReceived;
            _user.Connection.InvokeAsync(HubEvents.Send, _user.Username, " - Подключился");
        }

        private void User_MessageReceived(string user, string message)
        {
            Dispatcher.Invoke(() =>
            {
                var newMessage = $"{user}: {message}";
                MessageListBox.Items.Add(newMessage);
            });
        }

        private async void Message_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await _user.Connection.InvokeAsync(HubEvents.Send, _user.Username, Message.Text);
            }
        }
    }
}
