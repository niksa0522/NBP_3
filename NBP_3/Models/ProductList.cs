using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NBP_3.Models
{
    public class ProductList
    {
        [DisplayName("Search Text")]
        public string searchString { get; set; }

        [DisplayName("Max Price")]
        public float maxPrice { get; set; }

        [DisplayName("Tags")]
        public List<CheckboxList_model> tagList { get; set; }

        [DisplayName("Brands")]
        public List<CheckboxList_model> brandList { get; set; }

        public List<Product> Products { get; set; }

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
