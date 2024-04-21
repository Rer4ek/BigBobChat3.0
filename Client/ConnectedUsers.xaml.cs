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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client
{

    public partial class ConnectedUsers : UserControl
    {

        public int IDUser { get; set; } = 0;

        public string UserName
        { 
            get { return NameUser.Text; }
            set { NameUser.Text = value; }
        }

        public ImageSource UserAvatar
        {
            get { return UserImage.ImageSource; }
            set {  UserImage.ImageSource = value; }
        }
        

        public ConnectedUsers()
        {
            InitializeComponent();
        }

        public ConnectedUsers(UserData userData)
        {
            InitializeComponent();

            UserName = userData.Name;
            IDUser = userData.ID;
        }
    }
}
