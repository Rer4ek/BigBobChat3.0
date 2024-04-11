namespace Client
{
    public class UserData
    {

        public UserData(string id = "Anonymous", string name = "Anonymous", string login = "")
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
