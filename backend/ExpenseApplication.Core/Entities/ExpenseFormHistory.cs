using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseApplication.Core.Entities
{
    public class ExpenseFormHistory
    {
        public int HistoryId { get; set; }
        public int FormId { get; set; }
        public ExpenseForm? Form { get; set; }
        public string Action { get; set; } = string.Empty;
        public int PerformedBy { get; set; }
        public User? PerformedByUser { get; set; }
        public string? Comments { get; set; }
        public DateTime ActionDate { get; set; } = DateTime.UtcNow;
    }
}
