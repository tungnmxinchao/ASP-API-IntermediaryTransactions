using System.ComponentModel.DataAnnotations;

namespace IntermediaryTransactionsApp.Dtos.UserDto
{
    public class UpdateUserRequest
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }
        public bool? IsActive { get; set; } 
        public int? RoleId { get; set; }
    }
}
