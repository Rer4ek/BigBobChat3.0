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
            _user.ConnectedReceived += User_ConectedReceived;
            _user.DisconnectedReceived += User_DisconectedReceived;
            Connected();
        }

        private void User_MessageReceived(MessageData messageData)
        {
            Dispatcher.Invoke(() =>
            {
                var scrollViewer = GetDescendantByType(Messages, typeof(ScrollViewer)) as ScrollViewer;
                bool isAtBottom = scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight;

                Message messageControl = new Message(messageData);

                messageControl.HorizontalAlignment = messageData.Login == _user.Login ? HorizontalAlignment.Right : HorizontalAlignment.Left;
                messageControl.ContextMenuVisibility = messageData.Login == _user.Login ? Visibility.Visible : Visibility.Hidden;

                messageControl.DeleteEvent += DeleteMessage;
                Messages.Items.Add(messageControl);

                if (isAtBottom)
                {
                    scrollViewer.ScrollToEnd();
                }
            });
        }

        private void User_DisconectedReceived(UserData userData)
        {
            Dispatcher.Invoke(() =>
            {
                foreach (ConnectedUsers item in UsersOnline.Items)
                {
                    if (item.Login == userData.Login)
                    {
                        UsersOnline.Items.Remove(item);
                        break;
                    }
                }
            });
        }

        private void User_ConectedReceived(UserData userData)
        {
            Dispatcher.Invoke(() =>
            {
                foreach (ConnectedUsers item in UsersOnline.Items)
                {
                    if (item.Login == userData.Login)
                    {
                        return;
                    }
                }

                ConnectedUsers connectedUser = new ConnectedUsers(userData);
                UsersOnline.Items.Add(connectedUser);
            });
        }

        public static Visual GetDescendantByType(Visual element, Type type)
        {
            if (element == null) return null;
            if (element.GetType() == type) return element;
            Visual foundElement = null;
            if (element is FrameworkElement)
                (element as FrameworkElement).ApplyTemplate();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                Visual visual = VisualTreeHelper.GetChild(element, i) as Visual;
                foundElement = GetDescendantByType(visual, type);
                if (foundElement != null)
                    break;
            }
            return foundElement;
        }


        private async void Message_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await _user.Connection.InvokeAsync(HubEvents.Send, new MessageData("", _user.Username, _user.Login, DateTime.Now.ToString("HH:mm"), MessageText.Text));
                MessageText.Text = "";
            }
        }

        private void Message_GotFocus(object sender, RoutedEventArgs e)
        {
            MessageText.Text = "";
        }

        private async void Connected()
        {
            await _user.Connection.InvokeAsync(HubEvents.Connected, _user.UserData);
        }

        private async void Disconected()
        {
            await _user.Connection.InvokeAsync(HubEvents.Disconnected, _user.UserData);
            await _user.Disconnect();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Disconected();
        }

        public void DeleteMessage(Message message)
        {
            Messages.Items.Remove(message);
        }
    }
}
