namespace BigBobicChat
{
    internal static class HubEvents
    {

        public static string Receive { get; set; } = "Receive";

        public static string Send { get; set; } = "Send";

        public static string Register { get; set; } = "Register";

        public static string RegisterReceived { get; set; } = "RegisterReceived";

        public static string Login { get; set; } = "Login";

        public static string LoginReceived { get; set; } = "LoginReceived";

    }
}
