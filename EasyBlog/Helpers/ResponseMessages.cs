using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyBlog.Helpers
{
    public static class ResponseMessages
    {
        public static string Success = "Success";
        public static string RequiredFields = "Fill the required fields!";
        public static string InvalidEmailAddress = "Invalid email address!";
        public static string LoginException = "Invalid email address or password. Please try again.";
        public static string InvalidUser = "Invalid user!";
        public static string DifferentConfirmationPassword = "Password and Confirm Password are different!";
        public static string InvalidPhoneNumber = "Invalid phone number!";
        public static string MissingCountryCode = "Please add country code with + symbol!";
        public static string ExistingPhoneNumber = "The phone number already used by another user!";
        public static string ExistingEmailAddress = "Email address already used by another user!";
        public static string UnexpectedSystemException = "Unexpected system error! Please try again.";
        public static string DatabaseException = "SQL Server Error!";
        public static string EmailAddressUpdated = "Your email address is updated successfully!";
        public static string PhoneNumberUpdated = "Your phone number is updated successfully!";
        public static string PasswordReset = "Your password is changed successfully!";
        public static string WrongPassword = "Your password is wrong!";
        public static string FileExtensionTypeException = "You can only upload jpeg, jpg or png type files!";
        public static string UploadImageFiles = "Image files uploaded successfully!";
        public static string UploadImageFilesException = "Image files couldn't upload! Please try again.";
        public static string TemplateSave = "Template saved successfully!";
        public static string TemplateUpdate = "Template updated successfully!";
        public static string TemplateException = "An error occurred while saving the template!";
        public static string MainComponentsSave = "Main Components saved successfully!";
        public static string MainComponentsUpdate = "Main Components updated successfully!";
        public static string MainComponentsException = "An error occurred while saving the main components!";
        public static string SocialMediaException = "An error occurred while saving the social medias!";
        public static string NavigationSave = "Navigation saved successfully!";
        public static string NavigationUpdate = "Navigation updated successfully!";
        public static string NavigationException = "An error occurred while saving the navigation!";
        public static string HomeSave = "Home section saved successfully!";
        public static string HomeUpdate = "Home section updated successfully!";
        public static string HomeException = "An error occurred while saving the home section!";
        public static string AboutSave = "About section saved successfully!";
        public static string AboutUpdate = "About section updated successfully!";
        public static string AboutException = "An error occurred while saving the about section!";
        public static string PortfolioSaved = "Portfolio section saved successfully!";
        public static string PortfolioException = "An error occurred while saving the portfolio section!";
        public static string ContactSave = "Contact section saved successfully!";
        public static string ContactUpdate = "Contact section updated successfully!";
        public static string ContactException = "An error occurred while saving the contact section!";
        public static string BlogSaved = "Blog section saved successfully!";
        public static string BlogException = "An error occurred while saving the blog section!";
        public static string ResumeSaved = "Resume section saved successfully!";
        public static string ResumeException = "An error occurred while saving the resume section!";
        public static string True = "true";
        public static string False = "false";
        public static string SocialMediaDelete = "Social media deleted successfully!";
        public static string SocialMediaDeleteException = "An error occurred while deleting the social media!";
        public static string SectionDelete = "Section deleted successfully!";
        public static string SectionDeleteException = "An error occurred while deleting the section!";
        public static string StoryDelete = "Story deleted successfully!";
        public static string StoryDeleteException = "An error occurred while deleting the story!";
        public static string CategoryDelete = "Category deleted successfully!";
        public static string CategoryDeleteException = "An error occurred while deleting the category!";
        public static string SubTextDelete = "Sub text deleted successfully!";
        public static string SubTextDeleteException = "An error occurred while deleting the sub text!";



    }
}