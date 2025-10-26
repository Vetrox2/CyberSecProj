using backend.Models;

namespace backend.Mappers
{
    public static class UserMapper
    {
        public static UserDto ToDto(this User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Login = user.Login,
                Name = user.Name,
                MustChangePassword = user.MustChangePassword,
                IsBlocked = user.IsBlocked,
                PasswordValidTo = user.PasswordValidTo,
                RequirePasswordRules = user.RequirePasswordRules,
                RoleId = user.RoleId,
            };
        }
    }
}
