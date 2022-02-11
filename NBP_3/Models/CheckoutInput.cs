using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NBP_3.Models
{
    public class CheckoutInput
    {
        [DisplayName("Full Name")]
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public Address Address { get; set; } 
        public float Price { get; set; }
    }
}
