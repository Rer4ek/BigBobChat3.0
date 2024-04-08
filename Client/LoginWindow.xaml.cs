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
            InitializeComponent();
        }

        async private void LoginButton_Click(object sender, RoutedEventArgs e)
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
            
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
