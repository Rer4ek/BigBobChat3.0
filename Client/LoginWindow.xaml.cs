using Microsoft.AspNetCore.SignalR.Client;
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
using System.Windows.Shapes;

namespace Client
{
    public partial class LoginWindow : Window
    {

        private User _user;
        public LoginWindow()
        {
            _user = new User();
            _user.LoginReceived += User_LoginReceived;
            InitializeComponent();
        }

        async private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_user.IsConnected)
            {
                return;
            }
            await _user.Connection.InvokeAsync(HubEvents.Login, UserNameTextBox.Text, UserPasswordBox.Password);
        }

        private void CreateAccount_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!_user.IsConnected)
            {
                return;
            }

            CreateAccountWindow createAccountWindow = new CreateAccountWindow(this);
            createAccountWindow.Show();
            this.Hide();
        }

        private void CreateAccount_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void CreateAccount_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Arrow;
        }

        async private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (UserNameTextBox.Text.Trim() != string.Empty)
            {
                _user.Username = UserNameTextBox.Text;
            }
            await _user.Connect();

            if (!_user.IsConnected)
            {
                ErrorMessage.Text = "Сервер времено недоступен :(\nПопробуйте позже";
                return;
            }

        }

        private void User_LoginReceived(UserData userData)
        {
            Dispatcher.Invoke(() =>
            {
                if (userData != null)
                {
                    _user.Username = userData.Name;
                    _user.ID = userData.ID;

                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    ErrorMessage.Text = "Неверный логин или пароль";
                }
            });
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _user.LoginReceived -= User_LoginReceived;
        }
    }
}
