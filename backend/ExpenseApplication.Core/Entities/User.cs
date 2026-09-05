using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseApplication.Core.Entities
{
    public class User : IdentityUser<int>
    {
        public string FullName { get; set; } = string.Empty;

        public int RoleId { get; set; }
        public Role? Role { get; set; }

        public int? ManagerId { get; set; }
        public User? Manager { get; set; }
        public ICollection<User> DirectReports { get; set; } = new List<User>();

        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
