using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using MyScore.Core.Domain;
using MyScore.Models;
using MyScore.Persistence;
using MyScore.Persistence.Consts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MyScore.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private AppUserManager _userManager;
        private RoleManager<Role> _roleManager;
        private SignInManager<User, string> _signInManager;

        public HomeController()
        {

        }
        public HomeController(AppUserManager userManager, RoleManager<Role> roleManager, SignInManager<User, string> signInManager)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._signInManager = signInManager;
        }

        public AppUserManager UserManager { 
            get { 
                return this._userManager ?? HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
            private set
            {
                this._userManager = value;
            }
        }
        public RoleManager<Role> RoleManager
        {
            get
            {
                return this._roleManager ?? HttpContext.GetOwinContext().Get<RoleManager<Role>>();
            }
            private set
            {
                this._roleManager = value;
            }
        }

        public SignInManager<User, string> SignInManager
        {
            get
            {
                return this._signInManager ?? HttpContext.GetOwinContext().Get<SignInManager<User, string>>();
            }
            private set
            {
                this._signInManager = value;
            }
        }

        public ActionResult Index()
        {
            /*
             * 
             * 
            var roleUser = new Role(RoleNames.RoleUser);
            await this._roleManager.CreateAsync(roleUser);

            //initializing some users
            var users = new List<User>
            {
                new User
                {
                    UserName = "vinh",
                    Email = "vinh@gmail.com"
                },
                new User
                {
                    UserName = "tam",
                    Email = "tiger@test.com"
                },
            };

            foreach (var user in users)
            {
                //Creating user
                await this._userManager.CreateAsync(user, "User11");

                var userFromDb = await this._userManager.FindByNameAsync(user.UserName);

                //Registering user to role: User
                await this._userManager.AddToRoleAsync(userFromDb.Id, roleUser.Name);
            }

            */

            return View();
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel loginViewModel, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(loginViewModel);
            }

            var result = await this.SignInManager.PasswordSignInAsync(loginViewModel.UserName, loginViewModel.Password, false, false);
            if(result == SignInStatus.Success)
                return RedirectToLocal(returnUrl);

            ModelState.AddModelError("", "Invalid login attempt.");
            return View(loginViewModel);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}