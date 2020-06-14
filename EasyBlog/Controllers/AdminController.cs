using Antlr.Runtime.Misc;
using EasyBlog.Helpers;
using EasyBlog.Models;
using EasyBlog.Models.RequestModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.DynamicData;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace EasyBlog.Controllers
{
    public class AdminController : Controller
    {
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
            else
            {
                CreateImagePathForUser(Session["UserInformation"].ToString());
                ViewData["SocialMedias"] = GetSocialMediaList();
                return View(userInformationModel);
            }
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
        public List<SocialMedia> GetSocialMediaList() {
            using (EasyBlogEntities db = new EasyBlogEntities())
            {
                try
                {
                    List<SocialMedia> socialMedias = db.SocialMedias.ToList();
                    return socialMedias;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return null;
                }
            }
        }
        public JsonResult UploadImages()
        {
            List<HttpPostedFileBase> files = new List<HttpPostedFileBase>();
            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase file = Request.Files[i];
                if (CheckMimeType(file))
                {
                    files.Add(file);
                }
                else
                {
                    return Json("You can only upload jpeg, jpg and png type files!");
                }
            }
            UploadImages(files);
            return Json("Success");
        }
        private void UploadImages(List<HttpPostedFileBase> files)
        {
            foreach (HttpPostedFileBase file in files)
            {
                System.IO.Stream fileContent = file.InputStream;
                string path = Path.Combine(Server.MapPath("~/Images/" + Session["UserInformation"].ToString()),
                                                   Path.GetFileName(file.FileName));
                file.SaveAs(path);
            }
        }
        private bool CheckMimeType(HttpPostedFileBase file)
        {
            if (file.ContentType.Contains("jpeg") || file.ContentType.Contains("png") || file.ContentType.Contains("jpg"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool CreateImagePathForUser(string userID)
        {
            string path = Path.Combine(Server.MapPath("~/Images"), userID);
            if (!(Directory.Exists(path)))
            {
                Directory.CreateDirectory(path);
                return true;
            }
            return false;
        }
        public JsonResult SaveTemplate(string template)
        {
            try
            {
                using (EasyBlogEntities db = new EasyBlogEntities())
                {
                    long id = long.Parse(Session["UserInformation"].ToString());
                    Template checkExisting = db.Templates.Where(x => x.id == id).SingleOrDefault();
                    if (checkExisting == null)
                    {
                        Template mainTemplate = new Template();
                        mainTemplate.templateName = template;
                        mainTemplate.id = id;
                        db.Templates.Add(mainTemplate);
                        db.SaveChanges();
                        return Json("Template saved successfully!", JsonRequestBehavior.AllowGet);
                    }
                    else if (checkExisting.templateName != template)
                    {
                        checkExisting.templateName = template;
                        db.SaveChanges();
                        return Json("Template updated successfully!", JsonRequestBehavior.AllowGet);
                    }
                    return Json("Template saved successfully!", JsonRequestBehavior.AllowGet);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                return Json("System Error!", JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult SaveMainComponents(MainComponentsModel mainComponentsModel)
        {
            try
            {
                using (EasyBlogEntities db = new EasyBlogEntities())
                {
                    long id = long.Parse(Session["UserInformation"].ToString());
                    Main checkMain = db.Mains.Where(x => x.id == id).SingleOrDefault();
                    if (checkMain == null)
                    {
                        Main main = new Main();
                        main.id = id;
                        main.logo = mainComponentsModel.logo;
                        main.title = mainComponentsModel.title;
                        main.textColor = mainComponentsModel.textColor;
                        main.hoverColor = mainComponentsModel.hoverColor;
                        main.titleColor = mainComponentsModel.titleColor;
                        db.Mains.Add(main);
                        db.SaveChanges();
                    }
                    else{
                        checkMain.logo = mainComponentsModel.logo;
                        checkMain.title = mainComponentsModel.title;
                        checkMain.textColor = mainComponentsModel.textColor;
                        checkMain.hoverColor = mainComponentsModel.hoverColor;
                        checkMain.titleColor = mainComponentsModel.titleColor;
                        db.SaveChanges();
                    }
                    string response = AddSocialMediaLink(mainComponentsModel.socialMediaList);
                    return Json(response, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Json("System Error!", JsonRequestBehavior.AllowGet);
            }
        }
        private string AddSocialMediaLink(List<SocialMediaModel> socialMediaModels)
        {
            try
            {
                long id = long.Parse(Session["UserInformation"].ToString());
                using (EasyBlogEntities db = new EasyBlogEntities())
                {
                    List<SocialMediaLink> socialMedias = db.SocialMediaLinks.Where(x => x.userID == id).ToList();
                    if (socialMedias.Count == 0)
                    {
                        foreach (SocialMediaModel socialMediaModel in socialMediaModels)
                        { 
                            SocialMediaLink socialMediaLink = new SocialMediaLink();
                            socialMediaLink.userID = id;
                            socialMediaLink.link = socialMediaModel.link;
                            socialMediaLink.socialMedia = db.SocialMedias.Where(x => x.code == socialMediaModel.socialMedia).SingleOrDefault().id;
                            db.SocialMediaLinks.Add(socialMediaLink);
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        bool flag = false;
                        foreach (SocialMediaModel socialMediaModel in socialMediaModels)
                        {
                            long socialMediaID = db.SocialMedias.Where(x => x.code == socialMediaModel.socialMedia).SingleOrDefault().id;
                            flag = false;
                            foreach (SocialMediaLink link in socialMedias)
                            {
                                if (link.id == socialMediaID)
                                {
                                    link.link = socialMediaModel.link;
                                    db.SaveChanges();
                                    flag = true;
                                    break;
                                }
                            }
                            if (!flag)
                            {
                                SocialMediaLink socialMediaLink = new SocialMediaLink();
                                socialMediaLink.userID = id;
                                socialMediaLink.link = socialMediaModel.link;
                                socialMediaLink.socialMedia = socialMediaID;
                                db.SocialMediaLinks.Add(socialMediaLink);
                                db.SaveChanges();
                            }
                        }
                    }
                    return "Main components saved successfully";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "System Error!";
            }
        }
        public JsonResult SaveNavigation(NavigationModel navigationModel)
        {
            try
            {
                long id = long.Parse(Session["UserInformation"].ToString());
                using (EasyBlogEntities db = new EasyBlogEntities())
                {
                    Navigation check = db.Navigations.Where(x => x.id == id).SingleOrDefault();
                    if (check == null)
                    {
                        Navigation navigation = new Navigation();
                        navigation.barColor = navigationModel.barColor;
                        navigation.logo = navigationModel.logo;
                        navigation.id = id;
                        db.Navigations.Add(navigation);
                        db.SaveChanges();
                    }
                    else
                    {
                        check.barColor = navigationModel.barColor;
                        check.logo = navigationModel.logo;
                        db.SaveChanges();
                    }
                    string response = SaveNavigationItems(navigationModel.navigationItems);
                    return Json(response, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Json("System Error!", JsonRequestBehavior.AllowGet);
            }
        }
        private string SaveNavigationItems(List<NavigationItemModel> navigationItemModels)
        {
            try
            {
                long id = long.Parse(Session["UserInformation"].ToString());
                using (EasyBlogEntities db = new EasyBlogEntities())
                {
                    List<NavigationItem> check = db.NavigationItems.Where(x => x.navID == id).ToList();
                    if(check.Count == 0)
                    {
                        foreach (NavigationItemModel navigationItemModel in navigationItemModels)
                        {
                            NavigationItem navigationItem = new NavigationItem();
                            navigationItem.navID = db.Navigations.Where(x =>  x.id == id).SingleOrDefault().id;
                            navigationItem.content = navigationItemModel.content;
                            navigationItem.sectionName = navigationItemModel.sectionName;
                            db.NavigationItems.Add(navigationItem);
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        bool flag = false;
                        foreach (NavigationItemModel navigationItemModel in navigationItemModels)
                        {
                            flag = false;
                            foreach (NavigationItem navigationItem in check)
                            {
                                if (navigationItemModel.content == navigationItem.content)
                                {
                                    navigationItem.sectionName = navigationItemModel.sectionName;
                                    flag = true;
                                    db.SaveChanges();
                                    break;
                                }
                            }
                            if (!flag)
                            {
                                NavigationItem navigationItem = new NavigationItem();
                                navigationItem.navID = id;
                                navigationItem.content = navigationItemModel.content;
                                navigationItem.sectionName = navigationItemModel.sectionName;
                                db.NavigationItems.Add(navigationItem);
                                db.SaveChanges();
                            }
                        }
                    }
                    return "Navigation Items saved.";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "System Error.";
            }
        }
        public JsonResult SaveHome(HomeModel homeModel)
        {
            try
            {
                using(EasyBlogEntities db = new EasyBlogEntities())
                {
                    long id = long.Parse(Session["UserInformation"].ToString());
                    Home check = db.Homes.Where(x => x.id == id).SingleOrDefault();
                    if (check == null)
                    {
                        Home home = new Home();
                        home.mainText = homeModel.mainText;
                        home.id = id;
                        home.textColor = homeModel.textColor;
                        home.background = homeModel.background;
                        db.Homes.Add(home);
                        db.SaveChanges();
                    }
                    else
                    {
                        check.mainText = homeModel.mainText;
                        check.textColor = homeModel.textColor;
                        check.background = homeModel.background;
                        db.SaveChanges();
                    }
                    return Json(SaveHomeSubTexts(homeModel.subTextList), JsonRequestBehavior.AllowGet);                    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Json("System Error!", JsonRequestBehavior.AllowGet);
            }
        }
        private string SaveHomeSubTexts(List<string> subTexts)
        {
            try
            {
                using (EasyBlogEntities db = new EasyBlogEntities())
                {
                    long id = long.Parse(Session["UserInformation"].ToString());
                    foreach (string text in subTexts)
                    {
                        HomeSubText homeSubText = new HomeSubText();
                        homeSubText.homeID = id;
                        homeSubText.subText = text;
                        db.HomeSubTexts.Add(homeSubText);
                        db.SaveChanges();
                    }
                    return "Home section saved.";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "System Error!";
            }
        }
        public JsonResult SaveAbout(AboutModel aboutModel)
        {
            try
            {
                using (EasyBlogEntities db = new EasyBlogEntities())
                {
                    long id = long.Parse(Session["UserInformation"].ToString());

                    About about = new About();
                    about.background = aboutModel.background;
                    about.image = aboutModel.image;
                    about.header = aboutModel.header;
                    about.subTitle = aboutModel.subTitle;
                    about.body = aboutModel.body;
                    about.frameColor = aboutModel.frame;
                    about.id = id;
                    db.Abouts.Add(about);
                    db.SaveChanges();
                    if (aboutModel.informationList != null )
                    {
                        foreach (List<string> information in aboutModel.informationList)
                        {
                            AboutInformation aboutInformation = new AboutInformation();
                            aboutInformation.aboutID = id;
                            aboutInformation.informationTitle = information[0];
                            aboutInformation.informationValue = information[1];
                            db.AboutInformations.Add(aboutInformation);
                            db.SaveChanges();
                        }
                    }
                    return Json("About section saved.", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Json("System Error!", JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult SavePortfolio(PortfolioModel portfolioModel)
        {
            try
            {
                using (EasyBlogEntities db = new EasyBlogEntities())
                {
                    long id = long.Parse(Session["UserInformation"].ToString());

                    Portfolio portfolio = new Portfolio();
                    portfolio.id = id;
                    portfolio.background = portfolioModel.background;
                    portfolio.header = portfolioModel.header;
                    db.Portfolios.Add(portfolio);
                    db.SaveChanges();
                    foreach (PortfolioCategoryModel categoryModel in portfolioModel.portfolioCategories)
                    {
                        PortfolioCategory portfolioCategory = new PortfolioCategory();
                        portfolioCategory.portfolioID = id;
                        portfolioCategory.category = categoryModel.category;
                        db.PortfolioCategories.Add(portfolioCategory);
                        db.SaveChanges();
                        long categoryID = db.PortfolioCategories.Where(x => x.category == categoryModel.category && x.portfolioID == id).SingleOrDefault().id;
                        foreach (string image in categoryModel.images)
                        {
                            PortfolioCategoryImageRelationship portfolioCategoryImageRelationship = new PortfolioCategoryImageRelationship();
                            portfolioCategoryImageRelationship.portfolioCategoryID = categoryID;
                            portfolioCategoryImageRelationship.image = image;
                            db.PortfolioCategoryImageRelationships.Add(portfolioCategoryImageRelationship);
                            db.SaveChanges();
                        }
                    }
                    return Json("Portfolio section saved.", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Json("System Error!", JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult SaveContact(ContactModel contactModel)
        {
            try
            {
                using (EasyBlogEntities db = new EasyBlogEntities())
                {
                    long id = long.Parse(Session["UserInformation"].ToString());

                    Contact contact = new Contact();
                    contact.id = id;
                    contact.header = contactModel.header;
                    contact.backgroundColor = contactModel.background;
                    contact.phone = contactModel.phone;
                    contact.email = contactModel.email;
                    contact.address = contactModel.address;
                    contact.city = contactModel.city;
                    contact.country = contactModel.country;
                    contact.state = contactModel.state;
                    db.Contacts.Add(contact);
                    db.SaveChanges();
                    return Json("Contact section saved.", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Json("System Error!", JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult SaveBlog(BlogModel blogModel)
        {
            try
            {
                using (EasyBlogEntities db = new EasyBlogEntities())
                {
                    long id = long.Parse(Session["UserInformation"].ToString());

                    Blog blog = new Blog();
                    blog.id = id;
                    blog.backgroundColor = blogModel.background;
                    blog.header = blogModel.header;
                    db.Blogs.Add(blog);
                    db.SaveChanges();
                    foreach (StoryModel storyModel in blogModel.stories)
                    {
                        Story story = new Story();
                        story.blogID = id;
                        story.body = storyModel.body;
                        story.title = storyModel.title;
                        story.image = storyModel.image;
                        db.Stories.Add(story);
                        db.SaveChanges();
                    }
                    return Json("Blog section saved.", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Json("System Error!", JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult SaveResume(ResumeModel resumeModel)
        {
            try
            {
                using (EasyBlogEntities db = new EasyBlogEntities())
                {
                    long id = long.Parse(Session["UserInformation"].ToString());

                    Resume resume = new Resume();
                    resume.header = resumeModel.header;
                    resume.background = resumeModel.background;
                    resume.id = id;
                    db.Resumes.Add(resume);
                    db.SaveChanges();
                    foreach (ResumeSectionModel section in resumeModel.resumeSections)
                    {
                        ResumeSection resumeSection = new ResumeSection();
                        resumeSection.header = section.header;
                        resumeSection.resumeID = id;
                        db.ResumeSections.Add(resumeSection);
                        db.SaveChanges();
                        long sectionID = db.ResumeSections.Where(x => x.header == section.header && x.id == id).SingleOrDefault().id;
                        foreach (ResumeSubSectionModel subSection in section.resumeSubSections)
                        {
                            ResumeSectionItem resumeSectionItem = new ResumeSectionItem();
                            resumeSectionItem.resumeSectionID = sectionID;
                            resumeSectionItem.header = subSection.header;
                            resumeSectionItem.date = subSection.date;
                            resumeSectionItem.location = subSection.location;
                            resumeSectionItem.explanation = subSection.explanation;
                            db.ResumeSectionItems.Add(resumeSectionItem);
                            db.SaveChanges();
                            long subSectionID = db.ResumeSectionItems.Where(x => x.header == subSection.header && x.id == sectionID).SingleOrDefault().id;
                            if (subSection.explanationItems.Count != 0 || subSection.explanationItems != null)
                            {
                                foreach (string explanationItem in subSection.explanationItems)
                                {
                                    ResumeSectionItemExplanation resumeSectionItemExplanation = new ResumeSectionItemExplanation();
                                    resumeSectionItemExplanation.resumeSectionItemID = subSectionID;
                                    resumeSectionItemExplanation.explanation = explanationItem;
                                    db.ResumeSectionItemExplanations.Add(resumeSectionItemExplanation);
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                  
                    return Json("Resume section saved.", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Json("System Error!", JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult HasBlog()
        {
            try
            {
                using (EasyBlogEntities db = new EasyBlogEntities())
                {
                    long id = long.Parse(Session["UserInformation"].ToString());
                    Template template = db.Templates.Where(x => x.id == id).SingleOrDefault();
                    if (template == null)
                    {
                        return Json("true", JsonRequestBehavior.AllowGet);
                    }
                    return Json("false", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Json("false", JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult DeletePage()
        {
            try
            {
                using (EasyBlogEntities db = new EasyBlogEntities())
                {
                    DeleteMain();
                    DeleteNavigation();
                    DeleteHome();
                    DeleteAbout();
                    DeleteSocialMedia();
                    DeletePortfolio();
                    DeleteContact();
                    DeleteResume();
                    DeleteBlog();
                    DeleteTemplate();
                    DeleteImages();
                    return Json("true", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Json("false", JsonRequestBehavior.AllowGet);
            }
        }
        private bool DeleteTemplate()
        {
            try
            {
                using (EasyBlogEntities db = new EasyBlogEntities())
                {
                    long id = long.Parse(Session["UserInformation"].ToString());
                    Template template = db.Templates.Where(x => x.id == id).SingleOrDefault();
                    db.Templates.Remove(template);
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
        private bool DeleteMain()
        {
            try
            {
                using (EasyBlogEntities db = new EasyBlogEntities())
                {
                    long id = long.Parse(Session["UserInformation"].ToString());
                    Main main = db.Mains.Where(x => x.id == id).SingleOrDefault();
                    db.Mains.Remove(main);
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
        private bool DeleteNavigation()
        {
            try
            {
                using (EasyBlogEntities db = new EasyBlogEntities())
                {
                    long id = long.Parse(Session["UserInformation"].ToString());
                    List<NavigationItem> navigationItems = db.NavigationItems.Where(x => x.navID == id).ToList();
                    db.NavigationItems.RemoveRange(navigationItems);
                    db.SaveChanges();
                    Navigation navigation = db.Navigations.Where(x => x.id == id).SingleOrDefault();
                    db.Navigations.Remove(navigation);
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
        private bool DeleteHome()
        {
            try
            {
                using (EasyBlogEntities db = new EasyBlogEntities())
                {
                    long id = long.Parse(Session["UserInformation"].ToString());
                    List<HomeSubText> homeSubTexts = db.HomeSubTexts.Where(x => x.homeID == id).ToList();
                    db.HomeSubTexts.RemoveRange(homeSubTexts);
                    db.SaveChanges();
                    Home home = db.Homes.Where(x => x.id == id).SingleOrDefault();
                    db.Homes.Remove(home);
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
        private bool DeleteAbout()
        {
            try
            {
                using (EasyBlogEntities db = new EasyBlogEntities())
                {
                    long id = long.Parse(Session["UserInformation"].ToString());
                    List<AboutInformation> aboutInformation = db.AboutInformations.Where(x => x.aboutID == id).ToList();
                    db.AboutInformations.RemoveRange(aboutInformation);
                    db.SaveChanges();
                    About about = db.Abouts.Where(x => x.id == id).SingleOrDefault();
                    db.Abouts.Remove(about);
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
        private bool DeleteBlog()
        {
            try
            {
                using (EasyBlogEntities db = new EasyBlogEntities())
                {
                    long id = long.Parse(Session["UserInformation"].ToString());
                    List<Story> stories = db.Stories.Where(x => x.blogID == id).ToList();
                    db.Stories.RemoveRange(stories);
                    db.SaveChanges();
                    Blog blog = db.Blogs.Where(x => x.id == id).SingleOrDefault();
                    db.Blogs.Remove(blog);
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
        private bool DeleteSocialMedia()
        {
            try
            {
                using (EasyBlogEntities db = new EasyBlogEntities())
                {
                    long id = long.Parse(Session["UserInformation"].ToString());
                    List<SocialMediaLink> socialMediaLinks = db.SocialMediaLinks.Where(x => x.userID == id).ToList();
                    db.SocialMediaLinks.RemoveRange(socialMediaLinks);
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
        private bool DeletePortfolio()
        {
            try
            {
                using (EasyBlogEntities db = new EasyBlogEntities())
                {
                    long id = long.Parse(Session["UserInformation"].ToString());
                    List<PortfolioCategory> portfolioCategories = db.PortfolioCategories.Where(x => x.portfolioID == id).ToList();
                    foreach(PortfolioCategory portfolioCategory in portfolioCategories)
                    {
                        List<PortfolioCategoryImageRelationship> portfolioCategoryImageRelationships = db.PortfolioCategoryImageRelationships.Where(x => x.portfolioCategoryID == portfolioCategory.id).ToList();
                        db.PortfolioCategoryImageRelationships.RemoveRange(portfolioCategoryImageRelationships);
                        db.SaveChanges();
                        db.PortfolioCategories.Remove(portfolioCategory);
                        db.SaveChanges();
                    }
                    Portfolio portfolio = db.Portfolios.Where(x => x.id == id).SingleOrDefault();
                    db.Portfolios.Remove(portfolio);
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
        private bool DeleteResume()
        {
            try
            {
                using (EasyBlogEntities db = new EasyBlogEntities())
                {
                    long id = long.Parse(Session["UserInformation"].ToString());
                    List<ResumeSectionItemExplanation> resumeSectionItemExplanations = db.ResumeSectionItemExplanations.Where(x => x.resumeSectionItemID == id).ToList();
                    db.ResumeSectionItemExplanations.RemoveRange(resumeSectionItemExplanations);
                    db.SaveChanges();
                    List<ResumeSectionItem> resumeSectionItems = db.ResumeSectionItems.Where(x => x.resumeSectionID == id).ToList();
                    db.ResumeSectionItems.RemoveRange(resumeSectionItems);
                    db.SaveChanges();
                    List<ResumeSection> resumeSections = db.ResumeSections.Where(x => x.resumeID == id).ToList();
                    db.ResumeSections.RemoveRange(resumeSections);
                    db.SaveChanges();
                    Resume resume = db.Resumes.Where(x => x.id == id).SingleOrDefault();
                    db.Resumes.Remove(resume);
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
        private bool DeleteContact()
        {
            try
            {
                using (EasyBlogEntities db = new EasyBlogEntities())
                {
                    long id = long.Parse(Session["UserInformation"].ToString());
                    Contact contact = db.Contacts.Where(x => x.id == id).SingleOrDefault();
                    db.Contacts.Remove(contact);
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
        private bool DeleteImages()
        {
            try
            {
                string path = Server.MapPath("~/Images/" + Session["UserInformation"].ToString());
                System.IO.DirectoryInfo directory = new DirectoryInfo(path);
                foreach (FileInfo file in directory.EnumerateFiles())
                {
                    file.Delete();
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
    }
}