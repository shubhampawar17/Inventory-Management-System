namespace IMS.Contracts
{
    public record LoginRequest(string Username, string Password);

    public record LoginResponse(bool IsAuthenticated, string Message, string Username);
}
