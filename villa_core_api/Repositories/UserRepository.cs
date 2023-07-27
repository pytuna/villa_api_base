using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using VillaApi.Entities;
using VillaApi.Interfaces;
using VillaApi.Models;

namespace VillaApi.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ModelAppContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<ApplicationUser> _siginManager;

        public UserRepository(
            ModelAppContext db, 
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration
        )
        {
            _db = db;
            this._userManager = userManager;
            this._configuration = configuration;
            this._siginManager = signInManager;
        }

        public async Task<string> SignInAsync(SignInModel model)
        {
            ApplicationUser user = await this._userManager.FindByEmailAsync(model.Email);
            
            if(!await _userManager.IsEmailConfirmedAsync(user)){
                return string.Empty;
            }

            var result = await this._siginManager
                .PasswordSignInAsync(user.UserName, model.Password, false, false);
            
            if(!result.Succeeded){
                // Lock user account
                await this._userManager.AccessFailedAsync(user);
                return string.Empty;
            }

            var authClaim = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.Email),
                new Claim(ClaimTypes.NameIdentifier, model.Email),
                new Claim(JwtRegisteredClaimNames.Jti , Guid.NewGuid().ToString()),
            };
            var secret = Encoding.UTF8.GetBytes(this._configuration["JWT:Secret"]!);

            var token = new JwtSecurityToken(
                issuer: this._configuration["JWT:ValidIssuer"],
                audience: this._configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaim,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256)
            );
            
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<IdentityResult> SignUpAsync(SignUpModel model)
        {
            var user = new ApplicationUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.UserName,
            };
            
            return await this._userManager.CreateAsync(user, model.Password);
        }

        
        public async Task SignOutAsync()
        {
            await this._siginManager.SignOutAsync();
        }
    }
}