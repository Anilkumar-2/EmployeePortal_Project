using System;
using System.Collections.Generic;

#nullable disable

namespace EmployeeManagementPortal.Model
{
    public partial class Log
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Level { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
        public string Properties { get; set; }
    }
}
