﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC4._5._1.Utility
{
    class Helper
    {

       
        // return a date that is n days in future
        public static String daysInFuture(int n)
        {
            DateTime now = DateTime.Now, future;
            future = now.AddDays(n);
            return string.Format("{0:yyyy-MM-dd}", future);
        }

        public static string ReturnPassword()
        {
            
            return CommonUtility.GetConfigValue(ProjectConstants.PASSWORD);
        }

        public static string RetrunUsername()
        {
            return CommonUtility.GetConfigValue(ProjectConstants.USERNAME);
        }

        public static Dictionary<string, string> ReturnHttpHeader()
        {
            var httpHeaders = new Dictionary<string, string>();
            httpHeaders.Add("Username", Helper.RetrunUsername());
            httpHeaders.Add("Password", Helper.ReturnPassword());

            return httpHeaders;
        }
    }
}
