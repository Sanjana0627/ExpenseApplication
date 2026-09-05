using ExpenseApplication.Core.Entities;
using ExpenseApplication.Core.Enums;
using ExpenseApplication.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseApplication.Services
{
    // the main workflow logic: create/update a form, then approve/reject/pay it
    public class ExpenseFormService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ExpenseFormService> _logger;
        private static readonly string[] ValidCategories = { "Taxi", "Food", "Gas", "Travel", "Accommodation", "Other" };
        private const decimal MaxExpenseAmount = 5000m;

        public ExpenseFormService(IUnitOfWork unitOfWork,ILogger<ExpenseFormService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /* Shared by CreateExpenseFormAsync and UpdateExpenseFormAsync so the amount/category
         rules only have to be maintained in one place. checkCategories stays a parameter
         (rather than always on) so both callers keep behaving exactly as they did before.*/
        private (bool Success, string? Error) ValidateExpenseLines(
            List<(DateTime Date, string Category, string Purpose, string VendorName, string PaymentMethod, decimal Amount)> lines,
            bool checkCategories)
        {
            if (lines.Count == 0)
                return (false, "A form must contain at least one expense.");

            foreach (var line in lines)
            {
                if (line.Amount <= 0 || line.Amount > MaxExpenseAmount)
                    return (false, $"Expense amount {line.Amount} is invalid. Must be greater than 0 and no more than {MaxExpenseAmount}.");

                if (checkCategories && !ValidCategories.Contains(line.Category))
                    return (false, $"'{line.Category}' is not a valid category. Choose from: {string.Join(", ", ValidCategories)}.");
            }

            var totalRequested = lines.Sum(l => l.Amount);
            if (totalRequested > MaxExpenseAmount)
                return (false, $"Total form amount {totalRequested} exceeds the maximum allowed of {MaxExpenseAmount}.");

            return (true, null);
        }

        // validates the expense lines, builds a new form in PendingApproval status,
        // and logs a "Created" history entry
        public async Task<(bool Success,string? Error,int? FormId)> CreateExpenseFormAsync(
            int employeeId,int managerId, int currencyId, List<(DateTime Date, string Category, string Purpose, string VendorName, string PaymentMethod, decimal Amount)> lines)
        {
            var validation = ValidateExpenseLines(lines, checkCategories: true);
            if (!validation.Success)
                return (false, validation.Error, null);

            var form =new ExpenseForm
            {
                EmployeeId = employeeId,
                ManagerId = managerId,
                CurrencyId = currencyId,
                SubmissionDate = DateTime.UtcNow,
                Status = FormStatus.PendingApproval,
                LastUpdatedDate= DateTime.UtcNow,
            };
            foreach (var line in lines)
            {
                form.Expenses.Add(new Expense
                {
                    ExpenseDate = line.Date,
                    Category = line.Category,
                    Purpose = line.Purpose,
                    VendorName = line.VendorName,
                    PaymentMethod = line.PaymentMethod,
                    Amount = line.Amount
                });
            }
            form.TotalAmount = form.Expenses.Sum(e => e.Amount);
            form.History.Add(new ExpenseFormHistory
            {
                Action="Created",
                PerformedBy=employeeId,
                Comments="Form submitted by employee.",
                ActionDate=DateTime.UtcNow
            });
            await _unitOfWork.ExpenseForms.AddAsync(form);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Expense form {FormId} created by employee {EmployeeId}, total {TotalAmount}", form.FormId, employeeId, form.TotalAmount);
            return (true,null,form.FormId);
        }

        // replaces a form's expense lines (only allowed while Pending or Rejected),
        // puts it back in PendingApproval and logs an "Updated" history entry
        public async Task<(bool Success,string? Error)> UpdateExpenseFormAsync(
            int formId,int employeeId,int currencyId,
           List<(DateTime Date, string Category, string Purpose, string VendorName, string PaymentMethod, decimal Amount)> lines)
        {
            var form = await _unitOfWork.ExpenseForms.GetByIdAsync(formId);
            if(form==null)
                return (false,"Expense form not found.");
            if(form.EmployeeId!=employeeId)
                return (false,"You are not authorized to update this form.");
            if (form.Status != FormStatus.PendingApproval && form.Status != FormStatus.Rejected)
                return (false, $"This form cannot be edited because its status is '{form.Status}'.");

            var validation = ValidateExpenseLines(lines, checkCategories: false);
            if (!validation.Success)
                return (false, validation.Error);

            form.Expenses.Clear();
            foreach (var line in lines)
            {
                form.Expenses.Add(new Expense
                {
                    ExpenseDate = line.Date,
                    Category = line.Category,
                    Purpose = line.Purpose,
                    VendorName = line.VendorName,
                    PaymentMethod = line.PaymentMethod,
                    Amount = line.Amount
                });
            }
            form.CurrencyId= currencyId;
            form.TotalAmount = form.Expenses.Sum(e => e.Amount);
            form.Status= FormStatus.PendingApproval;
            form.LastUpdatedDate= DateTime.UtcNow;
            form.RejectionReason = null;
            form.History.Add(new ExpenseFormHistory
            {
                Action="Updated",
                PerformedBy=employeeId,
                Comments="Form updated by employee.",
                ActionDate=DateTime.UtcNow
            });
            _unitOfWork.ExpenseForms.Update(form);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Expense form {FormId} updated by employee {EmployeeId}", formId, employeeId);
            return (true,null);
        }

        // gets a form for the owning employee to view or edit - returns null if it's not theirs
        public async Task<ExpenseForm?> GetFormForEditAsync(int formId, int employeeId)
        {
            var form = await _unitOfWork.ExpenseForms.GetByIdAsync(formId);

            if (form == null || form.EmployeeId != employeeId)
                return null;

            return form;
        }

        // lists one employee's forms, with optional status/date filters
        public async Task<IEnumerable<ExpenseForm>> GetEmployeeFormsAsync(int employeeId, FormStatus? status, DateTime? fromDate, DateTime? toDate)
        {
            return await _unitOfWork.ExpenseForms.GetByEmployeeAsync(employeeId, status, fromDate, toDate);
        }

        // lists the forms waiting for one manager's approval
        public async Task<IEnumerable<ExpenseForm>> GetManagerPendingFormsAsync(int managerId, DateTime? fromDate, DateTime? toDate)
        {
            return await _unitOfWork.ExpenseForms.GetPendingApprovalForManagerAsync(managerId, fromDate, toDate);
        }

        // lists the manager-approved forms that are waiting to be paid
        public async Task<IEnumerable<ExpenseForm>> GetPendingPaymentFormsAsync(DateTime? fromDate, DateTime? toDate)
        {
            return await _unitOfWork.ExpenseForms.GetPendingPaymentAsync(fromDate, toDate);
        }

        // lists every form in the system, with optional status/date filters (used by Admin)
        public async Task<IEnumerable<ExpenseForm>> GetAllFormsAsync(FormStatus? status, DateTime? fromDate, DateTime? toDate)
        {
            return await _unitOfWork.ExpenseForms.GetAllAsync(status, fromDate, toDate);
        }

        // gets a form for review by the manager it's assigned to - returns null otherwise
        public async Task<ExpenseForm?> GetFormForManagerReviewAsync(int formId, int managerId)
        {
            var form = await _unitOfWork.ExpenseForms.GetByIdAsync(formId);

            if (form == null || form.ManagerId != managerId)
                return null;

            return form;
        }

        // approves a pending form (must belong to this manager), moving it on to the accountant
        public async Task<(bool Success,string? Error)> ApproveFormAsync(int formId, int managerId)
        {
            var form = await _unitOfWork.ExpenseForms.GetByIdAsync(formId);
            if(form==null)
                return (false,"Expense form not found.");
            if(form.ManagerId!=managerId)
                return (false, "You are not authorized to act on this form.");
            if(form.Status!=FormStatus.PendingApproval)
                return (false,$"This form cannot be approved because its status is '{form.Status}'.");
            form.Status= FormStatus.ManagerApproved;
            form.LastUpdatedDate= DateTime.UtcNow;
            form.History.Add(new ExpenseFormHistory
            {
                Action = "Approved",
                PerformedBy = managerId,
                Comments = "Approved by manager.",
                ActionDate = DateTime.UtcNow
            });
            _unitOfWork.ExpenseForms.Update(form);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Expense form {FormId} approved by manager {ManagerId}", formId, managerId);
            return (true,null);
        }

        // rejects a pending form with a required reason, so the employee can see why
        public async Task<(bool Success,string? Error)> RejectFormAsync(int formId, int managerId, string reason)
        {
            var form = await _unitOfWork.ExpenseForms.GetByIdAsync(formId);
            if(form==null)
                return (false,"Expense form not found.");
            if(form.ManagerId!=managerId)
                return (false, "You are not authorized to act on this form.");
            if(form.Status!=FormStatus.PendingApproval)
                return (false,$"This form cannot be rejected because its status is '{form.Status}'.");
            if(string.IsNullOrWhiteSpace(reason))
                return (false,"A rejection reason must be provided.");
            form.Status= FormStatus.Rejected;
            form.RejectionReason= reason;
            form.LastUpdatedDate= DateTime.UtcNow;
            form.History.Add(new ExpenseFormHistory
            {
                Action = "Rejected",
                PerformedBy = managerId,
                Comments = reason,
                ActionDate = DateTime.UtcNow
            });
            _unitOfWork.ExpenseForms.Update(form);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Expense form {FormId} rejected by manager {ManagerId}: {Reason}", formId, managerId, reason);
            return (true,null);
        }

        // marks a manager-approved form as paid (no real payment happens, just a status change)
        public async Task<(bool Success,string? Error)> PayFormAsync(int formId,int accountantId)
        {
            var form=await _unitOfWork.ExpenseForms.GetByIdAsync(formId);
            if (form==null)
                return (false,"Expense form not found.");
            if(form.Status!=FormStatus.ManagerApproved)
                return (false,$"This form cannot be paid because its status is '{form.Status}'.");
            form.Status= FormStatus.Paid;
            form.LastUpdatedDate= DateTime.UtcNow;
            form.History.Add(new ExpenseFormHistory
            {
                Action = "Paid",
                PerformedBy = accountantId,
                Comments = "Form marked as paid by accountant.",
                ActionDate = DateTime.UtcNow
            });
            _unitOfWork.ExpenseForms.Update(form);
             await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Expense form {FormId} marked as paid by accountant {AccountantId}", formId, accountantId);
            return (true,null);
        }

        // gets a form for the accountant to review - only returns it if it's ready to be paid
        public async Task<ExpenseForm?> GetFormForAccountantReviewAsync(int formId)
        {
            var form = await _unitOfWork.ExpenseForms.GetByIdAsync(formId);
            if (form == null || form.Status != FormStatus.ManagerApproved)
                return null;
            return form;
        }
    }
}

