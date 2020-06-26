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
        public static string UnexpectedSystemError = "Unexpected system error! Please try again.";
        public static string DatabaseError = "SQL Server Error!";





    }
}