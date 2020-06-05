using Antlr.Runtime.Misc;
using EasyBlog.Models;
using System;
using System.Linq;
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
            return View(userInformationModel);
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
            try
            {
                using (EasyBlogEntities db = new EasyBlogEntities())
                {
                    UserInformation userInformation = db.UserInformations.Where(x => x.email == email && x.phone == oldPhone).SingleOrDefault();
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
                return Json("exception", JsonRequestBehavior.AllowGet);
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
    }
}