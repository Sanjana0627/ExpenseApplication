using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseApplication.Core.Entities
{
    public class ErrorLog
    {
        public int LogId { get; set; }
        public string Source { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? StackTrace { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
