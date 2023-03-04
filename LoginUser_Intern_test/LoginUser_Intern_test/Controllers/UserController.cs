using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace LoginUser_Intern_test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataContext _context;

        public UserController(DataContext context)
        {
            _context = context;
        }

        // method register user
        [HttpPost("register")]
        public async Task<ActionResult> UserRegister(Request_UserRegister request)
        {
            if (_context.Users.Any(i => i.Email == request.Email))
            {
                return BadRequest("User already exists");
            }

            CreatePassHash(request.Password, out byte[] pass_hash, out byte[] pass_salt);

            var user = new User
            {
                Name = request.Name,
                Surname = request.Surname,
                Email = request.Email,
                PasswordHash = pass_hash,
                PassSalt = pass_salt,
                Birthday = request.Birthday,
                Gender = request.Gender,
                Mobile = request.Mobile,
                verificationToken = CreateRandomToken()
            };

            _context.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User created successfully.");
        }

        // method login user
        [HttpGet("Login")]
        public async Task<ActionResult> UserLogin(string Email = "default@gmail.com",string Password = "defaultPassword")
        {
            var user = await _context.Users.FirstOrDefaultAsync(i => i.Email == Email);
            if (user == null) return BadRequest("User not found");

            if (!VerifypassHash(Password, user.PasswordHash, user.PassSalt))
            {
                return BadRequest("Incorrect Password");
            }
            return Ok("User id reference : " + user.User_id);
        }

        // method verify email to reset password
        [HttpGet("VerifyEmail_resetPass")]
        public async Task<ActionResult> User_verify_Email(string Email = "default@gmail.com", string Birthday = "00/00/00")
        {
            var email_check = await _context.Users.FirstOrDefaultAsync(i => i.Email == Email);
            var bd_check = await _context.Users.FirstOrDefaultAsync(i => i.Birthday == Birthday);
            if (email_check == null || bd_check == null) return BadRequest("unreachable , try again !");
            return Ok(email_check.verificationToken);
        }

        // method resetPassword
        [HttpPut("ResetPassword")]
        public async Task<ActionResult> UserResetPassword(string TokenVerify = "defaultToken", string Password_reset = "defaultResetPass")
        {
            var verify_User = await _context.Users.FirstOrDefaultAsync(i => i.verificationToken == TokenVerify);
            if (verify_User == null) return BadRequest("Invalid Token");

            CreatePassHash(Password_reset, out byte[] pass_hash, out byte[] pass_salt);
            verify_User.PassSalt = pass_salt;
            verify_User.PasswordHash = pass_hash;
            verify_User.verificationToken = CreateRandomToken();

            await _context.SaveChangesAsync();
            return Ok("Reset password successfully");
        }

        // method passHash
        private static void CreatePassHash(string password, out byte[] pass_hash, out byte[] pass_salt)
        {
            using (var hmac = new HMACSHA512())
            {
                pass_salt = hmac.Key;
                pass_hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
        private static string CreateRandomToken()  
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        }

        // confirm password
        private static bool VerifypassHash(string password, byte[] pass_hash, byte[] pass_salt)
        {
            using (var hmac = new HMACSHA512(pass_salt))
            {
                var compute_hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return compute_hash.SequenceEqual(pass_hash);   
            }
        }

    }


    //Models request
    public class Request_UserRegister
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required, MinLength(5, ErrorMessage = "Password must be at least 5 characters")]
        public string Password { get; set; } = string.Empty;
        [Required, Phone]
        public string Mobile { get; set; } = string.Empty;
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Surname { get; set; } = string.Empty;
        [Required]
        public string Birthday { get; set; } =string.Empty;
        [Required]
        public string Gender { get; set; } = string.Empty;
    }
}
