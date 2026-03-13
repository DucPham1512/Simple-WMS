using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace InternProj.Services
{
    internal class DateStringConverter
    {
        static public string toString(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd");
        }

        static public DateTime toDateTime(string dateTimeString)
            {
                return DateTime.ParseExact(dateTimeString, "yyyy-MM-dd", null);
        }
    }
}
