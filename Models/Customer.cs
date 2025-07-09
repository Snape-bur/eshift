using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EShift.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string Address { get; set; }

        [Required(ErrorMessage = "Please select an associated user.")]
        [ForeignKey("AppUser")]
        public string AppUserId { get; set; }

        public virtual AppUser AppUser { get; set; }
        public List<Job> Jobs { get; set; } = new List<Job>();

    }
}
