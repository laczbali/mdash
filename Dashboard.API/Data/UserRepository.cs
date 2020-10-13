using System.Collections.Generic;
using System.Threading.Tasks;
using Dashboard.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Dashboard.API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        public UserRepository(DataContext context)
        {
            _context = context;
        }

        public void DeleteUser(User user)
        {
            _context.Remove(user);
            _context.SaveChanges();
        }

        public async Task<User> GetUser(string username)
        {
            var user = await _context.Users.Include(s => s.UserSettings).FirstOrDefaultAsync(u => u.Username == username);
            return user;
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();
            return users;
        }

        public async Task<bool> IsAdmin(string username)
        {
           var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
           if(user.UserLevel == AccessLevel.admin){
               return true;
           }
           return false;
        }
    }
}