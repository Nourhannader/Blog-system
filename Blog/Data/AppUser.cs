using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Blog.Data
{
    public class AppUser:IdentityUser
    {
        [Required(ErrorMessage = "First Name is required")]
        [MaxLength(50, ErrorMessage = "First Name cannot exceed 50 characters")]
        [MinLength(3, ErrorMessage = "First Name must be at least 3 characters long")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "First Name can only contain letters")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last Name is required")]
        [MaxLength(50, ErrorMessage = "Last Name cannot exceed 50 characters")]
        [MinLength(3, ErrorMessage = "Last Name must be at least 3 characters long")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Last Name can only contain letters")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Display(Name = "Profile Image")]
        public string? ProfilePictureUrl { get; set; }
    }
}
