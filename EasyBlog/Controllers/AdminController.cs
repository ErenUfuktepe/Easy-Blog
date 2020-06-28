using EasyBlog.Helpers;
using EasyBlog.Models;
using EasyBlog.Models.RequestModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace EasyBlog.Controllers
{
    public class AdminController : Controller
    {
        private EasyBlogEntities db = new EasyBlogEntities();

        public ActionResult Home(UserInformationModel userInformationModel)
        {
            if (userInformationModel.email == null || Session["UserInformation"] == null)
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
            CreateImagePathForUser(Session["UserInformation"].ToString());
            ViewData["SocialMedias"] = GetSocialMediaList();
            return View(userInformationModel);
        }
        public ActionResult Settings(UserInformationModel userInformationModel)
        {
            if (userInformationModel.email == null || Session["UserInformation"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            long id = long.Parse(Session["UserInformation"].ToString());
            userInformationModel = RefreshUserInformationModel(id);
            if (userInformationModel == null)
            {
                return RedirectToAction("Login", "User");
            }
            return View(userInformationModel);
        }
        public ActionResult UpdateBlog()
        {
            if (Session["UserInformation"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            ViewData["SocialMedias"] = GetSocialMediaList();
            return View(GetPageModel());
        }
        public ActionResult Logout()
        {
            HttpContext.Session.Abandon();
            Session["UserInformation"] = null;
            return RedirectToAction("Login", "User");
        }
        public JsonResult UpdateEmail(string oldEmail, string newEmail)
        {
            try
            {
                if (IsValidEmail(newEmail) != ResponseMessages.Success)
                {
                    return Json(ResponseMessages.InvalidEmailAddress, JsonRequestBehavior.AllowGet);
                }
                UserInformation userInformation = db.UserInformations.Where(x => x.email == oldEmail).SingleOrDefault();
                UserLogin userLogin = db.UserLogins.Where(x => x.email == oldEmail).SingleOrDefault();
                if (userInformation != null && userLogin != null)
                {
                    userInformation.email = newEmail;
                    db.SaveChanges();
                    userLogin.email = newEmail;
                    db.SaveChanges();
                    return Json(ResponseMessages.EmailAddressUpdated, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(ResponseMessages.UnexpectedSystemException, JsonRequestBehavior.AllowGet);
                }
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException e)
            {
                Console.WriteLine(e);
                return Json(ResponseMessages.ExistingEmailAddress, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Json(ResponseMessages.UnexpectedSystemException, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult UpdatePhone(string oldPhone, string newPhone, string email)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(newPhone))
                {
                    string validPhone = IsValidPhone(newPhone);
                    if (validPhone != ResponseMessages.Success)
                    {
                        return Json(new Response(validPhone, ResponseMessages.Error), JsonRequestBehavior.AllowGet);
                    }
                }
                if (!string.IsNullOrEmpty(newPhone))
                {
                    UserInformation newPhoneCheck = db.UserInformations.Where(x => x.phone == newPhone).SingleOrDefault();
                    if (newPhoneCheck != null)
                    {
                        return Json(new Response(ResponseMessages.ExistingPhoneNumber, ResponseMessages.Error), JsonRequestBehavior.AllowGet);
                    }
                }
                UserInformation userInformation = db.UserInformations.Where(x => x.email == email && x.phone == oldPhone).SingleOrDefault();            
                if (userInformation != null)
                {
                    userInformation.phone = newPhone;
                    db.SaveChanges();
                    return Json(new Response(ResponseMessages.PhoneNumberUpdated, ResponseMessages.Success), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new Response(ResponseMessages.UnexpectedSystemException, ResponseMessages.Error), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Json(new Response(ResponseMessages.UnexpectedSystemException, ResponseMessages.Error), JsonRequestBehavior.AllowGet);
            }
        }
        private UserInformationModel RefreshUserInformationModel(long id)
        {
            try {
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
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
        public JsonResult ResetPassword(string email, string oldPassword, string newPassword)
        {
            SecurityUtilize securityUtilize = new SecurityUtilize();
            try
            {
                UserLogin userLogin = db.UserLogins.Where(x => x.email == email).SingleOrDefault();
                if (securityUtilize.Encrypt(oldPassword) == userLogin.password)
                {
                    userLogin.password = securityUtilize.Encrypt(newPassword);
                    db.SaveChanges();
                    return Json(ResponseMessages.PasswordReset, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(ResponseMessages.WrongPassword, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e){
                Console.WriteLine(e);
                return Json(ResponseMessages.UnexpectedSystemException, JsonRequestBehavior.AllowGet);
            }
        }
        public List<SocialMedia> GetSocialMediaList() {
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
        public JsonResult UploadImages()
        {
            try
            {
                List<HttpPostedFileBase> files = new List<HttpPostedFileBase>();
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    HttpPostedFileBase file = Request.Files[i];
                    if (!(file.ContentType.Contains("jpeg") || file.ContentType.Contains("png") || file.ContentType.Contains("jpg")))
                    {
                        return Json(ResponseMessages.FileExtensionTypeException);
                    }
                    files.Add(file);
                }
                foreach (HttpPostedFileBase file in files)
                {
                    System.IO.Stream fileContent = file.InputStream;
                    string path = Path.Combine(Server.MapPath("~/Images/" + Session["UserInformation"].ToString()),
                                                       Path.GetFileName(file.FileName));
                    file.SaveAs(path);
                }
                return Json(ResponseMessages.UploadImageFiles);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Json(ResponseMessages.UploadImageFilesException);
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
                long id = long.Parse(Session["UserInformation"].ToString());
                Template checkExisting = db.Templates.Where(x => x.id == id).SingleOrDefault();
                if (checkExisting == null)
                {
                    Template mainTemplate = new Template();
                    mainTemplate.templateName = template;
                    mainTemplate.id = id;
                    db.Templates.Add(mainTemplate);
                    db.SaveChanges();
                    return Json(new Response(ResponseMessages.TemplateSave, ResponseMessages.Success), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    checkExisting.templateName = template;
                    db.SaveChanges();
                    return Json(new Response(ResponseMessages.TemplateUpdate, ResponseMessages.Success), JsonRequestBehavior.AllowGet);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                return Json(new Response(ResponseMessages.TemplateException, ResponseMessages.Error), JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult SaveMainComponents(MainComponentsModel mainComponentsModel)
        {
            try
            {
                string responseMessage = ResponseMessages.Success;
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
                    responseMessage = ResponseMessages.MainComponentsSave;
                }
                else{
                    if (mainComponentsModel.logo != null)
                    {
                        checkMain.logo = mainComponentsModel.logo;
                    }
                    checkMain.title = mainComponentsModel.title;
                    checkMain.textColor = mainComponentsModel.textColor;
                    checkMain.hoverColor = mainComponentsModel.hoverColor;
                    checkMain.titleColor = mainComponentsModel.titleColor;
                    db.SaveChanges();
                    responseMessage = ResponseMessages.MainComponentsUpdate;
                }
                if (mainComponentsModel.socialMediaList.Count() > 0)
                {
                    responseMessage = AddSocialMediaLink(mainComponentsModel.socialMediaList);
                    if(responseMessage == ResponseMessages.SocialMediaException)
                    {
                        return Json(new Response(responseMessage, ResponseMessages.Error), JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new Response(responseMessage, ResponseMessages.Success), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Json(new Response(ResponseMessages.MainComponentsException, ResponseMessages.Error), JsonRequestBehavior.AllowGet);
            }
        }
        private string AddSocialMediaLink(List<SocialMediaModel> socialMediaModels)
        {
            try
            {
                long id = long.Parse(Session["UserInformation"].ToString());
                List<SocialMediaLink> socialMedias = db.SocialMediaLinks.Where(x => x.userID == id).ToList();
                if (socialMedias.Count == 0)
                {
                    foreach (SocialMediaModel socialMediaModel in socialMediaModels)
                    {
                        if (socialMediaModel.link != null)
                        {
                            SocialMediaLink socialMediaLink = new SocialMediaLink();
                            socialMediaLink.userID = id;
                            socialMediaLink.link = socialMediaModel.link;
                            socialMediaLink.socialMedia = db.SocialMedias.Where(x => x.code == socialMediaModel.socialMedia).SingleOrDefault().id;
                            db.SocialMediaLinks.Add(socialMediaLink);
                            db.SaveChanges();
                        }
                    }
                    return ResponseMessages.MainComponentsSave;
                }
                else
                {
                    foreach (SocialMediaModel socialMediaModel in socialMediaModels)
                    {
                        bool flag = false;
                        long socialMediaID = db.SocialMedias.Where(x => x.code == socialMediaModel.socialMedia).SingleOrDefault().id;
                        foreach (SocialMediaLink link in socialMedias)
                        {
                            if (link.socialMedia == socialMediaID)
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
                    return ResponseMessages.MainComponentsUpdate;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return ResponseMessages.SocialMediaException;
            }
        }
        public JsonResult SaveNavigation(NavigationModel navigationModel)
        {
            try
            {
                string responseMessage = ResponseMessages.Success;
                long id = long.Parse(Session["UserInformation"].ToString());
                Navigation check = db.Navigations.Where(x => x.id == id).SingleOrDefault();
                if (check == null)
                {
                    Navigation navigation = new Navigation();
                    navigation.barColor = navigationModel.barColor;
                    navigation.logo = navigationModel.logo;
                    navigation.id = id;
                    db.Navigations.Add(navigation);
                    db.SaveChanges();
                    responseMessage = ResponseMessages.NavigationSave;
                }
                else
                {
                    check.barColor = navigationModel.barColor;
                    if (navigationModel.logo != null)
                    {
                        check.logo = navigationModel.logo;
                    }
                    db.SaveChanges();
                    responseMessage = ResponseMessages.NavigationUpdate;
                }
                responseMessage = SaveNavigationItems(navigationModel.navigationItems);
                if (responseMessage == ResponseMessages.NavigationException)
                {
                    return Json(new Response(responseMessage, ResponseMessages.Error), JsonRequestBehavior.AllowGet);
                }
                return Json( new Response(responseMessage, ResponseMessages.Success), JsonRequestBehavior.AllowGet);    
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Json(new Response(ResponseMessages.NavigationException, ResponseMessages.Error), JsonRequestBehavior.AllowGet);
            }
        }
        private string SaveNavigationItems(List<NavigationItemModel> navigationItemModels)
        {
            try
            {
                long id = long.Parse(Session["UserInformation"].ToString());
                List<NavigationItem> check = db.NavigationItems.Where(x => x.navID == id).ToList();
                if(check.Count == 0)
                {
                    foreach (NavigationItemModel navigationItemModel in navigationItemModels)
                    {
                        NavigationItem navigationItem = new NavigationItem();
                        navigationItem.navID = db.Navigations.Where(x =>  x.id == id).SingleOrDefault().id;
                        navigationItem.content = navigationItemModel.content;
                        navigationItem.sectionName = navigationItemModel.sectionName;
                        navigationItem.priority = navigationItemModel.priority;
                        db.NavigationItems.Add(navigationItem);
                        db.SaveChanges();
                    }
                    return ResponseMessages.NavigationSave;
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
                    return ResponseMessages.NavigationUpdate;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return ResponseMessages.NavigationException;
            }
        }
        public JsonResult SaveHome(HomeModel homeModel)
        {
            try
            {
                string responseMessage = ResponseMessages.Success;
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
                    responseMessage = ResponseMessages.HomeSave;
                }
                else
                {
                    check.mainText = homeModel.mainText;
                    check.textColor = homeModel.textColor;
                    if (homeModel.background != null)
                    {
                        check.background = homeModel.background;
                    }
                    db.SaveChanges();
                    responseMessage = ResponseMessages.HomeUpdate;
                }
                responseMessage = SaveHomeSubTexts(homeModel.subTextList);
                if (responseMessage == ResponseMessages.HomeException)
                {
                    return Json(new Response(responseMessage, ResponseMessages.Error), JsonRequestBehavior.AllowGet);
                }
                return Json(new Response(responseMessage, ResponseMessages.Success), JsonRequestBehavior.AllowGet);                                  
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Json(new Response(ResponseMessages.HomeException, ResponseMessages.Error), JsonRequestBehavior.AllowGet);
            }
        }
        private string SaveHomeSubTexts(List<string> subTexts)
        {
            try
            {
                long id = long.Parse(Session["UserInformation"].ToString());
                List<HomeSubText> check = db.HomeSubTexts.Where(x => x.homeID == id).ToList();
                if (check.Count() > 0)
                {
                    db.HomeSubTexts.RemoveRange(check);
                    db.SaveChanges();
                }
                foreach (string text in subTexts)
                {
                    HomeSubText homeSubText = new HomeSubText();
                    homeSubText.homeID = id;
                    homeSubText.subText = text;
                    db.HomeSubTexts.Add(homeSubText);
                    db.SaveChanges();
                }
                return ResponseMessages.HomeSave;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return ResponseMessages.HomeException;
            }
        }
        public JsonResult SaveAbout(AboutModel aboutModel)
        {
            try
            {
                string responseMessage = ResponseMessages.Success;
                long id = long.Parse(Session["UserInformation"].ToString());
                About check = db.Abouts.Where(x => x.id == id).SingleOrDefault();
                if (check == null)
                {
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
                    responseMessage = ResponseMessages.AboutSave;
                }
                else
                {
                    check.background = aboutModel.background;
                    if (aboutModel.image != null)
                    {
                        check.image = aboutModel.image;
                    }
                    check.header = aboutModel.header;
                    check.subTitle = aboutModel.subTitle;
                    check.body = aboutModel.body;
                    check.frameColor = aboutModel.frame;
                    db.SaveChanges();
                    responseMessage = ResponseMessages.AboutUpdate;
                }
                if (aboutModel.informationList != null )
                {
                    List<AboutInformation> checkInfo = db.AboutInformations.Where(x => x.aboutID == id).ToList();
                    if (checkInfo.Count() > 0)
                    {
                        db.AboutInformations.RemoveRange(checkInfo);
                    }
                    foreach (List<string> information in aboutModel.informationList)
                    {
                        AboutInformation aboutInformation = new AboutInformation();
                        aboutInformation.aboutID = id;
                        aboutInformation.informationTitle = information[0];
                        aboutInformation.informationValue = information[1];
                        db.AboutInformations.Add(aboutInformation);
                        db.SaveChanges();
                        responseMessage = ResponseMessages.AboutUpdate;
                    }
                }
                return Json(new Response(responseMessage, ResponseMessages.Success), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Json(new Response(ResponseMessages.AboutException, ResponseMessages.Error), JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult SavePortfolio(PortfolioModel portfolioModel)
        {
            try
            {
                long id = long.Parse(Session["UserInformation"].ToString());
                Portfolio check = db.Portfolios.Where(x => x.id == id).SingleOrDefault();
                if (check != null )
                {
                    DeletePortfolio();
                }
                Portfolio portfolio = new Portfolio();
                portfolio.id = id;
                portfolio.background = portfolioModel.background;
                portfolio.header = portfolioModel.header;
                db.Portfolios.Add(portfolio);
                db.SaveChanges();
                if (portfolioModel.portfolioCategories != null || portfolioModel.portfolioCategories.Count() !=  0)
                {
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
                }
                return Json(new Response(ResponseMessages.PortfolioSaved, ResponseMessages.Success), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Json(new Response(ResponseMessages.PortfolioException, ResponseMessages.Error ), JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult SaveContact(ContactModel contactModel)
        {
            try
            {
                long id = long.Parse(Session["UserInformation"].ToString());
                Contact checkContact = db.Contacts.Where(x => x.id == id).SingleOrDefault();
                if (checkContact == null)
                {
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
                    return Json(new Response(ResponseMessages.ContactSave, ResponseMessages.Success), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    checkContact.header = contactModel.header;
                    checkContact.backgroundColor = contactModel.background;
                    checkContact.phone = contactModel.phone;
                    checkContact.email = contactModel.email;
                    checkContact.address = contactModel.address;
                    checkContact.city = contactModel.city;
                    checkContact.country = contactModel.country;
                    checkContact.state = contactModel.state;
                    db.SaveChanges();
                    return Json(new Response(ResponseMessages.ContactUpdate, ResponseMessages.Success), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Json(new Response(ResponseMessages.ContactException, ResponseMessages.Error), JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult SaveBlog(BlogModel blogModel)
        {
            try
            {
                long id = long.Parse(Session["UserInformation"].ToString());
                Blog checkBlog = db.Blogs.Where(x => x.id == id).SingleOrDefault();
                if (checkBlog == null)
                {
                    Blog blog = new Blog();
                    blog.id = id;
                    blog.backgroundColor = blogModel.background;
                    blog.header = blogModel.header;
                    db.Blogs.Add(blog);
                    db.SaveChanges();
                }
                else
                {
                    checkBlog.backgroundColor = blogModel.background;
                    checkBlog.header = blogModel.header;
                    db.SaveChanges();
                }
                foreach (StoryModel storyModel in blogModel.stories)
                {
                    Story checkStories = db.Stories.Where(x => x.blogID == id && (x.body == storyModel.body
                        || x.title == storyModel.title || x.image == storyModel.image)).SingleOrDefault();
                    if (checkStories == null)
                    {
                        Story story = new Story();
                        story.blogID = id;
                        story.body = storyModel.body;
                        story.title = storyModel.title;
                        story.image = storyModel.image;
                        db.Stories.Add(story);
                        db.SaveChanges();
                    }
                    else
                    {
                        checkStories.body = storyModel.body;
                        checkStories.title = storyModel.title;
                        if (storyModel.image != null)
                        {
                            checkStories.image = storyModel.image;
                        }
                        db.SaveChanges();
                    }
                }
                return Json(new Response(ResponseMessages.BlogSaved, ResponseMessages.Success), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Json(new Response(ResponseMessages.BlogException, ResponseMessages.Error), JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult SaveResume(ResumeModel resumeModel)
        {
            try
            {
                long id = long.Parse(Session["UserInformation"].ToString());
                Resume checkResume = db.Resumes.Where(x => x.id == id).SingleOrDefault();
                if (checkResume != null)
                {
                    DeleteResume();
                }
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
                return Json(new Response(ResponseMessages.ResumeSaved, ResponseMessages.Success), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Json(new Response(ResponseMessages.ResumeException, ResponseMessages.Error), JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult HasBlog()
        {
            try
            {
                long id = long.Parse(Session["UserInformation"].ToString());
                Template template = db.Templates.Where(x => x.id == id).SingleOrDefault();
                if (template == null)
                {
                    return Json(new Response(ResponseMessages.True, ResponseMessages.Success), JsonRequestBehavior.AllowGet);
                }
                return Json(new Response(ResponseMessages.ExistingPage, ResponseMessages.Warning), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Json( new Response(ResponseMessages.UnexpectedSystemException, ResponseMessages.Error), JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult DeletePage()
        {
            try
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
                return Json(new Response(ResponseMessages.DeletePage, ResponseMessages.Success), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Json(new Response(ResponseMessages.DeletePageException, ResponseMessages.Error), JsonRequestBehavior.AllowGet);
            }
        }
        private bool DeleteTemplate()
        {
            try
            {
                long id = long.Parse(Session["UserInformation"].ToString());
                Template template = db.Templates.Where(x => x.id == id).SingleOrDefault();
                db.Templates.Remove(template);
                db.SaveChanges();
                return true;
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
                long id = long.Parse(Session["UserInformation"].ToString());
                Main main = db.Mains.Where(x => x.id == id).SingleOrDefault();
                db.Mains.Remove(main);
                db.SaveChanges();
                return true;
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
                long id = long.Parse(Session["UserInformation"].ToString());
                List<NavigationItem> navigationItems = db.NavigationItems.Where(x => x.navID == id).ToList();
                db.NavigationItems.RemoveRange(navigationItems);
                db.SaveChanges();
                Navigation navigation = db.Navigations.Where(x => x.id == id).SingleOrDefault();
                db.Navigations.Remove(navigation);
                db.SaveChanges();
                return true;
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
                long id = long.Parse(Session["UserInformation"].ToString());
                List<HomeSubText> homeSubTexts = db.HomeSubTexts.Where(x => x.homeID == id).ToList();
                db.HomeSubTexts.RemoveRange(homeSubTexts);
                db.SaveChanges();
                Home home = db.Homes.Where(x => x.id == id).SingleOrDefault();
                db.Homes.Remove(home);
                db.SaveChanges();
                return true;
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
                long id = long.Parse(Session["UserInformation"].ToString());
                List<AboutInformation> aboutInformation = db.AboutInformations.Where(x => x.aboutID == id).ToList();
                db.AboutInformations.RemoveRange(aboutInformation);
                db.SaveChanges();
                About about = db.Abouts.Where(x => x.id == id).SingleOrDefault();
                db.Abouts.Remove(about);
                db.SaveChanges();
                return true;
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
                long id = long.Parse(Session["UserInformation"].ToString());
                List<Story> stories = db.Stories.Where(x => x.blogID == id).ToList();
                db.Stories.RemoveRange(stories);
                db.SaveChanges();
                Blog blog = db.Blogs.Where(x => x.id == id).SingleOrDefault();
                db.Blogs.Remove(blog);
                db.SaveChanges();
                return true;
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
                long id = long.Parse(Session["UserInformation"].ToString());
                List<SocialMediaLink> socialMediaLinks = db.SocialMediaLinks.Where(x => x.userID == id).ToList();
                db.SocialMediaLinks.RemoveRange(socialMediaLinks);
                db.SaveChanges();
                return true;
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
                long id = long.Parse(Session["UserInformation"].ToString());
                Contact contact = db.Contacts.Where(x => x.id == id).SingleOrDefault();
                db.Contacts.Remove(contact);
                db.SaveChanges();
                return true;
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
        private PageModel GetPageModel()
        {
            try {
                PageModel pageModel = new PageModel();
                long id = long.Parse(Session["UserInformation"].ToString());
                pageModel.template = db.Templates.Where(x => x.id == id).SingleOrDefault().templateName;
                pageModel.mainComponents = GetMainComponentsModel(id);
                pageModel.navigationModel = GetNavigationModel(id);
                pageModel.home = GetHomeModel(id);
                pageModel.about = GetAboutModel(id);
                pageModel.portfolio = GetPortfolioModel(id);
                pageModel.blog = GetBlogModel(id);
                pageModel.resume = GetResumeModel(id);
                pageModel.contact = GetContactModel(id);
                pageModel.userInformation = RefreshUserInformationModel(id);
                return pageModel;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
        private MainComponentsModel GetMainComponentsModel(long id)
        {
            try
            {
                MainComponentsModel mainComponents = new MainComponentsModel();
                Main main = db.Mains.Where(x => x.id == id).SingleOrDefault();
                if (main != null)
                {
                    mainComponents.hoverColor = main.hoverColor;
                    mainComponents.logo = main.logo;
                    mainComponents.textColor = main.textColor;
                    mainComponents.title = main.title;
                    mainComponents.titleColor = main.titleColor;
                    List<SocialMediaLink> socialMediaLinks = db.SocialMediaLinks.Where(x => x.userID == id).ToList();
                    if (socialMediaLinks != null)
                    {
                        List<SocialMediaModel> socialMediaModels = new List<SocialMediaModel>();
                        foreach (SocialMediaLink socialMediaLink in socialMediaLinks)
                        {
                            SocialMediaModel socialMediaModel = new SocialMediaModel();
                            socialMediaModel.socialMedia = db.SocialMedias.Where(x => x.id == socialMediaLink.socialMedia).SingleOrDefault().code;
                            socialMediaModel.link = socialMediaLink.link;
                            socialMediaModels.Add(socialMediaModel);
                        }
                        mainComponents.socialMediaList = socialMediaModels;
                    }
                }
                return mainComponents;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
        private NavigationModel GetNavigationModel(long id)
        {
            try
            {
                NavigationModel navigationModel = new NavigationModel();
                Navigation navigation = db.Navigations.Where(x => x.id == id).SingleOrDefault();
                if (navigation != null)
                {
                    navigationModel.barColor = navigation.barColor;
                    navigationModel.logo = navigation.logo;
                    List<NavigationItem> navigationItems = db.NavigationItems.Where(x => x.navID == id).ToList();
                    if (navigationItems != null)
                    {
                        List<NavigationItemModel> navigationItemModels = new List<NavigationItemModel>();
                        foreach (NavigationItem navigationItem in navigationItems)
                        {
                            NavigationItemModel navigationItemModel = new NavigationItemModel();
                            navigationItemModel.content = navigationItem.content;
                            navigationItemModel.sectionName = navigationItem.sectionName;
                            navigationItemModel.priority = navigationItem.priority;
                            navigationItemModels.Add(navigationItemModel);
                        }
                        navigationModel.navigationItems = navigationItemModels;
                    }
                }
                return navigationModel;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
        private HomeModel GetHomeModel(long id)
        {
            try
            {
                HomeModel homeModel = new HomeModel();
                Home home = db.Homes.Where(x => x.id == id).SingleOrDefault();
                if (home != null)
                {
                    homeModel.mainText = home.mainText;
                    homeModel.background = home.background;
                    homeModel.textColor = home.textColor;
                    List<HomeSubText> subTextList = db.HomeSubTexts.Where(x => x.homeID == id).ToList();
                    if (subTextList != null)
                    {
                        List<string> subTexts = new List<string>();
                        foreach (HomeSubText subText in subTextList)
                        {
                            subTexts.Add(subText.subText);
                        }
                        homeModel.subTextList = subTexts;
                    }
                }
                return homeModel;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
        private AboutModel GetAboutModel(long id)
        {
            try
            {
                AboutModel aboutModel = new AboutModel();
                About about = db.Abouts.Where(x => x.id == id).SingleOrDefault();
                if (about != null)
                {
                    aboutModel.background = about.background;
                    aboutModel.body = about.body;
                    aboutModel.frame = about.frameColor;
                    aboutModel.header = about.header;
                    aboutModel.image = about.image;
                    aboutModel.subTitle = about.subTitle;
                    List<AboutInformation> aboutInformations = db.AboutInformations.Where(x => x.aboutID == id).ToList();
                    if (aboutInformations != null)
                    {
                        List<List<string>> informations = new List<List<string>>();
                        foreach (AboutInformation info in aboutInformations)
                        {
                            List<string> newInfo = new List<string>();
                            newInfo.Add(info.informationTitle);
                            newInfo.Add(info.informationValue);
                            informations.Add(newInfo);
                        }
                        aboutModel.informationList = informations;
                    }
                }
                return aboutModel;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
        private BlogModel GetBlogModel(long id)
        {
            try
            {
                BlogModel blogModel = new BlogModel();
                Blog blog = db.Blogs.Where(x => x.id == id).SingleOrDefault();
                if (blog != null)
                {
                    blogModel.background = blog.backgroundColor;
                    blogModel.header = blog.header;
                    List<Story> stories = db.Stories.Where(x => x.blogID == id).ToList();
                    if (stories != null)
                    {
                        List<StoryModel> storyModels = new List<StoryModel>();
                        foreach (Story story in stories)
                        {
                            StoryModel storyModel = new StoryModel();
                            storyModel.body = story.body;
                            storyModel.title = story.title;
                            storyModel.image = story.image;
                            storyModels.Add(storyModel);
                        }
                        blogModel.stories = storyModels;
                    }
                }
                return blogModel;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
        private ContactModel GetContactModel(long id)
        {
            try
            {
                ContactModel contactModel = new ContactModel();
                Contact contact = db.Contacts.Where(x => x.id == id).SingleOrDefault();
                if(contact != null)
                {
                    contactModel.background = contact.backgroundColor;
                    contactModel.address = contact.address;
                    contactModel.header = contact.header;
                    contactModel.city = contact.city;
                    contactModel.state = contact.state;
                    contactModel.phone = contact.phone;
                    contactModel.email = contact.email;
                    contactModel.country = contact.country;
                }
                return contactModel;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
        private PortfolioModel GetPortfolioModel(long id)
        {
            try
            {
                PortfolioModel portfolioModel = new PortfolioModel();
                Portfolio portfolio = db.Portfolios.Where(x => x.id == id).SingleOrDefault();
                if (portfolio != null)
                {
                    portfolioModel.background = portfolio.background;
                    portfolioModel.header = portfolio.header;
                    List<PortfolioCategory> portfolioCategories = db.PortfolioCategories.Where(x => x.portfolioID == id).ToList();
                    if (portfolioCategories != null)
                    {
                        List<PortfolioCategoryModel> portfolioCategoryModels = new List<PortfolioCategoryModel>();
                        foreach (PortfolioCategory portfolioCategory in portfolioCategories)
                        {
                            PortfolioCategoryModel portfolioCategoryModel = new PortfolioCategoryModel();
                            portfolioCategoryModel.category = portfolioCategory.category;
                            List<PortfolioCategoryImageRelationship> portfolioCategoryImageRelationships =
                                db.PortfolioCategoryImageRelationships.Where(x => x.portfolioCategoryID == portfolioCategory.id).ToList();
                            if (portfolioCategoryImageRelationships != null)
                            {
                                List<string> images = new List<string>();
                                foreach (PortfolioCategoryImageRelationship portfolioCategoryImage in portfolioCategoryImageRelationships)
                                {
                                    images.Add(portfolioCategoryImage.image);
                                    portfolioCategoryModel.images = images;
                                }
                                portfolioCategoryModels.Add(portfolioCategoryModel);
                                portfolioModel.portfolioCategories = portfolioCategoryModels;
                            }
                        }
                    }
                }
                return portfolioModel;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
        private ResumeModel GetResumeModel(long id)
        {
            try
            {
                ResumeModel resumeModel = new ResumeModel();
                Resume resume = db.Resumes.Where(x => x.id == id).SingleOrDefault();
                if (resume != null)
                {
                    resumeModel.background = resume.background;
                    resumeModel.header = resume.header;
                    List<ResumeSection> resumeSections = db.ResumeSections.Where(x => x.resumeID == id).ToList();
                    if (resumeSections != null)
                    {
                        List<ResumeSectionModel> resumeSectionModels = new List<ResumeSectionModel>();
                        foreach (ResumeSection resumeSection in resumeSections)
                        {
                            ResumeSectionModel resumeSectionModel = new ResumeSectionModel();
                            resumeSectionModel.header = resumeSection.header;
                            List<ResumeSectionItem> resumeSectionsItems =
                                db.ResumeSectionItems.Where(x => x.resumeSectionID == resumeSection.id).ToList();
                            if (resumeSectionsItems != null)
                            {
                                List<ResumeSubSectionModel> resumeSubSectionModels = new List<ResumeSubSectionModel>();
                                foreach (ResumeSectionItem resumeSectionItem in resumeSectionsItems)
                                {
                                    ResumeSubSectionModel resumeSubSectionModel = new ResumeSubSectionModel();
                                    resumeSubSectionModel.date = resumeSectionItem.date;
                                    resumeSubSectionModel.location = resumeSectionItem.location;
                                    resumeSubSectionModel.header = resumeSectionItem.header;
                                    resumeSubSectionModel.explanation = resumeSectionItem.explanation;
                                    List<ResumeSectionItemExplanation> resumeSectionItemExplanations =
                                        db.ResumeSectionItemExplanations.Where(x => x.resumeSectionItemID == resumeSectionItem.id).ToList();
                                    if (resumeSectionItemExplanations != null)
                                    {
                                        List<string> items = new List<string>();
                                        foreach (ResumeSectionItemExplanation resumeSectionItemExplanation in resumeSectionItemExplanations)
                                        {
                                            items.Add(resumeSectionItemExplanation.explanation);
                                            resumeSubSectionModel.explanationItems = items;
                                        }
                                        resumeSubSectionModels.Add(resumeSubSectionModel);
                                        resumeSectionModel.resumeSubSections = resumeSubSectionModels;
                                    }
                                }
                            }

                        }
                    }
                }
                return resumeModel;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
        public JsonResult DeleteSocialMediaLink(SocialMediaModel socialMedia)
        {
            try
            {
                long id = long.Parse(Session["UserInformation"].ToString());
                long mediaID = db.SocialMedias.Where(x => x.code == socialMedia.socialMedia).SingleOrDefault().id;
                SocialMediaLink link = db.SocialMediaLinks.Where(x => x.socialMedia == mediaID && x.link == socialMedia.link && x.userID == id).SingleOrDefault();
                db.SocialMediaLinks.Remove(link);
                db.SaveChanges();
                return Json(new Response(ResponseMessages.SocialMediaDelete, ResponseMessages.Success), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Json(new Response(ResponseMessages.SocialMediaDeleteException, ResponseMessages.Error), JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult DeleteSectionFromNavigation(string content)
        {
            try
            {
                if (content == "home")
                {
                    DeleteHome();
                }
                else if (content == "about")
                {
                    DeleteAbout();
                }
                else if (content == "blog")
                {
                    DeleteBlog();
                }
                else if (content == "contact")
                {
                    DeleteContact();
                }
                else if (content == "resume")
                {
                    DeleteResume();
                }
                else if (content == "portfolio")
                {
                    DeletePortfolio();
                }
                long id = long.Parse(Session["UserInformation"].ToString());
                NavigationItem navigationItem = db.NavigationItems.Where(x => x.navID == id && x.content == content).SingleOrDefault();
                if (navigationItem != null)
                {
                    db.NavigationItems.Remove(navigationItem);
                    db.SaveChanges();
                }
                ChangeNavigationItemPrioroty();
                return Json(new Response(ResponseMessages.SectionDelete, ResponseMessages.Success), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Json(new Response(ResponseMessages.SectionDeleteException, ResponseMessages.Error), JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult DeleteStory(StoryModel storyModel)
        {
            try
            {
                long id = long.Parse(Session["UserInformation"].ToString());
                Story story = db.Stories.Where(x => x.blogID == id && x.title == storyModel.title && x.body == storyModel.body).SingleOrDefault();
                db.Stories.Remove(story);
                db.SaveChanges();
                return Json(new Response(ResponseMessages.StoryDelete, ResponseMessages.Success), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Json(new Response(ResponseMessages.StoryDeleteException, ResponseMessages.Error), JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult DeletePortfolioCategory(PortfolioCategoryModel categoryModel)
        {
            try
            {
                if (categoryModel.category != null)
                {
                    long id = long.Parse(Session["UserInformation"].ToString());
                    PortfolioCategory category = db.PortfolioCategories.Where(x => x.portfolioID == id && x.category == categoryModel.category).SingleOrDefault();
                    List<PortfolioCategoryImageRelationship> relationship = db.PortfolioCategoryImageRelationships.Where(x => x.portfolioCategoryID == category.id).ToList();
                    db.PortfolioCategoryImageRelationships.RemoveRange(relationship);
                    db.PortfolioCategories.Remove(category);
                    db.SaveChanges();
                }
                return Json(new Response(ResponseMessages.CategoryDelete, ResponseMessages.Success), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Json(new Response(ResponseMessages.CategoryDeleteException, ResponseMessages.Error), JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult DeleteSubTextFromHome(string subText)
        {
            try
            {
                if (!string.IsNullOrEmpty(subText))
                {
                    long id = long.Parse(Session["UserInformation"].ToString());
                    HomeSubText homeSubText = db.HomeSubTexts.Where(x => x.homeID == id && x.subText == subText).SingleOrDefault();
                    db.HomeSubTexts.Remove(homeSubText);
                    db.SaveChanges();
                }
                return Json(new Response(ResponseMessages.SubTextDelete, ResponseMessages.Success), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Json(new Response(ResponseMessages.SubTextDeleteException, ResponseMessages.Error), JsonRequestBehavior.AllowGet);
            }
        }
        private bool ChangeNavigationItemPrioroty()
        {
            try
            {
                long id = long.Parse(Session["UserInformation"].ToString());
                List<NavigationItem> navigationItems = db.NavigationItems.Where(x => x.navID == id).ToList();
                if (navigationItems.Count() > 0)
                {
                    int priority = 1;
                    foreach(NavigationItem navigationItem in navigationItems)
                    {
                        if (priority != navigationItem.priority)
                        {
                            navigationItem.priority = priority;
                            db.SaveChanges();
                        }
                        priority++;
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
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
    }
}