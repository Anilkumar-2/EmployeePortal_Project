namespace EmployeeManagementPortal.DTO
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class SmtpSettingsModel
    {
        public int Port { get; set; }
        public string SenderDisplayName { get; set; }
        public string SenderAddress { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool EnableSsl { get; set; }
        public bool IsBodyHtml { get; set; }
        public string Host { get; set; }
        public bool UserDefaultCredentials { get; set; }
    }
}
