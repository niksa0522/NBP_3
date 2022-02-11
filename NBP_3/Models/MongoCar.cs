using MongoDB.Bson;
using MongoDB.Driver;

namespace NBP_3.Models
{
    public class MongoCar
    {
        public ObjectId Id { get; set; }

        public string Name { get; set; }
    }
}
