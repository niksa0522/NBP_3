using MongoDB.Bson;
using MongoDB.Driver;

namespace NBP_3.Models
{
    public class MongoTag
    {
        public ObjectId Id { get; set; }

        public string Name { get; set; }

        public List<string> Type { get; set; }

        public MongoTag()
        {
            Type = new List<string>();
        }
    }
}
