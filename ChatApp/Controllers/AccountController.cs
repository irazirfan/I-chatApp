using ChatApp.Common;
using ChatApp.Domain.Abstract;
using ChatApp.Models;
using System;
using System.Web.Mvc;

namespace ChatApp.Controllers
{
    public class AccountController : Controller
    {
        private IUser _UserRepo;
        public AccountController(IUser UserRepo)
        {
            this._UserRepo = UserRepo;
        }
        [HttpGet]
        public ActionResult Login()
        {
            if (MySession.Current.UserID > 0)
            {
                return RedirectToAction("Chat", "User");
            }
            UserModel objmodel = new UserModel();
            return View(objmodel);
        }
        [HttpPost]
        public ActionResult Login(UserModel objmodel)
        {
            if (objmodel.FormType == "Login")
            {
                return CreateLogin(objmodel);
            }
            else
            {
                ChatApp.Domain.Entity.User objentity = new Domain.Entity.User();
                objentity.CreatedOn = System.DateTime.Now;
                objentity.IsActive = true;
                objentity.FirstName = objmodel.FirstName;
                objentity.LastName = objmodel.LastName;
                objentity.UpdatedOn = System.DateTime.Now;
                objentity.Email = objmodel.Email;
                objentity.DOB = Convert.ToDateTime(objmodel.DOB);
                objentity.Gender = objmodel.Gender;
                var result = _UserRepo.SaveUser(objentity);
                if (!string.IsNullOrEmpty(result.Item2))
                {
                    objmodel.Error = result.Item2;
                    TempData["ReturnFrom"] = "SignUp";
                    return View("Login", objmodel);
                }
                objmodel.Email = objmodel.Email;
                return CreateLogin(objmodel);
            }
        }
        public ActionResult CreateLogin(UserModel objmodel)
        {
            var result = _UserRepo.CheckLogin(objmodel.Email);
            if (result != null)
            {
                MySession.Current.UserID = result.UserID;
                MySession.Current.FirstName = result.FirstName;
                MySession.Current.LastName = result.LastName;
                MySession.Current.Email = objmodel.Email;
                MySession.Current.ProfilePicture = CommonFunctions.GetProfilePicture(result.ProfilePicture, result.Gender); ;
                return RedirectToAction("Chat", "User");
            }
            else
            {
                TempData["ReturnFrom"] = "Login";
                objmodel.LoginError = "Login credentials is not valid. Please try again.";
                return View("Login", objmodel);
            }
        }
        public ActionResult Logout()
        {
            Session.Abandon();
            Session.Clear();
            return RedirectToAction("Login");
        }

    }
}
