using MongoDB.Bson;
using MongoDB.Driver;

namespace NBP_3.Models
{
    public class MongoUser
    {
        public ObjectId Id { get; set; }
        public string UserName { get; set; }

        public List<MongoCartObject> inCart { get; set; }

        public List<MongoDBRef> orders { get; set; }

        public MongoUser()
        {
            inCart = new List<MongoCartObject>();
            orders = new List<MongoDBRef>();
        }
    }
}
