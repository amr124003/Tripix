using System.ComponentModel.DataAnnotations;

namespace Tripix.View_Models
{
    public class CreditDTO
    {
        [Required]
        [MaxLength(19)]
        [MinLength(13)]
        public string CardNumber { get; set; }  // رقم البطاقة
        public string CardHolderName { get; set; }  // اسم صاحب البطاقة
        public string ExpiryMonth { get; set; }  // شهر الانتهاء
        public string ExpiryYear { get; set; }  // سنة الانتهاء
        public string CVV { get; set; }  // كود الأمان
    }
}
