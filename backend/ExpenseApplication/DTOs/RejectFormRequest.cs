namespace ExpenseApplication.DTOs
{
    // reason the manager types in when rejecting a form
    public class RejectFormRequest
    {
        public string Reason { get; set; } = string.Empty;
    }
}
