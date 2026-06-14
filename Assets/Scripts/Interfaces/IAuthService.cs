using System.Threading.Tasks;

public interface IAuthService
{
    string CurrentUserId { get; }
    bool IsLoggedIn { get; }
    
    Task<bool> Login(string email, string password);
    Task<bool> Register(string email, string password);
    void Logout();
}
