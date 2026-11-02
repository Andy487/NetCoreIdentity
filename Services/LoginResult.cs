namespace NetCoreIdentity.Services
{
    public enum LoginResult
    {
        Success,
        InvalidCredentials,
        Deactivated,
        LockedOut
    }
}