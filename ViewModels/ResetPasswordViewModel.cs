namespace EmployeeManagementPortal.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;

    public class ResetPasswordViewModel
    {
        [Required]
        public string Email { get; set; }
    }
}
