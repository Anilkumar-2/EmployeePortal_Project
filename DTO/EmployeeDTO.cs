namespace EmployeeManagementPortal.DTO
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class EmployeeDTO
    {
        public string Ename { get; set; }
        public string Email { get; set; }
        public int? DepartmentId { get; set; }
        public int? RoleId { get; set; }
        public string Password { get; set; }
    }
}
