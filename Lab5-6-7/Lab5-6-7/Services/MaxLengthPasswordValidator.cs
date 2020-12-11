using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Lab5_6_7.Services
{
    public class MaxLengthPasswordValidator<TUser> : IPasswordValidator<TUser> where TUser : IdentityUser
    {
        public Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user, string password)
        {
            if (password.Length > 64)
                return Task.FromResult(IdentityResult.Failed(new IdentityError
                {
                    Code = "Max Length",
                    Description = "Password shouldn't be longer than 64 symbols."
                }));

            return Task.FromResult(IdentityResult.Success);
        }
    }
}
