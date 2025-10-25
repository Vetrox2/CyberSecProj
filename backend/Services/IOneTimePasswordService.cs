using backend.Models;

namespace backend.Services
{
    public interface IOneTimePasswordService
    {
        Task<double> GenerateOneTimePasswordAsync(string userId);
        Task<bool> VerifyOneTimePasswordAsync(User user, double password);
    }
}
