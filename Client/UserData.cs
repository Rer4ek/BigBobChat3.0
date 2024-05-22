namespace Client
{
    public class UserData
    {

        public UserData(int id = 0, string name = "Anonymous", string userIcon = "user.jpg")
        {
            ID = id;
            Name = name;
            UserIcon = userIcon;
        }

        public int ID { get; set; } = 0;

        public string Name { get; set; } = "";

        public string UserIcon { get; set; } = "";
    }
}
