using Microsoft.AspNetCore.Mvc;

namespace ExpenseApplication.DTOs
{
    // what the employee sends when creating or updating an expense form
    public class CreateExpenseFormRequest
    {
        public int CurrencyId { get; set; }
        public List<ExpenseLineItem> Expenses { get; set; } = new();
    }
    // one expense line inside the form (taxi,food,gas)
    public class ExpenseLineItem
    {
        public DateTime ExpenseDate { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Purpose { get; set; } = string.Empty;
        public string VendorName { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}

