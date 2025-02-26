namespace EmployeeManagementPortal.Controllers
{
    using EmployeeManagementPortal.DTO;
    using EmployeeManagementPortal.Model;
    using EmployeeManagementPortal.ViewModels;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Net;
    using System.Net.Mail;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;

    public class AccountController : Controller
    {
        private readonly EmployeeManagementContext _context;
        private readonly IConfiguration _configuration;
        private readonly SmtpSettingsModel _smtpSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountController(EmployeeManagementContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _configuration = configuration;
            _smtpSettings = configuration.GetSection("SmtpSettings").Get<SmtpSettingsModel>();
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register(EmployeeDTO employee)
        {
            return View();
        }
        [HttpPost]
        public JsonResult CreateUser([FromBody] EmployeeDTO employee)
        {
            if (string.IsNullOrEmpty(employee.Email) || string.IsNullOrEmpty(employee.Password))
            {
                return Json(new { success = false, message = "Email and Password are required." });
            }

            var e = new Employee
            {
                Email = employee.Email,
                DepartmentId = employee.DepartmentId,
                Ename = employee.Ename,
                Password = employee.Password,
                RoleId = employee.RoleId
            };

            SendEmail(employee.Email, employee.Ename);

            _context.Employees.Add(e);
            _context.SaveChanges();

            return Json(new { success = true, message = "Registration successful!" });
        }

        public IActionResult SignIn(LoginViewModel login)
        {
            var role = _context.Employees
                .Where(x => x.Email == login.Email && x.Password == login.Password)
                .Select(x => x.Role.Role1)
                .FirstOrDefault();

            if (role != null)
            {
                var token = GenerateToken(login.Email, role);

                //HttpContext.Response.Cookies.Append("AuthToken", token, new CookieOptions
                //{
                //    HttpOnly = true,  // Prevent client-side scripts from accessing the cookie
                //    Secure = true,    // Use secure cookies (HTTPS required)
                //    SameSite = SameSiteMode.Strict,
                //    Expires = DateTime.Now.AddHours(1)
                //});

                Response.Cookies.Delete("AuthToken");
                Response.Cookies.Append("AuthToken", token, new CookieOptions
                {
                    HttpOnly = true, // Ensures the cookie is accessible only to the server
                    Secure = true,   // Ensures the cookie is sent only over HTTPS
                    Expires = DateTimeOffset.UtcNow.AddHours(1) // Set expiration time as needed
                });

                //ViewData["userEmail"] = User.FindFirst(ClaimTypes.Email)?.Value;
                ViewData["userEmail"] = login.Email;


                return RedirectToAction("Index", "Home");
            }
            ViewBag.Error = "Invalid username or password.";
            return View("Login");
        }

        public string GenerateToken(string email, string role)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");

            var claims = new[]
            {
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpiryMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public void SendEmail(string Email, string Name)
        {

            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpSettings.SenderAddress, _smtpSettings.SenderDisplayName),
                Subject = "Test Email",
                Body = "This is a test email sent from a .NET application.",
                IsBodyHtml = _smtpSettings.IsBodyHtml
            };

            mailMessage.To.Add(Email);

            string filePath = @"C:\Users\gujjari.anil\Downloads\ImportantDoc.docx";
            Attachment attachment = new Attachment(filePath);
            mailMessage.Attachments.Add(attachment);

            NetworkCredential networkCredential = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);
            SmtpClient smtpClient = new SmtpClient
            {
                Host = _smtpSettings.Host,
                Port = _smtpSettings.Port,
                EnableSsl = _smtpSettings.EnableSsl,
                UseDefaultCredentials = _smtpSettings.UserDefaultCredentials,
                Credentials = networkCredential
            };
            smtpClient.SendMailAsync(mailMessage);
        }

        public IActionResult ForgotPasswordView()
        {
            return View();
        }
       
        [HttpPost]
        public async Task<JsonResult> SendResetLink(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Generate a reset token (you can use a GUID or any other unique identifier)
                var resetToken = Guid.NewGuid().ToString();

                // Save the reset token and email in the database (you'll need a table for this)

                SaveResetToken(model.Email, resetToken);
                // Example: SaveResetToken(model.Email, resetToken);

                // Create the reset link
                var request = _httpContextAccessor.HttpContext.Request;
                var resetLink = Url.Action("ResetPasswordForm", "Account", new { token = resetToken }, protocol: request.Scheme);

                //await SendResetEmail(model.Email, "Reset Password", $"Please reset your password by clicking <a href='{resetLink}'>here</a>");

                return Json(new { success = true, message = "Reset Link Sent!!" });
            }

            return Json(new { success = true, message = "Invalid email!!" });

        }

        private async Task SendResetEmail(string email, string subject, string message)
        {
            var smtpClient = new SmtpClient(_smtpSettings.Host)
            {
                Port = _smtpSettings.Port,
                Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpSettings.SenderAddress),
                Subject = subject,
                Body = message,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(email);

            await smtpClient.SendMailAsync(mailMessage);
        }

        public void SaveResetToken(string email, string token)
        {
            var resetToken = new PasswordResetToken
            {
                Email = email,
                Token = token,
                ExpirationDate = DateTime.Now.AddHours(1) // Set expiration time as needed
            };

            _context.PasswordResetTokens.Add(resetToken);
            _context.SaveChanges();
        }

        //[HttpGet]
        //public ActionResult ResetPassword(string token)
        //{
        //    // Verify the token (you'll need to implement this)
        //    var email = VerifyResetToken(token);

        //    if (email == null)
        //    {
        //        return View("Error");
        //    }

        //    var model = new ResetPasswordFormViewModel { Token = token };
        //    return View("ResetPasswordForm",model);
        //}


        [HttpGet]
        public ActionResult ResetPasswordForm(string token)
        {
            // Check if the token is valid and not expired
            var resetToken = _context.PasswordResetTokens.SingleOrDefault(t => t.Token == token && t.ExpirationDate > DateTime.Now);
            if (resetToken.Email != null)
            {
                // If valid, create a model with the token and show the reset password form
                var model = new ResetPasswordFormViewModel { Token = token };
                return View(model);
            }
            // If the token is invalid or expired, redirect to an error page
            return RedirectToAction("InvalidToken");
        }

        [HttpPost]
        public JsonResult ResetPassword([FromBody] ResetPasswordFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if the token is valid and not expired
                var resetToken = _context.PasswordResetTokens.SingleOrDefault(t => t.Token == model.Token && t.ExpirationDate > DateTime.Now);
                if (resetToken.Email != null)
                {
                    // Update the user's password and clear the reset token
                    UpdatePassword(resetToken.Email, model.NewPassword);
                    _context.PasswordResetTokens.Remove(resetToken);
                    _context.SaveChanges();
                    return Json(new { message = "Password reset successfully." });
                }
            }
            // If the token is invalid or expired, return an error message
            return Json(new { message = "Error resetting password." });
        }


        private void UpdatePassword(string Email, string newPassword)
        {
            if (Email != null)
            {
                var employee = _context.Employees.FirstOrDefault(u => u.Email == Email);
                if (employee != null)
                {
                    employee.Password = newPassword; // In a real application, hash the password before saving
                    _context.SaveChanges();
                }
                
            }
        }
    }
}
