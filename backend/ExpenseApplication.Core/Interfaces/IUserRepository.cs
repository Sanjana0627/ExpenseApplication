using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpenseApplication.Core.Entities;

namespace ExpenseApplication.Core.Interfaces
{
    public interface IUserRepository
    {
        // looks up a user by their id
        Task<User?> GetUserByIdAsync(int userId);
        // looks up a user by their username
        Task<User?> GetUsernameAsync(string username);
        // gets everyone who reports to a given manager
        Task<IEnumerable<User>> GetDirectReportsAsync(int managerId);
    }
}
