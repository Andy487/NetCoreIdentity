namespace NetCoreIdentity.Services
{
    public interface IAccountService
    {
        Task<LoginResult> LoginAsync(string email, string password, bool rememberMe);
    }
}