namespace ERP.Dtos.Authentication
{
    public class LoginResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public TokenResponseDto TokenResponse { get; set; }
    }
}
