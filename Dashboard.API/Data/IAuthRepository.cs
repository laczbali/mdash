using System.Threading.Tasks;
using Dashboard.API.Models;

namespace Dashboard.API.Data
{
    public interface IAuthRepository
    {
        /// <summary>
        /// Stores a new User in the DB with the password hashed and salted.
        /// </summary>
        /// <param name="user">The user to store</param>
        /// <param name="password">The plain-text password to hash, then store.</param>
        /// <returns></returns>
        Task<User> Register(User user, string password);

        /// <summary>
        /// If the given user exists, and the password hashes match, then returns the User object. Otherwise it returns null.
        /// </summary>
        /// <param name="username">The username to log in</param>
        /// <param name="password">The plain-text password to hash, then compare</param>
        /// <returns>A User object if login is successful, null if not.</returns>
        Task<User> Login (string username, string password);

        /// <summary>
        /// Returns true if a user with the given username is already in the DB. Returns false otherwise.
        /// </summary>
        /// <param name="username">The username to check if it's in the DB.</param>
        /// <returns></returns>
        Task<bool> UserExists (string username);

        /// <summary>
        /// Checks to see if there are no users in the DB
        /// </summary>
        /// <returns></returns>
        Task<bool> IsUsersEmpty();
    }
}