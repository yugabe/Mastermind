using Mastermind.Api.Data;
using Mastermind.Api.Data.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Mastermind.Api.Services
{
    public class UserService
    {
        public IHttpContextAccessor HttpContextAccessor { get; }
        public MastermindDbContext DbContext { get; }

        public UserService(IHttpContextAccessor httpContextAccessor, MastermindDbContext dbContext)
        {
            HttpContextAccessor = httpContextAccessor;
            DbContext = dbContext;
        }

        public Guid GetCurrentUserId() => Guid.TryParse(HttpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier), out var result) ? result : Guid.Empty;

        public async Task<Guid> RegisterAsync(string username, string password)
        {
            var salt = new byte[128 / 8];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);

            var user = new User
            {
                Username = username,
                PasswordHash = Convert.ToBase64String(salt.Concat(KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA1, 10000, 256 / 8)).ToArray())
            };
            DbContext.Add(user);
            await DbContext.SaveChangesAsync();
            return user.Id;
        }

        public async Task<Guid?> CheckPasswordAsync(string username, string password)
        {
            var user = await DbContext.Users.SingleOrDefaultAsync(u => u.Username == username);
            if (user == null)
                return null;
            var passwordHash = Convert.FromBase64String(user.PasswordHash);
            return KeyDerivation.Pbkdf2(password, passwordHash[..(128 / 8)], KeyDerivationPrf.HMACSHA1, 10000, 256 / 8).SequenceEqual(passwordHash[(128 / 8)..]) ? (Guid?)user.Id : null;
        }

        public async Task HttpCookieSignInAsync(Guid userId) => 
            await HttpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) }, "basic")), new AuthenticationProperties
                {
                    AllowRefresh = true,
                    IssuedUtc = DateTimeOffset.UtcNow,
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(2)
                });

        public async Task HttpCookieSignOutAsync() =>
            await HttpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }
}
