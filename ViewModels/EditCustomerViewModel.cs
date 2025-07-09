using System.ComponentModel.DataAnnotations;
using System.Collections.Generic; // No longer needed if AppUsersList is removed
using Microsoft.AspNetCore.Mvc.Rendering; // No longer needed if AppUsersList is removed

namespace EShift.ViewModels
{
    public class EditCustomerViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Full Name cannot exceed 100 characters.")]
        public string FullName { get; set; }

        [Required]
        [Phone(ErrorMessage = "Invalid Phone Number format.")]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        public string Address { get; set; }

        // REMOVE these properties from the ViewModel as they won't be edited via the form:
        // public string AppUserId { get; set; }
        // public IEnumerable<SelectListItem> AppUsersList { get; set; }
    }
}