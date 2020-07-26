using EasyBlog.Helpers;
using EasyBlog.Models;
using EasyBlog.Models.RequestModels;
using Microsoft.Ajax.Utilities;
using System;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.EnterpriseServices;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace EasyBlog.Controllers
{
    public class UserController : Controller
    {
        private SecurityUtilize securityUtilize = new SecurityUtilize();
        private UserInformationModel userInformationModel = new UserInformationModel();
        private EasyBlogEntities db = new EasyBlogEntities();

        public ActionResult Login()
        {
            HttpContext.Session.Clear();
            HttpContext.Session["UserInformation"] = null;
            return View();
        }
        public ActionResult Register()
        {
            return View();
        }
        public JsonResult Authorization(UserLoginModel userLoginModel)
        {
            try
            {
                if (IsAnyNullOrEmpty(userLoginModel))
                {
                    return Json(ResponseMessages.RequiredFields, JsonRequestBehavior.AllowGet);
                }
                else if (IsValidEmail(userLoginModel.email) != ResponseMessages.Success)
                {
                    return Json(ResponseMessages.InvalidEmailAddress, JsonRequestBehavior.AllowGet);
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
                return Json(ResponseMessages.Success, JsonRequestBehavior.AllowGet);
            }
            catch (System.InvalidOperationException exception)
            {
                Console.WriteLine(exception);
                return Json(ResponseMessages.LoginException, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult ConfirmEmail(string email)
        {
            try
            {
                if (IsValidEmail(email) != ResponseMessages.Success)
                {
                    return Json(ResponseMessages.InvalidEmailAddress, JsonRequestBehavior.AllowGet);
                }
                UserInformation userInformation = db.UserInformations.Single( x => x.email == email);
                if (userInformation == null)
                {
                    return Json(ResponseMessages.InvalidUser, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string response = PhoneNumberWithStar(userInformation.phone) + "," + EmailAddressWithStar(userInformation.email);
                    return Json(response, JsonRequestBehavior.AllowGet);
                }
            }
            catch (System.InvalidOperationException exception)
            {
                Console.WriteLine(exception);
                return Json(ResponseMessages.InvalidUser, JsonRequestBehavior.AllowGet);
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
            string responseMessage = CheckUserArguments(userCreateModel);
            if (responseMessage != ResponseMessages.Success)
            {
                return Json(responseMessage, JsonRequestBehavior.AllowGet);
            }
            try
            {
                string isExisting = IsExistingUser(userCreateModel);
                if (isExisting != ResponseMessages.Success)
                {
                    return Json(new Response(isExisting, ResponseMessages.Error), JsonRequestBehavior.AllowGet);
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
                return Json(new Response(ResponseMessages.CreateAccount, ResponseMessages.Success), JsonRequestBehavior.AllowGet);
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException exception)
            {
                Console.WriteLine(exception);
                return Json(new Response(ResponseMessages.DatabaseException, ResponseMessages.Error), JsonRequestBehavior.AllowGet);
            }
        }
        private string CheckUserArguments(UserCreateModel userCreateModel)
        {
            if (userCreateModel.password != userCreateModel.confirmPassword)
            {
                return ResponseMessages.DifferentConfirmationPassword;
            }
            else if (string.IsNullOrWhiteSpace(userCreateModel.email) || string.IsNullOrWhiteSpace(userCreateModel.name)
                || string.IsNullOrWhiteSpace(userCreateModel.surname) || string.IsNullOrWhiteSpace(userCreateModel.password))
            {
                return ResponseMessages.RequiredFields;
            }
            else if (IsValidEmail(userCreateModel.email) != ResponseMessages.Success)
            {
                return ResponseMessages.InvalidEmailAddress;
            }
            else if (!string.IsNullOrWhiteSpace(userCreateModel.phone))
            {
                string responseMessage = IsValidPhone(userCreateModel.phone);
                if(responseMessage != ResponseMessages.Success)
                {
                    return responseMessage;
                }
            }
            return ResponseMessages.Success;
        }
        private string IsExistingUser(UserCreateModel userCreateModel)
        {
            try
            {
                UserInformation userInformationEmailCheck = db.UserInformations.Where(x => x.email == userCreateModel.email).SingleOrDefault();
                if (userInformationEmailCheck != null)
                {
                    return ResponseMessages.ExistingEmailAddress;
                }
                if (!string.IsNullOrEmpty(userCreateModel.phone))
                {
                    UserInformation userInformationPhoneCheck = db.UserInformations.Where(x => x.phone == userCreateModel.phone).SingleOrDefault();
                    if (userInformationPhoneCheck != null)
                    {
                        return ResponseMessages.ExistingPhoneNumber;
                    }
                }
                return ResponseMessages.Success;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return ResponseMessages.UnexpectedSystemException;
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
        private string IsValidEmail(string email)
        {
            if (!string.IsNullOrEmpty(email) && email.Contains('@'))
            {
                return ResponseMessages.Success;
            }
            else
            {
                return ResponseMessages.InvalidEmailAddress;
            }
        }
        private string IsValidPhone(string phone)
        {
            string validNumber = phone.Trim('+');
            if (!phone.Contains('+') || validNumber.Length <= 10)
            {
                return ResponseMessages.MissingCountryCode;
            }
            else if (!validNumber.All(char.IsDigit))
            {
                return ResponseMessages.InvalidPhoneNumber;
            }
            return ResponseMessages.Success;
        }
        private bool IsAnyNullOrEmpty(object myObject)
        {
            foreach (PropertyInfo propertyInfo in myObject.GetType().GetProperties())
            {
                if (propertyInfo.PropertyType == typeof(string))
                {
                    string value = (string)propertyInfo.GetValue(myObject);
                    if (string.IsNullOrEmpty(value))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public JsonResult Handshake(string method, string sendTo)
        {
            try {
                if (method == "email")
                {
                    string code = CreateCookie("Handshake");
                    if (code != null)
                    {
                        EmailHandler emailHadler = new EmailHandler();
                        EmailModel request = new EmailModel();
                        request.toEmail = sendTo;
                        request.subject = "Verification";
                        request.body = "Verification code: " + code;
                        emailHadler.SendEmail(request);
                    }
                }

                return Json(ResponseMessages.LoginException, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Json(ResponseMessages.LoginException, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult CheckCode(string code)
        {
            try
            {
                HttpCookie cookie = HttpContext.Request.Cookies.Get("Handshake");
                if(cookie.Value == code)
                {
                    return Json(ResponseMessages.Success, JsonRequestBehavior.AllowGet);
                }

                return Json(ResponseMessages.Error, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Json(ResponseMessages.LoginException, JsonRequestBehavior.AllowGet);
            }
        }

        private string CreateCookie(string name)
        {
            try
            {
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                Random random = new Random();               
                string value = new string(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray());
                HttpCookie cookie = new HttpCookie(name, value);
                DateTime now = DateTime.Now;
                cookie.Expires = now.AddSeconds(60);
                Response.Cookies.Add(cookie);
                return value;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
    }
}