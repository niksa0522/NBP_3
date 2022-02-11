
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NBP_3.Models
{
    public class Address
    {
        [DisplayName("Town")]
        public string Town { get; set; }

        [DisplayName("Post Number")]
        public int postNum { get; set; }

        [DisplayName("Street Name")]
        public string Street { get; set; }

        [DisplayName("Street Number")]
        public int streetNum { get; set; }

        [DisplayName("Apartment Number")]
        public int? aptNum { get; set; }
    }
}
