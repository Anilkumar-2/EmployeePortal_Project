namespace EmployeeManagementPortal.Controllers
{
    using EmployeeManagementPortal.ViewModels;
    using EmployeeManagementPortal.Model;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using EmployeeManagementPortal.DTO;
    using Microsoft.AspNetCore.Authorization;

    public class EmployeeController : Controller
    {
        private readonly EmployeeManagementContext _context;

        public EmployeeController(EmployeeManagementContext context)
        {
            _context = context;
        }

        public IActionResult EmployeeList()
        {
            var x = _context.Employees
                .Include(e => e.Department)
                .Where(x => x.IsDeleted == false)
                .Select(e => new EmployeeViewModel
                {
                    Id = e.Id,
                    EmployeeName = e.Ename,
                    Email = e.Email,
                    Department = e.Department.Dname
                }).ToList();
            return View(x);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult CreateEmployee()
        {
            return View();
        }

        public IActionResult AddEmployee(EmployeeDTO employee)
        {
            var x = new Employee
            {
                Email = employee.Email,
                Ename = employee.Ename,
                DepartmentId = employee.DepartmentId,
                RoleId = employee.RoleId,
                Password = employee.Password
            };

            _context.Employees.Add(x);
            _context.SaveChanges();
            return View();
        }

        public IActionResult GetEmployeeById(int Id)
        {
            var x = _context.Employees
                .Include(e => e.Department)
                .Where(e => e.Id == Id).First();
            return View("EmployeeData", x);
        }

        [HttpPost]
        public IActionResult DeleteById(int Id)
        {
            var employee = _context.Employees.First(x => x.Id == Id);
            if (employee == null)
            {
                return NotFound();
            }

            employee.IsDeleted = true;
            _context.Employees.Update(employee);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
