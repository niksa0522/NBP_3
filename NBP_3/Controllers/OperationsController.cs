using NBP_3.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System;
using NBP_3.MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;

namespace NBP_3.Controllers
{
    public class OperationsController : Controller
    {

        private UserManager<ApplicationUser> _userManager;
        private RoleManager<ApplicationRole> _roleManager;
        private readonly IMongoDatabase database;

        public OperationsController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IMongoDatabase database)
        {
            this.database = database;
            this._userManager = userManager;
            this._roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public ViewResult Create() => View();


        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser appUser = new ApplicationUser
                {
                    UserName = user.Username,
                };

                IdentityResult result = await _userManager.CreateAsync(appUser, user.Password);

                //Adding User to Admin Role
                await _userManager.AddToRoleAsync(appUser, "Admin");

                if (result.Succeeded)
                {
                    MongoDBLogic.CreateUser(user.Username, database);
                    ViewBag.Message = "User Created Successfully";
                }
                    
                else
                {
                    foreach (IdentityError error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                }
            }
            return View(user);
        }

        public IActionResult CreateRole() => View();

        [HttpPost]
        public async Task<IActionResult> CreateRole([Required] string name)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await _roleManager.CreateAsync(new ApplicationRole() { Name = name });
                if (result.Succeeded)
                    ViewBag.Message = "Role Created Successfully";
                else
                {
                    foreach (IdentityError error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                }
            }
            return View();
        }
    }
}
