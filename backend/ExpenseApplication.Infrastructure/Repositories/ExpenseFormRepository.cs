using ExpenseApplication.Core.Entities;
using ExpenseApplication.Core.Enums;
using ExpenseApplication.Core.Interfaces;
using ExpenseApplication.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseApplication.Infrastructure.Repositories
{
    public class ExpenseFormRepository : IExpenseFormRepository
    {
        private readonly ExpenseDbContext _context;
        public ExpenseFormRepository(ExpenseDbContext context)
        {
            _context = context;
        }
        // loads one form with its employee,expenses and history included
        public async Task<ExpenseForm?> GetByIdAsync(int formId) =>
            await _context.ExpenseForms
                .Include(f => f.Employee)
                .Include(f => f.Expenses)
                .Include(f => f.History)
                .FirstOrDefaultAsync(f => f.FormId == formId);

        // gets one employees forms,optionally filtered by status and date range
        public async Task<IEnumerable<ExpenseForm>> GetByEmployeeAsync(int employeeId, FormStatus? status, DateTime? fromDate, DateTime? toDate)
        {
            var query=_context.ExpenseForms
                .Where (f => f.EmployeeId == employeeId)
                .AsQueryable();
            if(status.HasValue)
                query=query.Where(f => f.Status == status.Value);
            if (fromDate.HasValue)
                query = query.Where(f => f.SubmissionDate >= fromDate.Value);
            if (toDate.HasValue)
                query=query.Where(f => f.SubmissionDate < toDate.Value.AddDays(1));
            return await query.OrderByDescending(f => f.SubmissionDate).ToListAsync();
        }

        // queues a new form to be inserted
        public async Task AddAsync(ExpenseForm form) =>
            await _context.ExpenseForms.AddAsync(form);

        // marks an existing form as changed so EF Core saves the update
        public void Update(ExpenseForm form) =>
            _context.ExpenseForms.Update(form);
        // gets the forms waiting for one manager's approval
        public async Task<IEnumerable<ExpenseForm>> GetPendingApprovalForManagerAsync(int managerId, DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.ExpenseForms
                .Include(f => f.Employee)
                .Where(f => f.ManagerId == managerId && f.Status == FormStatus.PendingApproval)
                .AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(f => f.SubmissionDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(f => f.SubmissionDate < toDate.Value.AddDays(1));

            return await query.OrderBy(f => f.SubmissionDate).ToListAsync();
        }
        // gets the managerapproved forms that are waiting to be paid
        public async Task<IEnumerable<ExpenseForm>> GetPendingPaymentAsync(DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.ExpenseForms
                .Include(f => f.Employee)
                .Where(f => f.Status == FormStatus.ManagerApproved)
                .AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(f => f.SubmissionDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(f => f.SubmissionDate < toDate.Value.AddDays(1));

            return await query.OrderBy(f => f.SubmissionDate).ToListAsync();
        }
        // gets every form in the system,optionally filtered by status and date range
        public async Task<IEnumerable<ExpenseForm>> GetAllAsync(FormStatus? status, DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.ExpenseForms
                .Include(f => f.Employee)
                .Include(f => f.Manager)
                .Include(f => f.Expenses)
                .Include(f => f.History)
                .AsQueryable();

            if (status.HasValue)
                query = query.Where(f => f.Status == status.Value);

            if (fromDate.HasValue)
                query = query.Where(f => f.SubmissionDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(f => f.SubmissionDate < toDate.Value.AddDays(1));

            return await query.OrderByDescending(f => f.SubmissionDate).ToListAsync();
        }
        
    }
}

