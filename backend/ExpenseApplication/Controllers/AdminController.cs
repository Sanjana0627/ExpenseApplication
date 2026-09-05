using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ExpenseApplication.Services;
using ExpenseApplication.Services.Reports;
using ExpenseApplication.Core.Enums;


namespace ExpenseApplication.Controllers
{
    [Authorize(Roles ="Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly ExpenseFormService _expenseFormService;
        private readonly AdminReportService _reportService;

        // constructor
        public AdminController(ExpenseFormService expenseFormService, AdminReportService reportService)
        {
            _expenseFormService = expenseFormService;
            _reportService = reportService;
        }
        // lists every expense form with optional status/date filters
        [HttpGet("expense-forms")]
        public async Task<IActionResult> GetAllExpenseForms(
            [FromQuery] string? status, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            FormStatus? statusFilter = null;
            if(!string.IsNullOrEmpty(status)&&Enum.TryParse<FormStatus>(status, true, out var parsedStatus))
               statusFilter = parsedStatus;
            var forms=await _expenseFormService.GetAllFormsAsync(statusFilter, fromDate, toDate);
            var result = forms.Select(f => new
            {
                f.FormId,
                f.EmployeeId,
                EmployeeName = f.Employee != null ? f.Employee.FullName : "Unknown",
                f.ManagerId,
                ManagerName = f.Manager != null ? f.Manager.FullName : "Unknown",
                f.Status,
                f.TotalAmount,
                f.CurrencyId,
                f.SubmissionDate,
                f.LastUpdatedDate,
                f.RejectionReason,
                Expenses = f.Expenses.Select(e => new
                {
                    e.ExpenseId,
                    e.ExpenseDate,
                    e.Purpose,
                    e.Amount
                })
            });
            return Ok(result);
        }
        // report: how many forms are in each status and counts status which are same
        [HttpGet("reports/status-breakdown")]
        public async Task<IActionResult> GetStatusBreakdown()
        {
            var data = await _reportService.GetStatusBreakdownAsync();
            return Ok(data);
        }
        // report: how many forms were submitted per month
        [HttpGet("reports/monthly-form-count")]
        public async Task<IActionResult> GetMonthlyFormCount()
        {
            var data = await _reportService.GetMonthlyFormCountAsync();
            return Ok(data);
        }
        // report: approve vs reject rate for each manager
        [HttpGet("reports/rejection-rate-by-manager")]
        public async Task<IActionResult> GetRejectionRateByManager()
        {
            var data = await _reportService.GetRejectionRateByManagerAsync();
            return Ok(data);
        }

        // report: average time between a form being submitted and getting a decision
        [HttpGet("reports/average-turnaround")]
        public async Task<IActionResult> GetAverageTurnaround()
        {
            var data = await _reportService.GetAverageTurnaroundAsync();
            return Ok(data);
        }

        // report: how many expenses fall under each category
        [HttpGet("reports/expense-count-by-category")]
        public async Task<IActionResult> GetExpenseCountByCategory()
        {
            var data = await _reportService.GetExpenseCountByCategoryAsync();
            return Ok(data);
        }
    }
}

