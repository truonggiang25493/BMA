using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace BMA.Common
{
    public class CommonFunction
    {
        public static string ChangeDateTimeToVietnameseType(DateTime dateTime)
        {
            string rs = "";
            // Get date time string
            string dateTimeString = dateTime.ToString("dd/MM/yyyy hh:mm tt");
            // If the 2-letter last is AM, change to Sáng
            // If the 2-letter last is PM, change to Chiều
            int length = dateTimeString.Length;
            string twoLastLetter = dateTimeString.Substring(length - 2);
            if (twoLastLetter.Equals("AM"))
            {
                rs = dateTimeString.Substring(0, length - 2) + "Sáng";
            }
            else
            {
                rs = dateTimeString.Substring(0, length - 2) + "Chiều";
            }
            return rs;
        }
    }
}