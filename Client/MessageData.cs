using System.Collections.Generic;

namespace Client
{
    public class MessageData
    {

        public MessageData(string id, string name, string login, string time, string text)
        {
            ID = id;
            Name = name;
            Login = login;
            Time = time;
            Text = text;
        }

        public string ID { get; set; } = "";

        public string Name { get; set; } = "";

        public string Login { get; set; } = "";

        public string Time { get; set; } = "";

        public string Text { get; set; } = "";


    }
}
