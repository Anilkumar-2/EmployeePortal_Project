using System;
using System.Collections.Generic;

#nullable disable

namespace EmployeeManagementPortal.Model
{
    public partial class Employee
    {
        public int Id { get; set; }
        public string Ename { get; set; }
        public string Email { get; set; }
        public int? DepartmentId { get; set; }
        public int? RoleId { get; set; }
        public string Password { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Department Department { get; set; }
        public virtual Role Role { get; set; }
    }
}
