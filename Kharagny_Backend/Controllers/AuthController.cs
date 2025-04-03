using Kharagny_Backend.DTOs;
using Kharagny_Backend.Entites;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Mail;
using System.Net;
using System.Text;
using Kharagny_Backend.Services;

namespace Kharagny_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly JWTService _jwtService;
        public AuthController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, JWTService jwtService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _jwtService = jwtService;
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login([FromBody] LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized("Invalid email or password.");

            if (!user.EmailConfirmed)
                return Unauthorized("Please confirm your email before logging in.");

            var result = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!result)
                return Unauthorized("Invalid email or password.");

            var roles = await _userManager.GetRolesAsync(user);
            var jwt = _jwtService.CreateJWT(user, roles);

            return Ok(new UserDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Jwt = jwt
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) != null)
                return BadRequest("User already exists with that email.");

            if (model.Password != model.ConfirmPassword)
                return BadRequest("Passwords do not match.");

            var user = new User
            {

                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                Area = model.Area,
                Gender = model.Gender,
                BirthDate = model.BirthDate,
                Email = model.Email,
                UserName = model.Email.Split('@')[0]
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return BadRequest("User registration failed.");

            if (!await _roleManager.RoleExistsAsync("User"))
                await _roleManager.CreateAsync(new IdentityRole("User"));

            await _userManager.AddToRoleAsync(user, "User");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var confirmationLink = $"{_configuration["AppUrl"]}/api/auth/confirmemail?token={encodedToken}&email={user.Email}"; // front aw Local 3ady

            await SendConfirmationEmail(user.Email, confirmationLink);

            return Ok("Registration successful. Please check your email for confirmation.");
        }

        [HttpGet("confirmemail")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return BadRequest("Invalid email address.");

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            return result.Succeeded ? Ok("Email confirmed successfully.") : BadRequest("Email confirmation failed.");
        }

        // Reset Password Gp Backend file
        private async Task SendConfirmationEmail(string email, string confirmationLink)
        {
            var smtpSettings = _configuration.GetSection("EmailSettings").Get<SmtpSettings>();
            var message = new MailMessage
            {
                From = new MailAddress(smtpSettings.FromEmail),
                Subject = "Please confirm your email",
                Body = $"<h1>Please confirm your email</h1><p>Click the following link to confirm your email: <a href='{confirmationLink}'>Confirm Email</a></p>",
                IsBodyHtml = true
            };
            message.To.Add(email);

            using (var smtpClient = new SmtpClient(smtpSettings.SmtpServer, smtpSettings.Port))
            {
                smtpClient.Credentials = new NetworkCredential(smtpSettings.Username, smtpSettings.Password);
                smtpClient.EnableSsl = true;  // Enable SSL for secure connection

                try
                {
                    await smtpClient.SendMailAsync(message);
                }
                catch (SmtpException smtpEx)
                {
                    // Log the error and send a more detailed response
                    Console.WriteLine(smtpEx.Message);
                    throw new Exception("Error sending confirmation email. Please check your SMTP settings.");
                }
            }
        }
    }
}
