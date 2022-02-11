using NBP_3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using NBP_3.MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
namespace NBP_3.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {

        private readonly IMongoDatabase database;

        public ProductsController(IMongoDatabase database)
        {

            this.database = database;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AddAccessory()
        {
            List<MongoBrand> brands = MongoDBLogic.GetBrands("Accessory",database);
            List<MongoTag> tags = MongoDBLogic.GetTags("Accessory", database);
            List<CheckboxList_model> checkboxes = new List<CheckboxList_model>();
            int val = 1;
            foreach(var tag in tags)
            {
                checkboxes.Add(new CheckboxList_model() { Selected = false, Value = val, Text = tag.Name });
                val++;
            }
            AutoAccessory ac = new AutoAccessory();
            ac.Tags = checkboxes;
            ViewData["Brands"] = new SelectList(brands, "Name", "Name");
            return View(ac);
        }
        [HttpPost]
        public IActionResult AddAccessory( AutoAccessory autoAccessory)
        {

            

            MongoAutoAccessory ac = new MongoAutoAccessory();

            ac.Name=autoAccessory.Name;
            ac.Description=autoAccessory.Description;
            List<string> tags = new List<string>();
            foreach (CheckboxList_model s in autoAccessory.Tags)
            {
                if (s.Selected) { 
                tags.Add(s.Text);
                }
            }
            ac.Tags = tags;
            ac.Brand=autoAccessory.Brand;
            ac.Price=autoAccessory.Price;

            if (autoAccessory.Image != null)
            {
                MemoryStream ms = new MemoryStream();
                autoAccessory.Image.CopyTo(ms);
                ac.Image = ms.ToArray();
                ms.Close();
                ms.Dispose();
            }

            MongoDBLogic.AddAutoAccessory(ac, database);

            return RedirectToAction("Index", "Products");
        }
        public IActionResult AddPart()
        {
            List<MongoBrand> brands = MongoDBLogic.GetBrands("Part",database);
            List<MongoTag> tags = MongoDBLogic.GetTags("Part", database);
            List<MongoCar> cars = MongoDBLogic.GetAllCars(database);
            List<CheckboxList_model> checkboxes = new List<CheckboxList_model>();
            int val = 1;
            foreach (var tag in tags)
            {
                checkboxes.Add(new CheckboxList_model() { Selected = false, Value = val, Text = tag.Name });
                val++;
            }
            List<CheckboxList_model> checkboxesCars = new List<CheckboxList_model>();
            val = 1;
            foreach (var tag in cars)
            {
                checkboxesCars.Add(new CheckboxList_model() { Selected = false, Value = val, Text = tag.Name });
                val++;
            }
            AutoPart ap = new AutoPart();
            ap.Tags = checkboxes;
            ap.Cars = checkboxesCars;
            ViewData["Brands"] = new SelectList(brands, "Name", "Name");

            return View(ap);
        }
        [HttpPost]
        public IActionResult AddPart(AutoPart autoPart)
        {
            MongoAutoPart ac = new MongoAutoPart();

            ac.Name = autoPart.Name;
            ac.Description = autoPart.Description;
            List<string> tags = new List<string>();
            foreach (CheckboxList_model s in autoPart.Tags)
            {
                if (s.Selected)
                {
                    tags.Add(s.Text);
                }
            }
            List<string> cars = new List<string>();
            foreach (CheckboxList_model s in autoPart.Cars)
            {
                if (s.Selected)
                {
                    cars.Add(s.Text);
                }
            }
            ac.Tags = tags;
            ac.Brand = autoPart.Brand;
            ac.Price = autoPart.Price;
            ac.Cars = cars;

            if (autoPart.Image != null)
            {
                MemoryStream ms = new MemoryStream();
                autoPart.Image.CopyTo(ms);
                ac.Image = ms.ToArray();
                ms.Close();
                ms.Dispose();
            }

            MongoDBLogic.AddAutoPart(ac, database);

            return RedirectToAction("Index", "Products");
        }


        public IActionResult Delete(string id)
        {
            MongoDBLogic.RemoveProduct(id, database);
            return RedirectToAction("Index", "Products");
        }
        public IActionResult Edit(string id)
        {
            MongoProduct mp = MongoDBLogic.GetProduct(id, database);
            if(mp.GetType() == typeof(MongoAutoAccessory))
            {
                return RedirectToAction("EditAccessory", "Products", new { id });
            }
            else
            {
                return RedirectToAction("EditPart", "Products", new { id });
            }
        }
        public IActionResult EditAccessory(string id)
        {
            MongoAutoAccessory mp = MongoDBLogic.GetAutoAccessory(id, database);

            List<MongoBrand> brands = MongoDBLogic.GetBrands("Accessory", database);
            List<MongoTag> tags = MongoDBLogic.GetTags("Accessory", database);
            List<CheckboxList_model> checkboxes = new List<CheckboxList_model>();
            int val = 1;
            foreach (var tag in tags)
            {
                bool selected = false;
                if (mp.Tags.Contains(tag.Name))
                    selected = true;
                checkboxes.Add(new CheckboxList_model() { Selected = selected, Value = val, Text = tag.Name });
                val++;
            }
            AutoAccessoryNoImage ac = new AutoAccessoryNoImage();

            ac.Name = mp.Name;
            ac.Price = mp.Price;
            ac.Description = mp.Description;
            ac.Brand = mp.Brand;
            ac.Tags = checkboxes;
            ac.Id = new ObjectId(id);
            ViewData["Brands"] = new SelectList(brands, "Name", "Name");
            return View(ac);


        }
        public IActionResult EditPart(string id)
        {
            MongoAutoPart mp = MongoDBLogic.GetAutoPart(id, database);


            List<MongoBrand> brands = MongoDBLogic.GetBrands("Part", database);
            List<MongoTag> tags = MongoDBLogic.GetTags("Part", database);
            List<MongoCar> cars = MongoDBLogic.GetAllCars(database);
            List<CheckboxList_model> checkboxes = new List<CheckboxList_model>();
            int val = 1;
            foreach (var tag in tags)
            {
                bool selected = false;
                if (mp.Tags.Contains(tag.Name))
                    selected = true;
                checkboxes.Add(new CheckboxList_model() { Selected = selected, Value = val, Text = tag.Name });
                val++;
            }
            List<CheckboxList_model> checkboxesCars = new List<CheckboxList_model>();
            val = 1;
            foreach (var tag in cars)
            {
                bool selected = false;
                if (mp.Cars.Contains(tag.Name))
                    selected = true;
                checkboxesCars.Add(new CheckboxList_model() { Selected = selected, Value = val, Text = tag.Name });
                val++;
            }
            AutoPartNoImage ap = new AutoPartNoImage();
            ap.Name = mp.Name;
            ap.Price = mp.Price;
            ap.Description = mp.Description;
            ap.Brand = mp.Brand;
            ap.Tags = checkboxes;
            ap.Cars = checkboxesCars;
            ap.Id = new ObjectId(id);
            ViewData["Brands"] = new SelectList(brands, "Name", "Name");

            return View(ap);
        }
        [HttpPost]
        public IActionResult EditAccessory(AutoAccessoryNoImage autoAccessory, string Id)
        {
            MongoAutoAccessory ac = MongoDBLogic.GetAutoAccessory(Id, database);

             ac.Name = autoAccessory.Name;
            ac.Description = autoAccessory.Description;
            List<string> tags = new List<string>();
            foreach (CheckboxList_model s in autoAccessory.Tags)
            {
                if (s.Selected)
                {
                    tags.Add(s.Text);
                }
            }
            ac.Tags = tags;
            ac.Brand = autoAccessory.Brand;
            ac.Price = autoAccessory.Price;


            if (autoAccessory.Image != null)
            {
                MemoryStream ms = new MemoryStream();
                autoAccessory.Image.CopyTo(ms);
                ac.Image = ms.ToArray();
                ms.Close();
                ms.Dispose();
            }

            MongoDBLogic.EditAccessory(ac, database);

            return RedirectToAction("Index", "Products");
        }

        [HttpPost]
        public IActionResult EditPart(AutoPartNoImage autoPart, string Id)
        {

            MongoAutoPart ac = MongoDBLogic.GetAutoPart(Id, database);

            ac.Name = autoPart.Name;
            ac.Description = autoPart.Description;
            List<string> tags = new List<string>();
            foreach (CheckboxList_model s in autoPart.Tags)
            {
                if (s.Selected)
                {
                    tags.Add(s.Text);
                }
            }
            List<string> cars = new List<string>();
            foreach (CheckboxList_model s in autoPart.Cars)
            {
                if (s.Selected)
                {
                    cars.Add(s.Text);
                }
            }
            ac.Tags = tags;
            ac.Brand = autoPart.Brand;
            ac.Price = autoPart.Price;
            ac.Cars = cars;

            if (autoPart.Image != null)
            {
                MemoryStream ms = new MemoryStream();
                autoPart.Image.CopyTo(ms);
                ac.Image = ms.ToArray();
                ms.Close();
                ms.Dispose();
            }

            MongoDBLogic.EditPart(ac, database);

            return RedirectToAction("Index", "Products");
        }
    }
}
