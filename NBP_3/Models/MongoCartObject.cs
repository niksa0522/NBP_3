using MongoDB.Bson;
using MongoDB.Driver;

namespace NBP_3.Models
{
    public class MongoCartObject
    {
        public string Name { get;set; }
        public float Price { get;set; }

        public int Amount { get;set; }

        public MongoDBRef Product { get; set; }
    }
}
