using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NBP_3.Models
{
    public class AutoPartList : ProductList
    {
        public List<AutoPart> AutoParts { get; set; }

        [DisplayName("Cars")]
        public List<CheckboxList_model> carsList { get; set; }
    }
}
