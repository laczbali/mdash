using System;
using System.Threading.Tasks;
using Dashboard.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Dashboard.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        private readonly ISettingsRepository _settingsRepo;

        public AuthRepository(DataContext context, ISettingsRepository settingsRepo)
        {
            _settingsRepo = settingsRepo;
            // Get the DbContext
            _context = context;
        }

        public async Task<User> Login(string username, string password)
        {
            // Try to get user
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);

            // Invalid user
            if (user == null)
            {
                return null;
            }

            // Invalid password
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                return null;
            }

            // Valid login
            return user;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                // Create password hash with previously saved salt / encryption key
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                // Check byte-by-byte if the computed hash matched the stored hash
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i])
                    {
                        // Invalid password
                        return false;
                    }
                }
            }

            // Valid password
            return true;
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                // Create and store hash salt / encryption key
                passwordSalt = hmac.Key;
                // Create and store hash
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<bool> UserExists(string username)
        {
            // Check if any username matches param username
            if (await _context.Users.AnyAsync(x => x.Username == username))
            {
                return true;
            }

            // Param username is unique
            return false;
        }

        public async Task<bool> IsUsersEmpty()
        {
            var userList = await _context.Users.ToListAsync();
            if(userList.Count == 0) { return true; }
            return false;
        }
    }
}