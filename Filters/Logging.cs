//namespace EmployeeManagementPortal.Filters
//{
//    using Microsoft.AspNetCore.Hosting.Server;
//    using Microsoft.IdentityModel.Protocols;
//    using Serilog;
//    using System;
//    using System.Collections.Generic;
//    using System.Linq;
//    using System.Threading.Tasks;

//    public class Logging : IHttpApplication
//    {
//        protected void Application_Start()
//        {
//            Log.Logger = new LoggerConfiguration()
//                .WriteTo.MSSqlServer(
//                connectionString: ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString,
//                    sinkOptions: new MSSqlServerSinkOptions { TableName = "Logs", AutoCreateSqlTable = false }
//                )
//                .Enrich.WithProperty("Application", "YourMVCApp")
//                .MinimumLevel.Error()
//                .CreateLogger();

//        }
//    }
//}
