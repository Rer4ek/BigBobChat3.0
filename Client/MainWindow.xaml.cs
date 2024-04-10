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
                var scrollViewer = GetDescendantByType(MessageListBox, typeof(ScrollViewer)) as ScrollViewer;
                bool isAtBottom = scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight;

                string newMessage = $"{user}: {message}";
                MessageListBox.Items.Add(newMessage);

                if (isAtBottom)
                {
                    scrollViewer.ScrollToEnd();
                }
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
                await _user.Connection.InvokeAsync(HubEvents.Send, _user.Username, Message.Text);
                Message.Text = "";
            }
        }

        private void Message_GotFocus(object sender, RoutedEventArgs e)
        {
            Message.Text = "";
        }
    }
}
