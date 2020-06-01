using EasyBlog.Models;
using System.Web.Mvc;

namespace EasyBlog.Controllers
{
    public class PageController : Controller
    {
        // GET: Page
        public ActionResult Administer(UserInformationModel userInformationModel)
        {
            if(userInformationModel.email == null || Session["UserInformation"] == null)
            {
                return RedirectToAction("Login","User");
            }
            return View(userInformationModel);
        }
    }
}