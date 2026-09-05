using ExpenseApplication.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseApplication.Services.Reports
{
    // Holds all the "Admin - Reports" queries so ExpenseFormService 
    public class AdminReportService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdminReportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // counts how many forms are in each status
        public async Task<IEnumerable<object>> GetStatusBreakdownAsync()
        {
            var forms = await _unitOfWork.ExpenseForms.GetAllAsync(null, null, null);
            return forms
                .GroupBy(f => f.Status)
                .Select(g => new
                {
                    Status = g.Key.ToString(),
                    Count = g.Count(),
                }).ToList();
        }

        // counts how many forms were submitted in each month
        public async Task<IEnumerable<object>> GetMonthlyFormCountAsync()
        {
            var forms = await _unitOfWork.ExpenseForms.GetAllAsync(null, null, null);

            return forms
                .GroupBy(f => new { f.SubmissionDate.Year, f.SubmissionDate.Month })
                .Select(g => new
                {
                    Month = $"{g.Key.Year}-{g.Key.Month:D2}",
                    FormCount = g.Count()
                })
                .OrderBy(x => x.Month)
                .ToList();
        }

        // totals how much each employee has claimed, highest spender first
        public async Task<IEnumerable<object>> GetSpendByEmployeeAsync()
        {
            var forms = await _unitOfWork.ExpenseForms.GetAllAsync(null, null, null);
            return forms
                .GroupBy(f => f.Employee != null ? f.Employee.FullName : "Unknown")
                .Select(g => new
                {
                    EmployeeName = g.Key,
                    TotalAmount = g.Sum(f => f.TotalAmount)
                })
                .OrderByDescending(x => x.TotalAmount)
                .ToList();
        }

        // counts how many individual expenses fall under each category
        public async Task<IEnumerable<object>> GetExpenseCountByCategoryAsync()
        {
            var forms = await _unitOfWork.ExpenseForms.GetAllAsync(null, null, null);

            return forms
                .SelectMany(f => f.Expenses)
                .GroupBy(e => e.Category)
                .Select(g => new
                {
                    Category = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .ToList();
        }

        // works out each manager's approve/reject counts and rejection rate, based on their history entries
        public async Task<IEnumerable<object>> GetRejectionRateByManagerAsync()
        {
            var forms = await _unitOfWork.ExpenseForms.GetAllAsync(null, null, null);

            var decisionActions = forms
                .SelectMany(f => f.History)
                .Where(h => h.Action == "Approved" || h.Action == "Rejected")
                .GroupBy(h => h.PerformedBy)
                .Select(g => new
                {
                    ManagerId = g.Key,
                    Approved = g.Count(h => h.Action == "Approved"),
                    Rejected = g.Count(h => h.Action == "Rejected")
                })
                .ToList();

            var result = new List<object>();
            foreach (var g in decisionActions)
            {
                var manager = await _unitOfWork.Users.GetUserByIdAsync(g.ManagerId);
                var total = g.Approved + g.Rejected;
                var rate = total == 0 ? 0 : Math.Round((double)g.Rejected / total * 100, 1);

                result.Add(new
                {
                    ManagerName = manager?.FullName ?? "Unknown",
                    ApprovedCount = g.Approved,
                    RejectedCount = g.Rejected,
                    RejectionRatePercent = rate
                });
            }

            return result;
        }

        // averages how many hours it takes a form to get a decision (approve or reject) after submission
        public async Task<object> GetAverageTurnaroundAsync()
        {
            var forms = await _unitOfWork.ExpenseForms.GetAllAsync(null, null, null);

            var turnaroundHours = new List<double>();
            foreach (var form in forms)
            {
                var decision = form.History
                    .Where(h => h.Action == "Approved" || h.Action == "Rejected")
                    .OrderBy(h => h.ActionDate)
                    .FirstOrDefault();

                if (decision != null)
                {
                    var hours = (decision.ActionDate - form.SubmissionDate).TotalHours;
                    if (hours >= 0)
                        turnaroundHours.Add(hours);
                }
            }

            var average = turnaroundHours.Count > 0 ? Math.Round(turnaroundHours.Average(), 1) : 0;

            return new { AverageTurnaroundHours = average, SampleSize = turnaroundHours.Count };
        }
    }
}

