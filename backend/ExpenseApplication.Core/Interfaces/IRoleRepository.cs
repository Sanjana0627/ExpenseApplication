using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpenseApplication.Core.Entities;

namespace ExpenseApplication.Core.Interfaces
{
    public interface IRoleRepository
    {
        // looks up a role by its id -used the role name in jwt token
        Task<Role?> GetByIdAsync(int roleId);
    }
}
