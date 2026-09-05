using ExpenseApplication.Core.Entities;
using ExpenseApplication.Core.Interfaces;
using ExpenseApplication.Infrastructure.Data;

namespace ExpenseApplication.Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ExpenseDbContext _context;

        public RoleRepository(ExpenseDbContext context)
        {
            _context = context;
        }

        // looks up a role by its id
        public async Task<Role?> GetByIdAsync(int roleId) =>
            await _context.Roles.FindAsync(roleId);
    }
}
