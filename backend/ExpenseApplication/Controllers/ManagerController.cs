using ExpenseApplication.DTOs;
using ExpenseApplication.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpenseApplication.Controllers
{
    [Authorize(Roles ="Manager")]
    [ApiController]
    [Route("api/[controller]")]
    public class ManagerController : ControllerBase
    {
        private readonly ExpenseFormService _expenseFormService;

        //constructor
        public ManagerController(ExpenseFormService expenseFormService)
        {
            _expenseFormService = expenseFormService;
        }

        // lists the forms waiting for this manager's approval
        [HttpGet("expense-forms")]
        public async Task<IActionResult> GetPendingApprovals([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            var managerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (managerIdClaim == null || !int.TryParse(managerIdClaim, out var managerId))
                return Unauthorized();

            var forms = await _expenseFormService.GetManagerPendingFormsAsync(managerId, fromDate, toDate);

            var result = forms.Select(f => new
            {
                f.FormId,
                f.EmployeeId,
                EmployeeName = f.Employee != null ? f.Employee.FullName : "Unknown",
                f.Status,
                f.TotalAmount,
                f.CurrencyId,
                f.SubmissionDate
            });

            return Ok(result);
        }
        // gets one form's full details so the manager can review it before deciding
        [HttpGet("expense-forms/{id}")]
        public async Task<IActionResult> GetExpenseFormForReview(int id)
        {
            var managerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (managerIdClaim == null || !int.TryParse(managerIdClaim, out var managerId))
                return Unauthorized();

            var form = await _expenseFormService.GetFormForManagerReviewAsync(id, managerId);
            if (form == null)
                return NotFound(new { message = "Expense form not found." });

            return Ok(new
            {
                form.FormId,
                form.EmployeeId,
                EmployeeName = form.Employee != null ? form.Employee.FullName : "Unknown",
                form.CurrencyId,
                form.Status,
                form.TotalAmount,
                Expenses = form.Expenses.Select(e => new
                {
                    e.ExpenseId,
                    e.ExpenseDate,
                    e.Category,
                    e.Purpose,
                    e.VendorName,
                    e.PaymentMethod,
                    e.Amount
                })
            });
        }
        // approves the form so it moves on to the accountant for payment
        [HttpPut("expense-forms/{id}/approve")]
        public async Task<IActionResult> ApproveForm(int id)
        {
            var managerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (managerIdClaim == null || !int.TryParse(managerIdClaim, out var managerId))
                return Unauthorized();

            var result = await _expenseFormService.ApproveFormAsync(id, managerId);

            if (!result.Success)
                return BadRequest(new { message = result.Error });

            return Ok(new { message = "Expense form approved successfully." });
        }
        // rejects the form and records the reason so the employee can see it
        [HttpPut("expense-forms/{id}/reject")]
        public async Task<IActionResult> RejectForm(int id, RejectFormRequest request)
        {
            var managerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (managerIdClaim == null || !int.TryParse(managerIdClaim, out var managerId))
                return Unauthorized();

            var result = await _expenseFormService.RejectFormAsync(id, managerId, request.Reason);

            if (!result.Success)
                return BadRequest(new { message = result.Error });

            return Ok(new { message = "Expense form rejected." });
        }
    }
}
