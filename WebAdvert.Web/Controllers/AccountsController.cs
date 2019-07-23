using Amazon.Extensions.CognitoAuthentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebAdvert.Web.Models;
using System.Linq;
using Amazon.AspNetCore.Identity.Cognito;

namespace WebAdvert.Web.Controllers
{

    public class AccountsController :Controller
    {
        private readonly SignInManager<CognitoUser> _signInManager;
        private readonly UserManager<CognitoUser> _userManager;
        private readonly CognitoUserPool _pool;

        public AccountsController(SignInManager<CognitoUser> signInManager, UserManager<CognitoUser> userManager, CognitoUserPool pool )
        {
            this._signInManager = signInManager;
            this._userManager = userManager;
            this._pool = pool;
        }

        [HttpGet]
        public async Task<IActionResult> SignUp()
        {
            var model = new SignUpModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _pool.GetUser(model.Email);
                if (user.Status != null)
                {
                    ModelState.AddModelError("UserExists","User Already Exists");
                    return View(model);
                }
                user.Attributes.Add(Amazon.AspNetCore.Identity.Cognito.CognitoAttribute.Name.ToString(), model.Email);
                var createdUser = await _userManager.CreateAsync(user, model.Password).ConfigureAwait(false);
                if (createdUser.Succeeded)
                {
                    RedirectToAction("Confirm");
                }

                ModelState.AddModelError("UserExists", "User Already Exists");
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Confirm()
        {
            var model = new ConfirmModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Confirm(ConfirmModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email).ConfigureAwait(false);
                if (user == null)
                {
                    ModelState.AddModelError("NotFound", "A user with given Email is not valid");
                }

                //if (_userManager is CognitoUserManager<CognitoUser> users)
                //{

                //}
                var result = await (_userManager as CognitoUserManager<CognitoUser>).ConfirmSignUpAsync(user, model.Code, true).ConfigureAwait(false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError(item.Code, item.Description);
                    }
                }
            }
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Login()
        {
            var model = new LoginModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                                                                model.UserName,
                                                                model.Password,
                                                                model.RememberMe,
                                                                false
                                                                ).ConfigureAwait(false);
                if (result.Succeeded)
                {
                  return  RedirectToAction("Index", "Home");
                }
                else {
                    ModelState.AddModelError("LoginError", "Email or password is not valid");
                }
            }
            return View(model);
        }
    }



}
