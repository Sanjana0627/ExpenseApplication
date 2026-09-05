using ExpenseApplication.Core.Enums;
using ExpenseApplication.Core.Interfaces;
using ExpenseApplication.DTOs;
using ExpenseApplication.Infrastructure.Repositories;
using ExpenseApplication.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;


namespace ExpenseApplication.Controllers
{
    [Authorize(Roles = "Employee")]
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly ExpenseFormService _expenseFormService;
        private readonly IUnitOfWork _unitOfWork;
        //constructor
        public EmployeeController(ExpenseFormService expenseFormService, IUnitOfWork unitOfWork)
        {
            _expenseFormService = expenseFormService;
            _unitOfWork = unitOfWork;
        }
        // creates and submits a new expense form for the logged-in employee
        [HttpPost("expense-forms")]
        public async Task<IActionResult> CreateExpenseForm(CreateExpenseFormRequest request)
        {
            var employeeIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (employeeIdClaim == null || !int.TryParse(employeeIdClaim, out int employeeId))
            {
                return Unauthorized();
            };
            var employee = await _unitOfWork.Users.GetUserByIdAsync(employeeId);
            if (employee?.ManagerId == null)
                return BadRequest(new { message = "You don't have a manager assigned. Contact your Admin." });

            int managerId = employee.ManagerId.Value;
            var lines = request.Expenses
                .Select(e => (e.ExpenseDate, e.Category, e.Purpose, e.VendorName, e.PaymentMethod, e.Amount))
                .ToList();
            var result = await _expenseFormService.CreateExpenseFormAsync(
                employeeId,
                managerId,
                request.CurrencyId, lines);
            if (!result.Success)
            {
                return BadRequest(new { message = result.Error });
            }
            return Ok(new { message = "Expense form created successfully", formId = result.FormId });
        }
        // gets one of the employee's own forms so it can be viewed or edited
        [HttpGet("expense-forms/{id}")]
        public async Task<IActionResult> GetExpenseForm(int id)
        {
            var employeeIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (employeeIdClaim == null || !int.TryParse(employeeIdClaim, out var employeeId))
            {
                return Unauthorized();
            };
            var form = await _expenseFormService.GetFormForEditAsync(id, employeeId);
            if (form == null)
            {
                return NotFound(new { message = "Expense form not found." });
            }
            return Ok(new
            {
                form.FormId,
                form.CurrencyId,
                form.Status,
                form.TotalAmount,
                form.RejectionReason,
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
        // updates a form if edited after rejection or before getting manager approval if the employee wants to edit
        [HttpPut("expense-forms/{id}")]
        public async Task<IActionResult> UpdateExpenseForm(int id, CreateExpenseFormRequest request)
        {
            var employeeIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (employeeIdClaim == null || !int.TryParse(employeeIdClaim, out var employeeId))
            {
                return Unauthorized();
            };
            var lines = request.Expenses
                .Select(e => (e.ExpenseDate, e.Category, e.Purpose, e.VendorName, e.PaymentMethod, e.Amount))
                .ToList();
            var result = await _expenseFormService.UpdateExpenseFormAsync(
                id,
                employeeId,
                request.CurrencyId,
                lines);
            if (!result.Success)
            {
                return BadRequest(new { message = result.Error });
            }
            return Ok(new { message = "Expense form updated successfully" });
        }
        // lists all forms the logged-in employee has submitted, with optional filters
        [HttpGet("expense-forms")]
        public async Task<IActionResult> GetMyExpenseForms([FromQuery] string? status, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            var employeeIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (employeeIdClaim == null || !int.TryParse(employeeIdClaim, out var employeeId))
            {
                return Unauthorized();
            }
            FormStatus? statusFilter = null;
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<FormStatus>(status, true, out var parsedStatus))
                statusFilter = parsedStatus;
            var forms = await _expenseFormService.GetEmployeeFormsAsync(employeeId, statusFilter, fromDate, toDate);
            var result= forms.Select(f => new
            {
                f.FormId,
                f.Status,
                f.TotalAmount,
                f.CurrencyId,
                f.SubmissionDate,
                f.LastUpdatedDate
            });
            return Ok(result);

        }
    }
}

