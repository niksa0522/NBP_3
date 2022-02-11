namespace NBP_3.Models
{
    public class MongoAutoPart : MongoProduct
    {
        public List<string> Cars { get; set; }
        
        public MongoAutoPart() : base()
        {
            Cars = new List<string>();
        }
    }
}
