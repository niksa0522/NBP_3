using MongoDB.Bson;
using MongoDB.Driver;

namespace NBP_3.Models
{
    public class MongoBrand
    {
        public ObjectId Id { get; set; }

        public string Name { get; set; }
        public List<string> Type { get; set; }

        public MongoBrand()
        {
            Type = new List<string>();
        }
    }
}
