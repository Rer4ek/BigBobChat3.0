namespace BigBobicChat
{
    public class UserData
    {

        public UserData(string id, string name, string login)
        {
            ID = id;
            Name = name;
            Login = login;
        }

        public string ID { get; set; } = "";

        public string Name { get; set; } = "";

        public string Login { get; set; } = "";

    }
}
