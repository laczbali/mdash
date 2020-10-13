using System.Collections.Generic;
using System.Threading.Tasks;
using Dashboard.API.Models;

namespace Dashboard.API.Data
{
    public interface IUserRepository
    {
        void DeleteUser(User user);
        Task<User> GetUser(string username);
        Task<IEnumerable<User>> GetAllUsers();
        Task<bool> IsAdmin(string username);
    }
}