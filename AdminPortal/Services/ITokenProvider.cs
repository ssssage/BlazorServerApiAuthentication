namespace AdminPortal.Services
{
    public interface ITokenProvider
    {
        Task<string> GetTokenAsync(string email, string password);
       
    }
}
