using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "User Name is required.")]

        public string UserName { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]

        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]

        public string Password { get; set; }
        [Required(ErrorMessage = "Confirm Password is required.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
        
        public IFormFile? ImagePorfile { get; set; }
    }
}
