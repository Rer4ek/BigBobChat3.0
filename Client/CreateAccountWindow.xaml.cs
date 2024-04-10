using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class CreateAccountWindow : Window
    {
        private LoginWindow _loginWindow;
        private User _user;
        public CreateAccountWindow(LoginWindow loginWindow)
        {
            InitializeComponent();

            _loginWindow = loginWindow;

            _user = User.Singelton;
            _user.RegisterReceived += User_RegisterReceived;
            

            this.Closing += WindowClosing;
        }

        public void WindowClosing(object sender, CancelEventArgs e)
        {
            _loginWindow.Show();
            _user.RegisterReceived -= User_RegisterReceived;
        }

        async private void Registration_Click(object sender, RoutedEventArgs e)
        {
            bool allNormal = true;

            WrongLogin.Text = CheckСorrectness(ErrorMessages.EmptyError, CheckEmpty(Login.Text), ref allNormal);

            WrongUsername.Text = CheckСorrectness(ErrorMessages.EmptyError, CheckEmpty(UserName.Text), ref allNormal);

            WrongPassword.Text = CheckСorrectness(ErrorMessages.LessCharacterError, (Password.Password.Length < 8), ref allNormal);

            WrongPasswordMatch.Text = CheckСorrectness(ErrorMessages.MatchError, (Password.Password != PasswordRepeat.Password), ref allNormal);

            WrongPassword.Text = (Password.Password.Contains(" ")) ? ErrorMessages.SpacePasswordError : WrongPassword.Text;
           

            if (allNormal)
            {
                await _user.Connection.InvokeAsync(HubEvents.Register, Login.Text, UserName.Text, Password.Password);
            }

        }

        private string CheckСorrectness(string errorMesage, bool condition, ref bool allNormal)
        {
            if (condition)
            {
                allNormal = false;
                return errorMesage;
            }
            else
            {
                return "";
            }
        }

        private bool CheckEmpty(string text)
        {
            return text.Trim() == string.Empty;
        }

        private void User_RegisterReceived(bool answer)
        {
            Dispatcher.Invoke(() =>
            {
                if(answer)
                {
                    this.Close();
                }
                else
                {
                    RegisterError.Text = ErrorMessages.RegisterError;
                }
            });
        }
    }
}
