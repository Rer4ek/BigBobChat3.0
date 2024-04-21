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
    public partial class Message : UserControl
    {

        public delegate void DeleteHandler(Message message);
        public event DeleteHandler DeleteEvent;

        public Message()
        {
            InitializeComponent();
            MessageTime = DateTime.Now.ToLocalTime().ToString("HH:mm");
        }

        public Message(MessageData messageData)
        {
            InitializeComponent();

            ID = messageData.ID;
            MessageUserName = messageData.Name;
            Text = messageData.Text;
            MessageTime = messageData.Time;
            IDUser = messageData.IDUser;
        }

        public int IDUser { get; set; } = 0;

        public int ID { get; set; } = 0;

        [Browsable(true)]
        public string Text
        {
            get { return MessageText.Text; }
            set { MessageText.Text = value; }
        }

        [Browsable(true)]
        public Visibility ContextMenuVisibility
        {
            get { return Menu.Visibility; }
            set { Menu.Visibility = value; }
        }

        [Browsable(true)]
        public string MessageTime
        {
            get { return Time.Text; }
            set { Time.Text = value; }
        }

        [Browsable(true)]
        public string MessageUserName
        {
            get { return NameUser.Text; }
            set { NameUser.Text = value; }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            DeleteEvent?.Invoke(this);
        }
    }
}
