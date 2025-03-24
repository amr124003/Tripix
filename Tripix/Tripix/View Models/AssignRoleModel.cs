using System.ComponentModel.DataAnnotations;

namespace Tripix.View_Models
{
    public class AssignRoleModel
    {
        [Required(ErrorMessage = "Email Is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Role Is Required")]
        [AllowedValues("User", "Admin", "SuperAdmin", ErrorMessage = "This Role Can't Be Added")]
        public string Role { get; set; }
    }
}
