using System.ComponentModel.DataAnnotations;

namespace EShift.Models
{
    public class JobRequestModel
    {
        [Required(ErrorMessage = "Full Name is required")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        [RegularExpression(@"[0-9]{10,15}", ErrorMessage = "Phone Number must be 10-15 digits")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Email Address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Pickup Address is required")]
        public string PickupAddress { get; set; }

        [Required(ErrorMessage = "Delivery Address is required")]
        public string DeliveryAddress { get; set; }

        [Required(ErrorMessage = "Move Date is required")]
        public DateTime MoveDate { get; set; }

        [Required(ErrorMessage = "Preferred Time is required")]
        public string MoveTime { get; set; }

        [Required(ErrorMessage = "Item Category is required")]
        public string Category { get; set; }

        [Required(ErrorMessage = "Number of Items is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Number of Items must be at least 1")]
        public int ItemCount { get; set; }
        public double EstimatedKg { get; set; }



        public string? Instructions { get; set; }

    }
}