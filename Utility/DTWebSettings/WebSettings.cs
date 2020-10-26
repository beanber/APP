using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Beanber.Utility.DTWebSettings
{
    //Lấy đường đẫn website
    public class ApplicationURL
    {
        public static string Root
        {
            get
            {
                string sPath = HttpContext.Current.Request.ApplicationPath;
                return sPath.EndsWith("/") ? sPath : sPath + "/";
            }
        }
        public static string ToFloder(string sPathFloder)
        {
            string sFloderPath = Root + sPathFloder;
            return sFloderPath.EndsWith("/") ? sFloderPath : sFloderPath + "/";
        }
    }   
}