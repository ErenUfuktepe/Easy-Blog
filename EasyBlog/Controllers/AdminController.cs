using Antlr.Runtime.Misc;
using EasyBlog.Models;
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
            return View(userInformationModel);
        }
        public ActionResult Settings(UserInformationModel userInformationModel)
        {
            return View(userInformationModel);
        }
        public ActionResult UpdateBlog(UserInformationModel userInformationModel)
        {
            return View(userInformationModel);
        }

        public ActionResult Logout()
        {
            Session["UserInformation"] = null;
            return RedirectToAction("Login", "User");
        }
    }
}