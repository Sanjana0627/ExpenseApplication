using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpenseApplication.Core.Enums;
namespace ExpenseApplication.Core.Entities
{
    public class ExpenseForm
    {
        public int FormId { get; set; }
        public int EmployeeId { get; set; }
        public User? Employee { get; set; }
        public int ManagerId { get; set; }
        public User? Manager { get; set; }
        public int CurrencyId { get; set; } 
        public Currency? Currency { get; set; }
        public decimal TotalAmount { get; set; }
        public string? RejectionReason { get; set; }
        public DateTime SubmissionDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }= DateTime.UtcNow;
        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
        public ICollection<ExpenseFormHistory> History { get; set; } = new List<ExpenseFormHistory>();
        public FormStatus Status { get; set; }
    }
}
