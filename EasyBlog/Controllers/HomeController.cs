using EasyBlog.Models;
using EasyBlog.Models.RequestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EasyBlog.Controllers
{
    public class HomeController : Controller
    {
        private EasyBlogEntities db = new EasyBlogEntities();

        public ActionResult Index(PageModel pageModel)
        {
            if(pageModel.mainComponents != null)
            {
                return View(pageModel);
            }
            else if(Session["UserInformation"] != null)
            {
                return View(GetPageModel());
            }
            return RedirectToAction("Login", "User");
        }
        private UserInformationModel RefreshUserInformationModel(long id)
        {
            try
            {
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
        private PageModel GetPageModel()
        {
            try
            {
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
                if (contact != null)
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
    }
}