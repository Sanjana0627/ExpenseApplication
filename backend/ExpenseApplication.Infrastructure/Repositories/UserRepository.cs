using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpenseApplication.Core.Entities;
using ExpenseApplication.Core.Interfaces;
using ExpenseApplication.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ExpenseApplication.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ExpenseDbContext _context;
        public UserRepository(ExpenseDbContext context)
        {
            _context = context;
        }

        // looks up a user by their id
        public async Task<User?> GetUserByIdAsync(int userId) =>
            await _context.Users.FindAsync(userId);

        // looks up a user by their username
        public async Task<User?> GetUsernameAsync(string username) =>
            await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);

        // gets everyone who reports to a given manager
        public async Task<IEnumerable<User>> GetDirectReportsAsync(int managerId) =>
            await _context.Users.Where(u => u.ManagerId == managerId).ToListAsync();
    }
}
