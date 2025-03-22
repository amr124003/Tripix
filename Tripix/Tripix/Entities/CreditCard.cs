#nullable disable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tripix.Entities
{
    public class CreditCard
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string CardNumber { get; set; }
        public string? Type { get; set; }
        public string? BankName { get; set; }
        public string? Schema { get; set; }
        [Required]
        public string CardHolderName { get; set; }
        [Required]
        [RegularExpression("^(0[1-9]|1[0-2])\\/([0-9]{2})$")]
        public string ExpiryDate { get; set; }
        [Required]
        [RegularExpression("^[0-9]{3,4}$")]
        public string CVV { get; set; }
    }
}
