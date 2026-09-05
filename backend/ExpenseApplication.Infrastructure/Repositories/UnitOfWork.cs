using ExpenseApplication.Core.Entities;
using ExpenseApplication.Core.Interfaces;
using ExpenseApplication.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseApplication.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ExpenseDbContext _context;
        public IUserRepository Users { get; }
        public IExpenseFormRepository ExpenseForms { get; }

        public IRoleRepository Roles { get; }

        // creates each repository sharing the one DbContext instance
        public UnitOfWork(ExpenseDbContext context)
        {
            _context = context;
            Users = new UserRepository(context);
            ExpenseForms = new ExpenseFormRepository(context);
            Roles = new RoleRepository(context); 
        }

        // saves every pending change made through the repositories above
        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

        // releases the underlying DbContext
        public void Dispose() => _context.Dispose();

    }
}
