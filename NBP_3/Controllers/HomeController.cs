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
    
    public class HomeController : Controller
    {

        private readonly IMongoDatabase database;

        public HomeController(IMongoDatabase database)
        {

            this.database = database;
        }

        public IActionResult Index(string name, string type, int? pageNumber)
        {
            if (String.IsNullOrEmpty(name) && type==null)
            {
                if(pageNumber == null)
                {
                    pageNumber = 1;
                }
                ProductList productList = new ProductList();
                List<Product> products = new List<Product>();
                //AutoAccessory ac=new AutoAccessory();
                List<MongoProduct> mongoProducts = MongoDBLogic.GetProductsByPage((int)pageNumber,database);
                //mac = MongoDBLogic.GetAutoAccessoresWithPicture(database);

                /*ac.Description = mac.Description;
                ac.Name =  mac.Name;
                ac.Brand = mac.Brand;
                ac.Price = mac.Price;
                */
                int i = 0;
                ViewBag.ImageDataUrl = new List<String>();
                foreach (MongoProduct product in mongoProducts)
                {
                    products.Add(new Product() { Id = product.Id, Name = product.Name, Price = product.Price });
                    if (product.Image != null)
                    {
                        string imageBase64Data = Convert.ToBase64String(product.Image);
                        string imageDataUrl = string.Format("data:image/jpg;base64,{0}", imageBase64Data);
                        ViewBag.ImageDataUrl.Add(imageDataUrl);
                    }
                    else
                    {
                        ViewBag.ImageDataUrl.Add(new string(""));
                    }
                    i++;
                }
                ViewBag.size = i;
                productList.Products = products;
                int pageSize = 8;
                long documents = MongoDBLogic.GetProductNum(database);

                productList.PageIndex = (int)pageNumber;
                productList.TotalPages = (int)Math.Ceiling((int)documents / (double)pageSize);

                return View(productList);
            }
            else
            {
                if (type == "Part")
                {
                    return RedirectToAction("SearchPart", new { name });
                }
                else
                {
                    //AutoAccessoryList al = new AutoAccessoryList() { searchString=name };
                    return RedirectToAction("SearchAccessory", new { name }); 
                }
            }
        }
        public IActionResult SearchPart(string name)
        {
            List<MongoBrand> brands = MongoDBLogic.GetBrands("Part", database);
            List<MongoTag> tags = MongoDBLogic.GetTags("Part", database);
            List<MongoCar> cars = MongoDBLogic.GetAllCars(database);
            List<CheckboxList_model> checkboxesTags = new List<CheckboxList_model>();
            int val = 1;
            foreach (var tag in tags)
            {
                checkboxesTags.Add(new CheckboxList_model() { Selected = false, Value = val, Text = tag.Name });
                val++;
            }
            val = 1;
            List<CheckboxList_model> checkboxesBrands = new List<CheckboxList_model>();
            foreach (var tag in brands)
            {
                checkboxesBrands.Add(new CheckboxList_model() { Selected = false, Value = val, Text = tag.Name });
                val++;
            }
            val = 1;
            List<CheckboxList_model> checkboxesCars = new List<CheckboxList_model>();
            foreach (var tag in cars)
            {
                checkboxesCars.Add(new CheckboxList_model() { Selected = false, Value = val, Text = tag.Name });
                val++;
            }
            AutoPartList al = new AutoPartList();
            al.tagList = checkboxesTags;
            al.brandList = checkboxesBrands;
            al.carsList = checkboxesCars;

            List<AutoPart> products = new List<AutoPart>();
            List<MongoAutoPart> mongoProducts;
            long documents;
            if (String.IsNullOrEmpty(name))
            {
                
                //AutoAccessory ac=new AutoAccessory();
                mongoProducts = MongoDBLogic.GetAutoParts(1,database);
                //mac = MongoDBLogic.GetAutoAccessoresWithPicture(database);
                documents = MongoDBLogic.GetPartNum(database);
                /*ac.Description = mac.Description;
                ac.Name =  mac.Name;
                ac.Brand = mac.Brand;
                ac.Price = mac.Price;
                */

            }
            else
            {
                al.searchString = name;
                mongoProducts = MongoDBLogic.GetAutoParts(1,al,database);
                documents = MongoDBLogic.GetPartNum(al,database);
            }
            int i = 0;
            ViewBag.ImageDataUrl = new List<String>();
            foreach (MongoAutoPart product in mongoProducts)
            {
                products.Add(new AutoPart() { Id=product.Id, Name = product.Name, Price = product.Price });
                if (product.Image != null)
                {
                    string imageBase64Data = Convert.ToBase64String(product.Image);
                    string imageDataUrl = string.Format("data:image/jpg;base64,{0}", imageBase64Data);
                    ViewBag.ImageDataUrl.Add(imageDataUrl);
                }
                else
                {
                    ViewBag.ImageDataUrl.Add(new string(""));
                }
                i++;
            }
            int pageSize = 8;

            al.PageIndex = 1;
            al.TotalPages = (int)Math.Ceiling((int)documents / (double)pageSize);

            ViewBag.size = i;
            al.AutoParts = products;
            return View(al);
        }

        [HttpPost]
        public IActionResult SearchPart(AutoPartList al)
        {
            int pageIndex;
            if (!String.IsNullOrEmpty(Request.Form["Next"]))
            {
                pageIndex = al.PageIndex+1;
            }
            else if(!String.IsNullOrEmpty(Request.Form["Previous"]))
            {
                pageIndex = al.PageIndex - 1;
            }
            else
            {
                pageIndex = 1;
            }

                List<AutoPart> products = new List<AutoPart>();
                List<MongoAutoPart> mongoProducts = MongoDBLogic.GetAutoParts(pageIndex,al, database);
                int i = 0;
                ViewBag.ImageDataUrl = new List<String>();
                foreach (MongoAutoPart product in mongoProducts)
                {
                    products.Add(new AutoPart() { Id = product.Id, Name = product.Name, Price = product.Price });
                    if (product.Image != null)
                    {
                        string imageBase64Data = Convert.ToBase64String(product.Image);
                        string imageDataUrl = string.Format("data:image/jpg;base64,{0}", imageBase64Data);
                        ViewBag.ImageDataUrl.Add(imageDataUrl);
                    }
                    else
                    {
                        ViewBag.ImageDataUrl.Add(new string(""));
                    }
                    i++;
                }
                ViewBag.size = i;
                al.AutoParts = products;

                int pageSize = 8;
                long documents = MongoDBLogic.GetPartNum(al,database);

                al.PageIndex = pageIndex;
                al.TotalPages = (int)Math.Ceiling((int)documents / (double)pageSize);

                return View(al);
        }

        public IActionResult SearchAccessory(string name)
        {

            

            List<MongoBrand> brands = MongoDBLogic.GetBrands("Accessory", database);
            List<MongoTag> tags = MongoDBLogic.GetTags("Accessory", database);
            List<CheckboxList_model> checkboxesTags = new List<CheckboxList_model>();
            int val = 1;
            foreach (var tag in tags)
            {
                checkboxesTags.Add(new CheckboxList_model() { Selected = false, Value = val, Text = tag.Name });
                val++;
            }
            val = 1;
            List<CheckboxList_model> checkboxesBrands = new List<CheckboxList_model>();
            foreach (var tag in brands)
            {
                checkboxesBrands.Add(new CheckboxList_model() { Selected = false, Value = val, Text = tag.Name });
                val++;
            }

            AutoAccessoryList al = new AutoAccessoryList();
            al.tagList = checkboxesTags;
            al.brandList = checkboxesBrands;

            List<AutoAccessory> products = new List<AutoAccessory>();
            List<MongoAutoAccessory> mongoProducts;
            long documents;
            if (String.IsNullOrEmpty(name))
            {
                
                //AutoAccessory ac=new AutoAccessory();
                mongoProducts = MongoDBLogic.GetAutoAccessores(1,database);
                //mac = MongoDBLogic.GetAutoAccessoresWithPicture(database);
                documents = MongoDBLogic.GetAccessoryNum(database);
                /*ac.Description = mac.Description;
                ac.Name =  mac.Name;
                ac.Brand = mac.Brand;
                ac.Price = mac.Price;
                */
            }
            else
            {
                al.searchString = name;
                mongoProducts = MongoDBLogic.GetAutoAccessories(1,al,database);
                documents = MongoDBLogic.GetAccessoryNum(al,database);
            }
            int i = 0;
            ViewBag.ImageDataUrl = new List<String>();
            foreach (MongoAutoAccessory product in mongoProducts)
            {
                products.Add(new AutoAccessory() { Id = product.Id, Name = product.Name, Price = product.Price });
                if (product.Image != null)
                {
                    string imageBase64Data = Convert.ToBase64String(product.Image);
                    string imageDataUrl = string.Format("data:image/jpg;base64,{0}", imageBase64Data);
                    ViewBag.ImageDataUrl.Add(imageDataUrl);
                }
                else
                {
                    ViewBag.ImageDataUrl.Add(new string(""));
                }
                i++;
            }
            ViewBag.size = i;
            al.AutoAccessories = products;

            int pageSize = 8;

            al.PageIndex = 1;
            al.TotalPages = (int)Math.Ceiling((int)documents / (double)pageSize);

            return View(al);
        }

        [HttpPost]
        public IActionResult SearchAccessory(AutoAccessoryList al)
        {

            int pageIndex;
            if (!String.IsNullOrEmpty(Request.Form["Next"]))
            {
                pageIndex = al.PageIndex + 1;
            }
            else if (!String.IsNullOrEmpty(Request.Form["Previous"]))
            {
                pageIndex = al.PageIndex - 1;
            }
            else
            {
                pageIndex = 1;
            }


            List<AutoAccessory> products = new List<AutoAccessory>();
            List<MongoAutoAccessory> mongoProducts = MongoDBLogic.GetAutoAccessories(pageIndex,al,database);
            int i = 0;
            ViewBag.ImageDataUrl = new List<String>();
            foreach (MongoAutoAccessory product in mongoProducts)
            {
                products.Add(new AutoAccessory() { Id = product.Id, Name = product.Name, Price = product.Price });
                if (product.Image != null)
                {
                    string imageBase64Data = Convert.ToBase64String(product.Image);
                    string imageDataUrl = string.Format("data:image/jpg;base64,{0}", imageBase64Data);
                    ViewBag.ImageDataUrl.Add(imageDataUrl);
                }
                else
                {
                    ViewBag.ImageDataUrl.Add(new string(""));
                }
                i++;
            }
            ViewBag.size = i;
            al.AutoAccessories = products;

            int pageSize = 8;
            long documents = MongoDBLogic.GetAccessoryNum(al, database);

            al.PageIndex = pageIndex;
            al.TotalPages = (int)Math.Ceiling((int)documents / (double)pageSize);

            return View(al);
        }



        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult ViewProduct(string id)
        {
            MongoProduct mp = MongoDBLogic.GetProduct(id, database);
            if (mp.GetType() == typeof(MongoAutoAccessory))
            {
                return RedirectToAction("ViewAccessory", "Home", new { id });
            }
            else if(mp.GetType() == typeof(MongoAutoPart))
            {
                return RedirectToAction("ViewPart", "Home", new { id });
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public IActionResult ViewAccessory(string id, string? message)
        {
            MongoAutoAccessory mac = MongoDBLogic.GetAutoAccessory(id, database);
            if (mac.Image != null)
            {
                    string imageBase64Data = Convert.ToBase64String(mac.Image);
                    string imageDataUrl = string.Format("data:image/jpg;base64,{0}", imageBase64Data);
                    ViewBag.ImageDataUrl=imageDataUrl;
            }
            else
            {
                    ViewBag.ImageDataUrl=(new string(""));
            }
            if (!String.IsNullOrEmpty(message))
            {
                ViewBag.Message = message;
            }
            return View(mac);
        }
        public IActionResult ViewPart(string id, string? message)
        {
            MongoAutoPart mac = MongoDBLogic.GetAutoPart(id, database);
            ViewBag.ImageDataUrl = new List<String>();
            if (mac.Image != null)
            {
                string imageBase64Data = Convert.ToBase64String(mac.Image);
                string imageDataUrl = string.Format("data:image/jpg;base64,{0}", imageBase64Data);
                ViewBag.ImageDataUrl = imageDataUrl;
            }
            else
            {
                ViewBag.ImageDataUrl = (new string(""));
            }
            if (!String.IsNullOrEmpty(message))
            {
                ViewBag.Message = message;
            }
            return View(mac);
        }
    }
}