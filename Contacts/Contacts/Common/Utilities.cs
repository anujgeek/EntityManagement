using Contacts.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Contacts.Common
{
    public static class Utilities
    {
        public const string JSON_EMPTY_OBJECT = "{}";
        public const string JSON_EMPTY_ARRAY = "[]";
        public const string SUCCESS = "Success";
        public const string FAILURE = "Failure";
        public const string EXCEPTION = "Exception";

        public static string GetConnectionStringForDatabase()
        {
            return @"CONNECTION_STRING";
        }

        public static int SortString(string s1, string s2, string sortDirection)
        {
            if (s1 == null && s2 == null)
            {
                return 0;
            }
            else if (s1 != null && s2 != null)
            {
                return sortDirection == "asc" ? s1.CompareTo(s2) : s2.CompareTo(s1);
            }
            else if (s1 == null)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }

        public static int SortInteger(int i1, int i2, string sortDirection)
        {
            return sortDirection == "asc" ? i1.CompareTo(i2) : i2.CompareTo(i1);
        }

        public static int SortDateTime(DateTime? d1, DateTime? d2, string sortDirection)
        {
            if (d1.HasValue && d2.HasValue)
                return sortDirection == "asc" ? d1.Value.CompareTo(d2) : d2.Value.CompareTo(d1);
            else if (d1.HasValue == false && d2.HasValue == false)
                return 0;
            else if (d1.HasValue)
                return 1;
            else
                return -1;
        }

        public static int SortDateTimeInString(string s1, string s2, string sortDirection)
        {
            DateTime d1 = DateTime.Parse(s1);
            DateTime d2 = DateTime.Parse(s2);
            return sortDirection == "asc" ? d1.CompareTo(d2) : d2.CompareTo(d1);
        }

        public static bool Contains(this string source, string destination, StringComparison comparison)
        {
            return source.IndexOf(destination, comparison) >= 0;
        }

        static Random randomGenerateRandomDOBForAge = new Random();
        public static DateTime GenerateRandomDOBForAge(int startAge, int endAge)
        {
            int currentYear = 2015;

            DateTime startDate = new DateTime(currentYear - startAge, 1, 31);
            DateTime endDate = new DateTime(currentYear - endAge, 12, 31);
            int dayDifference = Math.Abs(((TimeSpan)(endDate - startDate)).Days);
            int range = (dayDifference == 0) ? 1 : dayDifference;
            return startDate.AddDays(randomGenerateRandomDOBForAge.Next(range));
        }
    }
}