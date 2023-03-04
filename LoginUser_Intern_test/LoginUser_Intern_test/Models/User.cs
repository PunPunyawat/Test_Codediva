using System.ComponentModel.DataAnnotations;
namespace LoginUser_Intern_test.Models
{
    public class User
    {
        [Key]
        public int User_id { get; set; }
        public string Email { get; set; } = string.Empty;
        public byte[] PasswordHash {get; set; } = new byte[32];
        public byte[] PassSalt { get; set; } = new byte[32];
        public string verificationToken { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public string Birthday { get; set; } = string.Empty;
    }
}
