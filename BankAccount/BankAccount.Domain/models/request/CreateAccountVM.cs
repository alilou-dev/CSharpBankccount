using System.ComponentModel.DataAnnotations;

namespace BankAccount.Domain.models.request
{
    public class CreateAccountVM
    {
        [Required]
        [MinLength(5)]
        public string? Label { get; set; }
    }
}
