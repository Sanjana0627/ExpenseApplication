using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseApplication.Core.Entities
{
    
    public class Expense
    {
        public int ExpenseId { get; set; }
        public int FormId { get; set; }
        public ExpenseForm? Form { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Purpose { get; set; } = string.Empty;
        public string VendorName { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }=DateTime.UtcNow;
    }
}
