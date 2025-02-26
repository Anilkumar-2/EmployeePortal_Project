namespace EmployeeManagementPortal.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class EmployeeViewModel
    {
        public int Id { get; set; }
        public string EmployeeName { get; set; }
        public string Email { get; set; }
        public string Department { get; set; }
    }
}
