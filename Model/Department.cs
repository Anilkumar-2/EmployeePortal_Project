using System;
using System.Collections.Generic;

#nullable disable

namespace EmployeeManagementPortal.Model
{
    public partial class Department
    {
        public Department()
        {
            Employees = new HashSet<Employee>();
        }

        public int Id { get; set; }
        public string Dname { get; set; }

        public virtual ICollection<Employee> Employees { get; set; }
    }
}
