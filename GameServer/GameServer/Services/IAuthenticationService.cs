namespace GameServer.Services;

public interface IAuthenticationService
{
    (bool success, string content) Register(string username, string password);
    (bool success, string token) Login(string username, string password);
}