using ExpenseApplication.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpenseApplication.Controllers
{
    [Authorize(Roles ="Accountant")]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountantController : ControllerBase
    {
        private readonly ExpenseFormService _expenseFormService;

        //constructor
        public AccountantController(ExpenseFormService expenseFormService)
        {
            _expenseFormService = expenseFormService;
        }

        // lists the forms that a manager already approved and are waiting to be paid
        [HttpGet("expense-forms")]
        public async Task<IActionResult> GetPendingPayments([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            var forms = await _expenseFormService.GetPendingPaymentFormsAsync(fromDate, toDate);

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
        // marks a managerapproved form as paid
        [HttpPut("expense-forms/{id}/pay")]
        public async Task<IActionResult> PayForm(int id)
        {
            var accountantIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(accountantIdClaim == null||!int.TryParse(accountantIdClaim,out var accountantId))
                return Unauthorized();
            var result=await _expenseFormService.PayFormAsync(id,accountantId);
            if(!result.Success)
                return BadRequest(new { message = result.Error });
            return Ok(new {message= "Expense form marked as paid." });
        }
        // gets one form's full details so the accountant can review it before paying
        [HttpGet("expense-forms/{id}")]
        public async Task<IActionResult> GetExpenseFormForReview(int id)
        {
            var form = await _expenseFormService.GetFormForAccountantReviewAsync(id);
            if (form == null)
                return NotFound(new { message = "Expense form not found or awaiting payment." });
            return Ok(new
            {
                form.FormId,
                form.EmployeeId,
                EmployeeName = form.Employee != null ? form.Employee.FullName : "UnKnown",
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


    }

}
