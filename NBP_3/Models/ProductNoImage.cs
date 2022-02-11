using Microsoft.AspNetCore.Mvc.Rendering;
using MongoDB.Bson;
using MongoDB.Driver;



namespace NBP_3.Models
{
    public class ProductNoImage
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        public string Description { get; set; }

        public string Brand { get; set; }

        public IFormFile? Image { get; set; }

        public List<CheckboxList_model> Tags { get; set; }

    }
}