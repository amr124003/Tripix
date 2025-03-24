using System.ComponentModel.DataAnnotations;

namespace Tripix.View_Models
{
    public class OtpRequest
    {
        [Required(ErrorMessage = "Email Is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
