using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class ActionsService(AppDbContext db) : IActionsService
    {
        public async Task<bool> CanEditFileAsync(Guid userId)
        {
            var userAction = await db.Actions.FirstOrDefaultAsync(a => a.UserId == userId);

            if (userAction == null)
                userAction = await AddUserActionRecord(userId);

            if (userAction.IsActivated)
                return true;

            if (userAction.Counter >= 5)
            {
                return false;
            }

            userAction.Counter++;
            await db.SaveChangesAsync();

            return true;
        }

        private async Task<Models.Action> AddUserActionRecord(Guid userId)
        {
            var userAction = new Models.Action
            {
                UserId = userId,
                IsActivated = false,
                Counter = 0
            };
            db.Actions.Add(userAction);
            await db.SaveChangesAsync();
            return userAction;
        }

        public string GenerateUnlockKey(Guid userId)
        {
            var encryptedKey = CaesarCipher.GenerateUnlockKey(userId);
            return encryptedKey;
        }

        public async Task<bool> ValidateUnlockKeyAsync(Guid userId, string encryptedKey)
        {
            if (!CaesarCipher.ValidateUnlockKey(encryptedKey, userId))
                return false;

            var userAction = await db.Actions.FirstOrDefaultAsync(a => a.UserId == userId);

            if (userAction == null)
            {
                userAction = new Models.Action
                {
                    UserId = userId,
                    IsActivated = true,
                    Counter = 0
                };
                db.Actions.Add(userAction);
            }
            else
            {
                userAction.IsActivated = true;
            }

            await db.SaveChangesAsync();
            return true;
        }
    }
}
