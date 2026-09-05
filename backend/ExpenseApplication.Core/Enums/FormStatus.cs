using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseApplication.Core.Enums
{
    //submitted -> manager decides -> accountant pays
    public enum FormStatus
    {
        PendingApproval = 1,
        Rejected = 2,
        ManagerApproved = 3,
        Paid = 5
    }
}
