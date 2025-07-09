using System.ComponentModel.DataAnnotations;


namespace EShift.ViewModels
{
    public class EditAdminViewModel
    {
        public string AppUserId { get; set; }
        public string FullName { get; set; }
        public string Department { get; set; }
        public string Email { get; set; }
    }
}
