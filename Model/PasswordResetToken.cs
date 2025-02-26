using System;
using System.Collections.Generic;

#nullable disable

namespace EmployeeManagementPortal.Model
{
    public partial class PasswordResetToken
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
