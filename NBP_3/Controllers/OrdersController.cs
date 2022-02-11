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
    public class OrdersController : Controller
    {
        private readonly IMongoDatabase database;

        public OrdersController(IMongoDatabase database)
        {

            this.database = database;
        }

        public IActionResult Index()
        {
            OrderList ol = new OrderList();
            long documents;
            if (User.IsInRole("Admin"))
            {
                
                List<MongoOrder> orders = MongoDBLogic.GetOrders(1,database);
                documents = MongoDBLogic.GetOrderNum(database);
                ol.Orders = orders;
            }
            else
            {
                string name = User.Identity.Name;
                List<MongoOrder> orders = MongoDBLogic.GetOrders(1,name, database);
                documents = MongoDBLogic.GetOrderNum(name,database);
                ol.Orders = orders;
            }
            int pageSize = 8;
            ol.PageIndex = 1;
            ol.TotalPages = (int)Math.Ceiling((int)documents / (double)pageSize);
            return View(ol);
        }
        [HttpPost]
        public IActionResult Index(OrderList ol)
        {
            int pageIndex;
            if (!String.IsNullOrEmpty(Request.Form["Next"]))
            {
                pageIndex = ol.PageIndex + 1;
            }
            else if (!String.IsNullOrEmpty(Request.Form["Previous"]))
            {
                pageIndex = ol.PageIndex - 1;
            }
            else
            {
                pageIndex = 1;
            }

            long documents;
            if (User.IsInRole("Admin"))
            {

                List<MongoOrder> orders = MongoDBLogic.GetOrders(pageIndex,ol, database);
                documents = MongoDBLogic.GetOrderNum(database);
                ol.Orders = orders;
            }
            else
            {
                string name = User.Identity.Name;
                List<MongoOrder> orders = MongoDBLogic.GetOrders(pageIndex, name, database);
                documents = MongoDBLogic.GetOrderNum(name, database);
                ol.Orders = orders;
            }
            int pageSize = 8;
            ol.PageIndex = pageIndex;
            ol.TotalPages = (int)Math.Ceiling((int)documents / (double)pageSize);
            return View(ol);
        }

        public IActionResult View(string id)
        {
            MongoOrder order = MongoDBLogic.GetOrder(id, database);
            return View(order);
        }
        public IActionResult AddOrderInfo(MongoOrder order, string orderInfo, string Id)
        {
            if (orderInfo== null)
            {

            }
            else
            {
                MongoDBLogic.AddInfoToOrder(Id, orderInfo, database);
            }
            return RedirectToAction("View", "Orders", new { id = Id });
        }
    }
}
