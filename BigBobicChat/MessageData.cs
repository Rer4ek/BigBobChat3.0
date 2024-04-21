using System.Collections.Generic;

namespace BigBobicChat
{
    public class MessageData
    {

        public MessageData(int id, int idUser, string name, string time, string text)
        {
            ID = id;
            IDUser = idUser;
            Name = name;
            Time = time;
            Text = text;
        }

        public int ID { get; set; } = 0;

        public int IDUser { get; set; } = 0;

        public string Name { get; set; } = "";

        public string Time { get; set; } = "";

        public string Text { get; set; } = "";


    }
}
