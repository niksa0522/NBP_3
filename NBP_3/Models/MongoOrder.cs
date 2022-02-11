using MongoDB.Bson;
using MongoDB.Driver;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NBP_3.Models
{
    public class MongoOrder
    {
        public ObjectId Id { get; set; }

        public MongoDBRef user { get; set; }

        public string FullName { get; set; }
        
        public string Phone { get; set; }
        public float Price { get; set; }

        public Address Address { get; set; }

        [DisplayName("Order Date")]
        public DateTime orderDate { get; set; }
        public List<String> OrderStatus { get; set; }

        public List<MongoCartObject> produtcts { get; set; }
    }
}
