namespace ERP.Dtos.Authentication
{
    public enum LoginTypeDto
    {
        Username = 1,
        Email = 2
    }

    public class LoginRequestDto
    {
        public string Login { get; set; }
        public LoginTypeDto LoginType { get; set; }
        public string Password { get; set; }
        public string DeviceId { get; set; }
        public string FcmToken { get; set; }
        public string AppVersion { get; set; }
        public string DeviceName { get; set; }
    }
}
