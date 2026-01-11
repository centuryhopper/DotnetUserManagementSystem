
namespace Shared.Models;

public class ServiceResponses
{
    public record class GeneralResponse(bool Flag, string Message);
    public record class GeneralResponseWithPayload<T>(bool Flag, string Message, T Payload);
    public record class LoginResponse(bool Flag, string Token, string Message);
}