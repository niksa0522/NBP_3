
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace NBP_3.Models
{
    public class OrderList
    {
        public List<MongoOrder> Orders { get; set; }

        public string ID { get; set; }

        [DisplayName("Day Ordered")]
        public DateTime? date { get; set; }

        public int PageIndex { get; set; }
        public int TotalPages { get; set; }

        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (PageIndex < TotalPages);
            }
        }
    }
}
