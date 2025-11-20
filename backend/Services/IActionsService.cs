namespace backend.Services
{
    public interface IActionsService
    {
        Task<bool> CanEditFileAsync(Guid userId);
        string GenerateUnlockKey(Guid userId);
        Task<bool> ValidateUnlockKeyAsync(Guid userId, string encryptedKey);
    }
}
