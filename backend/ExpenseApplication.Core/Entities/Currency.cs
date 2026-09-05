using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseApplication.Core.Entities
{
    public class Currency
    {
        public int CurrencyId { get; set; }
        public string Code { get; set; } = string.Empty;
    }
}
