using NBP_3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using NBP_3.MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;

namespace NBP_3.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class TagBrandCarController : Controller
    {

        private readonly IMongoDatabase database;

        public TagBrandCarController(IMongoDatabase database)
        {

            this.database = database;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Tags()
        {
            List<MongoTag> tags = MongoDBLogic.GetAllTags(database);


            return View(tags);
        }
        public IActionResult AddTag(string tag, string type)
        {
            List<string> types = new List<string>();

            if(type == "Both")
            {
                types.Add("Part");
                types.Add("Accessory");
            }
            else
            {
                types.Add(type);
            }
            MongoDBLogic.AddTag(tag, types, database);
            return RedirectToAction("Tags","TagBrandCar");
        }
        public async Task<IActionResult> DeleteTag(string Id)
        {
            MongoDBLogic.DeleteTag(Id,database);
            return RedirectToAction("Tags", "TagBrandCar");
        }
        public IActionResult Brands()
        {
            List<MongoBrand> tags = MongoDBLogic.GetAllBrands(database);


            return View(tags);
        }
        public IActionResult AddBrand(string tag, string type)
        {
            List<string> types = new List<string>();

            if (type == "Both")
            {
                types.Add("Part");
                types.Add("Accessory");
            }
            else
            {
                types.Add(type);
            }
            MongoDBLogic.AddBrand(tag, types, database);
            return RedirectToAction("Brands", "TagBrandCar");
        }
        public async Task<IActionResult> DeleteBrand(string Id)
        {
            MongoDBLogic.DeleteBrand(Id, database);
            return RedirectToAction("Brands", "TagBrandCar");
        }
        public IActionResult Cars()
        {
            List<MongoCar> tags = MongoDBLogic.GetAllCars(database);


            return View(tags);
        }
        public IActionResult AddCar(string tag)
        {
            MongoDBLogic.AddCar(tag, database);
            return RedirectToAction("Cars", "TagBrandCar");
        }
        public async Task<IActionResult> DeleteCar(string Id)
        {
            MongoDBLogic.DeleteCar(Id, database);
            return RedirectToAction("Cars", "TagBrandCar");
        }
    }
}
