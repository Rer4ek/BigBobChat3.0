namespace Client
{
    public class UserData
    {

        public UserData(int id = 0, string name = "Anonymous")
        {
            ID = id;
            Name = name;
        }

        public int ID { get; set; } = 0;

        public string Name { get; set; } = "";

    }
}
