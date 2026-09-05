using ExpenseApplication.Core.Entities;
using ExpenseApplication.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseApplication.Core.Interfaces
{
    public interface IExpenseFormRepository
    {
        // gets one form by id, with its employee/expenses/history loaded
        Task<ExpenseForm?> GetByIdAsync(int formId);
        // gets the forms belonging to one employee, with optional status/date filters
        Task<IEnumerable<ExpenseForm>> GetByEmployeeAsync(int employeeId, FormStatus? status, DateTime? fromDate, DateTime? toDate);
        // adds a new form to the database (not saved until SaveChangesAsync)
        Task AddAsync(ExpenseForm form);
        // marks an existing form as modified so EF Core will save the changes
        void Update(ExpenseForm form);
        // gets the forms waiting for one manager's approval
        Task<IEnumerable<ExpenseForm>> GetPendingApprovalForManagerAsync(int managerId, DateTime? fromDate, DateTime? toDate);
        // gets the manager-approved forms that are waiting to be paid
        Task<IEnumerable<ExpenseForm>> GetPendingPaymentAsync(DateTime? fromDate, DateTime? toDate);
        // gets every form in the system, with optional status/date filters
        Task<IEnumerable<ExpenseForm>> GetAllAsync(FormStatus? status, DateTime? fromDate, DateTime? toDate);
    }
}
