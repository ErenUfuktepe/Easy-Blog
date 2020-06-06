using EasyBlog.Helpers;
using EasyBlog.Models;
using EasyBlog.Models.RequestModels;
using Microsoft.Ajax.Utilities;
using System;
using System.ComponentModel.DataAnnotations;
using System.EnterpriseServices;
using System.Linq;
using System.Web.Mvc;

namespace EasyBlog.Controllers
{
    public class UserController : Controller
    {
        private SecurityUtilize securityUtilize = new SecurityUtilize();
        private UserInformationModel userInformationModel = new UserInformationModel();
        
        public ActionResult Login()
        {
            Session["UserInformation"] = null;
            return View();
        }
        public ActionResult Register()
        {
            return View();
        }
        public JsonResult Authorization(UserLoginModel userLoginModel)
        {
            string response = "";
            try
            {
                using (EasyBlogEntities db = new EasyBlogEntities())
                {
                    if(string.IsNullOrWhiteSpace(userLoginModel.email) || string.IsNullOrWhiteSpace(userLoginModel.password))
                    {
                        response = "Fill the required fields!";
                        return Json(response, JsonRequestBehavior.AllowGet);
                    }
                    if (!(userLoginModel.email.Contains('@') && userLoginModel.email.Contains('.')))
                    {
                        response = "Invalid email address!";
                        return Json(response, JsonRequestBehavior.AllowGet);
                    }

                    string password = securityUtilize.Encrypt(userLoginModel.password);
                    UserLogin userLogin = db.UserLogins.Single(x => x.email == userLoginModel.email && x.password == password);
                    UserInformation userInformation = db.UserInformations.Single(x => x.id == userLogin.id);

                    userInformationModel.email = userInformation.email;
                    userInformationModel.name = userInformation.name;
                    userInformationModel.surname = userInformation.surname;
                    userInformationModel.phone = userInformation.phone;
                    userInformationModel.createdDate = userInformation.createdDate;
                    userInformationModel.modifiedDate = userInformation.modifiedDate;
                    userInformationModel.lastLoginDate = userInformation.lastLoginDate;
                    Session["UserInformation"] = userInformation.id;
                    TempData["UserInformation"] = userInformationModel;
                    response = "Success";
                }
            }
            catch (System.InvalidOperationException exception)
            {
                Console.WriteLine(exception);
                response = "Invalid email address or password. Please try again.";
                return Json(response, JsonRequestBehavior.AllowGet);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ConfirmEmail(string email)
        {
            string response = "User not found";
            try
            {
                using (EasyBlogEntities db = new EasyBlogEntities())
                {
                    if (string.IsNullOrWhiteSpace(email))
                    {
                        response = "Fill the required fields!";
                        return Json(response, JsonRequestBehavior.AllowGet);
                    }
                    if (!(email.Contains('@') && email.Contains('.')))
                    {
                        response = "Invalid email address!";
                        return Json(response, JsonRequestBehavior.AllowGet);
                    }

                    UserInformation userInformation = db.UserInformations.Single( x => x.email == email);

                    if (string.IsNullOrWhiteSpace(userInformation.phone) || string.IsNullOrEmpty(userInformation.phone))
                    {
                        response = PhoneNumberWithStar(userInformation.phone) + "," + EmailAddressWithStar(userInformation.email);
                    }
                    else
                    {
                        response = PhoneNumberWithStar(userInformation.phone) + "," + EmailAddressWithStar(userInformation.email); 
                    }
                    return Json(response, JsonRequestBehavior.AllowGet);
                }
            }
            catch (System.InvalidOperationException exception)
            {
                Console.WriteLine(exception);
                return Json(response, JsonRequestBehavior.AllowGet);
            }
        }
        private string PhoneNumberWithStar(string phone)
        {
            if(string.IsNullOrEmpty(phone) || string.IsNullOrWhiteSpace(phone))
            {
                return "";
            }

            char[] phoneFormat = phone.ToCharArray();
            int numberOfStars = (phone.Length - 4);
            string phoneNumberWithStar = "";

            for (int index = 0; index < phone.Length; index++)
            {
                if (index < numberOfStars)
                {
                    phoneNumberWithStar += '*';
                }
                else
                {
                    phoneNumberWithStar += phoneFormat[index];
                }
            }
            return phoneNumberWithStar;
        }
        private string EmailAddressWithStar(string email)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrWhiteSpace(email))
            {
                return "";
            }

            char[] emailFormat = email.ToCharArray();
            string emailWithStar = "";
            bool flag = false;

            for (int index = 0; index < email.Length; index++)
            {
                if (index < 4 || flag)
                {
                    emailWithStar += emailFormat[index];
                }
                else
                {
                    if (emailFormat[index] == '@')
                    {
                        flag = true;
                        emailWithStar += emailFormat[index];
                        continue;
                    }
                    emailWithStar += '*';
                }
            }
            return emailWithStar;
        }
        public JsonResult CreateUser(UserCreateModel userCreateModel)
        {
            string check = CheckUserArguments(userCreateModel);
            if (check != "Success")
            {
                return Json(check, JsonRequestBehavior.AllowGet);
            }

            try
            {
                using (EasyBlogEntities db = new EasyBlogEntities())
                {
                    if (IsExistingUser(userCreateModel))
                    {
                        return Json("Existing User!", JsonRequestBehavior.AllowGet);
                    }

                    DateTime now = DateTime.Now;

                    UserInformation userInformation = new UserInformation();
                    userInformation.name = userCreateModel.name;
                    userInformation.surname = userCreateModel.surname;
                    userInformation.email = userCreateModel.email;
                    userInformation.phone = string.IsNullOrEmpty(userCreateModel.phone) ? "" : userCreateModel.phone; 
                    userInformation.createdDate = now;
                    userInformation.modifiedDate = now;
                    userInformation.lastLoginDate = now;
                    db.UserInformations.Add(userInformation);
                    db.SaveChanges();

                    UserLogin userLogin = new UserLogin();
                    userLogin.id = db.UserInformations.Single(x => x.email == userCreateModel.email).id;
                    userLogin.email = userCreateModel.email;
                    userLogin.password = securityUtilize.Encrypt(userCreateModel.password);
                    db.UserLogins.Add(userLogin);
                    db.SaveChanges();
                } 
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException exception)
            {
                Console.WriteLine(exception);
                return Json("Database Error", JsonRequestBehavior.AllowGet);
            }

            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        private string CheckUserArguments(UserCreateModel userCreateModel)
        {
            if (userCreateModel.password != userCreateModel.confirmPassword)
            {
                return "Password and Confirm Password are different!";
            }
            else if (string.IsNullOrWhiteSpace(userCreateModel.email) || string.IsNullOrWhiteSpace(userCreateModel.name)
                || string.IsNullOrWhiteSpace(userCreateModel.surname) || string.IsNullOrWhiteSpace(userCreateModel.password))
            {
                return "Please fill all required fields!";
            }
            else if (!(userCreateModel.email.Contains('@') && userCreateModel.email.Contains('.')))
            {
                return "Invalid email address!";
            }
            else if (!string.IsNullOrWhiteSpace(userCreateModel.phone))
            {
                string validNumber = userCreateModel.phone.Trim('+');
                if (!userCreateModel.phone.Contains('+') || validNumber.Length <= 10)
                {
                    return "Please add country code with + symbol!";
                }
                else if (!validNumber.All(char.IsDigit))
                {
                    return "Given phone number is not valid";
                }
            }
            return "Success";
        }
        private bool IsExistingUser(UserCreateModel userCreateModel)
        {
            using (EasyBlogEntities db = new EasyBlogEntities())
            {
                try
                {
                    UserInformation userInformationEmailCheck = db.UserInformations.Where(x => x.email == userCreateModel.email).SingleOrDefault();
                    UserInformation userInformationPhoneCheck = db.UserInformations.Where(x => x.phone == userCreateModel.phone).SingleOrDefault();

                    if (userInformationEmailCheck != null || userInformationPhoneCheck != null)
                    {
                        return true;
                    }
                    return false;
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    throw (exception);
                }
            }
        }
        public RedirectToRouteResult Entry()
        {
            UserInformationModel userInformation = TempData["UserInformation"] as UserInformationModel;
            TempData.Remove("UserInformation");
            if (Session["UserInformation"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            return RedirectToAction("Home","Admin", userInformation);
        }

    }
}