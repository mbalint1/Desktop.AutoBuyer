using AutoBuyer.API.POC.Models;

namespace AutoBuyer.API.POC.Providers
{
    public interface IAuthProvider
    {
        AuthResponse Authenticate(string user, string password);
    }
}