using System.ComponentModel.DataAnnotations;
using Blog.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Blog.Data
{
    public class AppUser:IdentityUser
    {
        [Display(Name = "Profile Image")]
        [ValidateNever]
        public string? ProfilePictureUrl { get; set; }
        [ValidateNever]
        public ICollection<Post> Posts { get; set; }
    }
}
