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
    [Authorize]
    public class CartController : Controller
    {

        private readonly IMongoDatabase database;

        public CartController(IMongoDatabase database)
        { 
            this.database = database;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AddAccessoryToCart(string id)
        {
            string name = User.Identity.Name;
            MongoDBLogic.AddAccessoryToCart(name, id, database);
            string Message = new string("Product added to cart!");
            return RedirectToAction("ViewAccessory", "Home", new { id, Message });
        }
        public IActionResult AddPartToCart(string id)
        {
            string name = User.Identity.Name;
            MongoDBLogic.AddPartToCart(name, id, database);
            string Message = new string("Product added to cart!");
            return RedirectToAction("ViewPart", "Home", new { id, Message });
        }


        public IActionResult ShowCart()
        {
            string name = User.Identity.Name;
            MongoUser user = MongoDBLogic.FindUser(name, database);
            return View(user.inCart);
        }
        public IActionResult DeleteProduct(string id)
        {
            string name = User.Identity.Name;
            MongoDBLogic.RemoveProductFromCart(name,id, database);
            return RedirectToAction("ShowCart","Cart");
        }
        public IActionResult Checkout()
        {
            string name = User.Identity.Name;
            float price = MongoDBLogic.GetFullPrice(name,database);
            CheckoutInput input = new CheckoutInput();
            input.Price = price;
            return View(input);
        }
        [HttpPost]
        public IActionResult Checkout(CheckoutInput input)
        {
            string name = User.Identity.Name;
            MongoUser user = MongoDBLogic.FindUser(name,database);

            MongoOrder order = new MongoOrder();

            order.FullName = input.FullName;
            order.Price = MongoDBLogic.GetFullPrice(name, database);
            order.Phone = input.Phone;
            order.orderDate = DateTime.Now;
            order.OrderStatus = new List<String>();
            order.OrderStatus.Add("Order Made");
            order.Address=input.Address;
            order.user = new MongoDBRef("User", user.Id);
            order.produtcts = user.inCart;

            MongoDBLogic.Checkout(name,order,database);
            return RedirectToAction("Index","Home");
        }
    }
}
