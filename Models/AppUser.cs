using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace EShift.Models
{
    public class AppUser : IdentityUser
    {
        [Required]
        public string FullName { get; set; }
    }

}
