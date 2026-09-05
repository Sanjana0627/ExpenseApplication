using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseApplication.Core.Interfaces
{
    public interface IUnitOfWork: IDisposable
    {
        IUserRepository Users { get; }
        IExpenseFormRepository ExpenseForms { get; }
        // commits every pending change made through the repositories above
        Task<int> SaveChangesAsync();
        IRoleRepository Roles { get; }
    }
}
