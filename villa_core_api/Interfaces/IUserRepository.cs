

using Microsoft.AspNetCore.Identity;
using VillaApi.Models;

namespace VillaApi.Interfaces
{
    public interface IUserRepository
    {
        public Task<string> SignInAsync(SignInModel model);
        public Task<IdentityResult> SignUpAsync(SignUpModel model);
        public Task SignOutAsync();
    }
}