using Antlr.Runtime.Misc;
using EasyBlog.Helpers;
using EasyBlog.Models;
using System;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;
using System.Web.Mvc;

namespace EasyBlog.Controllers
{
    public class AdminController : Controller
    {
        // GET: Page
        public ActionResult Home(UserInformationModel userInformationModel)
        {
            if(userInformationModel.email == null || Session["UserInformation"] == null)
            {
                return RedirectToAction("Login","User");
            }
            return View(userInformationModel);
        }
        public ActionResult CreateBlog(UserInformationModel userInformationModel)
        {
            if (userInformationModel.email == null || Session["UserInformation"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            return View(userInformationModel);
        }
        public ActionResult Settings(UserInformationModel userInformationModel)
        {
            if (userInformationModel.email == null || Session["UserInformation"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            else
            {
                long id = long.Parse(Session["UserInformation"].ToString());
                userInformationModel = RefreshUserInformationModel(id);
                if (userInformationModel == null)
                {
                    return RedirectToAction("Login", "User");
                }
                return View(userInformationModel);
            }
        }
        public ActionResult UpdateBlog(UserInformationModel userInformationModel)
        {
            if (userInformationModel.email == null || Session["UserInformation"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            return View(userInformationModel);
        }

        public ActionResult Logout()
        {
            Session["UserInformation"] = null;
            return RedirectToAction("Login", "User");
        }

        public JsonResult UpdateEmail(string oldEmail, string newEmail)
        {
            string response = "Success";
            try
            {
                if (string.IsNullOrEmpty(newEmail))
                {
                    response = "Email can't be empty!";
                    return Json(response, JsonRequestBehavior.AllowGet);
                }
                if(!(newEmail.Contains('@') && newEmail.Contains('.')))
                {
                    response = "Invalid email address!";
                    return Json(response, JsonRequestBehavior.AllowGet);
                }
                using (EasyBlogEntities db = new EasyBlogEntities())
                {
                    UserInformation userInformation = db.UserInformations.Where(x => x.email == oldEmail).SingleOrDefault();
                    UserLogin userLogin = db.UserLogins.Where(x => x.email == oldEmail).SingleOrDefault();
                    if (userInformation != null && userLogin != null)
                    {
                        userInformation.email = newEmail;
                        db.SaveChanges();
                        userLogin.email = newEmail;
                        db.SaveChanges();
                        return Json(response, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        response = "System Error!";
                        return Json(response, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException e)
            {
                Console.WriteLine(e);
                response = "This user already exists!";
                return Json(response, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                response = "System Error!";
                Console.WriteLine(e);
                return Json(response, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult UpdatePhone(string oldPhone, string newPhone, string email)
        {
            string response = "Success";
            if (!string.IsNullOrWhiteSpace(newPhone))
            {
                string validNumber = newPhone.Trim('+');
                if (!newPhone.Contains('+') || validNumber.Length <= 10)
                {
                    response = "Please add country code with + symbol!";
                    return Json(response, JsonRequestBehavior.AllowGet);
                }
                else if (!validNumber.All(char.IsDigit))
                {
                    response =  "Given phone number is not valid";
                    return Json(response, JsonRequestBehavior.AllowGet);
                }
            }
            try
            {
                using (EasyBlogEntities db = new EasyBlogEntities())
                {
                    UserInformation userInformation = db.UserInformations.Where(x => x.email == email && x.phone == oldPhone).SingleOrDefault();

                    if (!string.IsNullOrEmpty(newPhone)) {
                        UserInformation newPhoneCheck = db.UserInformations.Where(x => x.phone == newPhone).SingleOrDefault();
                        if (newPhoneCheck != null)
                        {
                            response = "This phone number belongs to someone else!";
                            return Json(response, JsonRequestBehavior.AllowGet);
                        }
                    }
                    if (userInformation != null)
                    {
                        userInformation.phone = newPhone;
                        db.SaveChanges();
                        return Json(response, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Json("System Error.", JsonRequestBehavior.AllowGet);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        private UserInformationModel RefreshUserInformationModel(long id)
        {
            try {
                using (EasyBlogEntities db = new EasyBlogEntities())
                {
                    UserInformation userInformation = db.UserInformations.Where(x => x.id == id).SingleOrDefault();
                    UserInformationModel userInformationModel = new UserInformationModel();
                    userInformationModel.email = userInformation.email;
                    userInformationModel.phone = userInformation.phone;
                    userInformationModel.createdDate = userInformation.createdDate;
                    userInformationModel.modifiedDate = userInformation.modifiedDate;
                    userInformationModel.lastLoginDate = userInformation.lastLoginDate;
                    userInformationModel.name = userInformation.name;
                    userInformationModel.surname = userInformation.surname;
                    return userInformationModel;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public JsonResult ResetPassword(string email, string oldPassword, string newPassword)
        {
            SecurityUtilize securityUtilize = new SecurityUtilize();
            string response = "Success";
            try
            {
                using (EasyBlogEntities db = new EasyBlogEntities())
                {
                    UserLogin userLogin = db.UserLogins.Where(x => x.email == email).SingleOrDefault();
                    if (securityUtilize.Encrypt(oldPassword) == userLogin.password)
                    {
                        userLogin.password = securityUtilize.Encrypt(newPassword);
                        db.SaveChanges();
                        return Json(response, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        response = "Wronge password!";
                        return Json(response, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception e){
                Console.WriteLine(e);
                return Json("System Error.", JsonRequestBehavior.AllowGet);
            }
        }
    }
}