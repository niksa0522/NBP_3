using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace NBP_3.Models
{
    [BsonDiscriminator(RootClass =true)]
    [BsonKnownTypes(typeof(MongoAutoAccessory),typeof(MongoAutoPart))]
    public class MongoProduct
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        public string Description { get; set; }

        public string Brand { get; set; }
        public List<string> Tags { get; set; }

        public byte[] Image { get; set; }

        public MongoProduct()
        {
            Tags = new List<string>();
        }

    }
}
