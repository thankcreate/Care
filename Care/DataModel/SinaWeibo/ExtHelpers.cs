using System;
using System.Windows;
using System.Globalization;
using System.Windows.Media;
using System.IO.IsolatedStorage;
using System.IO;

namespace Care
{
    public class ExtHelpers
    {
        static CultureInfo cInfo = new CultureInfo("en-US");

        public static string GetTime(string sinaFormat)
        {
            string result;
            string[] strTemps = sinaFormat.Split(' ');
            strTemps[4] = strTemps[4].Substring(0, 3) + ":" + strTemps[4].Substring(3, 2);
            result = String.Join(" ", strTemps);

            DateTime dt = DateTime.ParseExact(result, "ddd MMM dd HH:mm:ff zzz yyyy", cInfo);
            result = dt.ToString("MM/dd, HH:mm");
            return result;
        }

        public static string GetSinaTimeFullString(string sinaFormat)
        {
            string result;
            string[] strTemps = sinaFormat.Split(' ');
            strTemps[4] = strTemps[4].Substring(0, 3) + ":" + strTemps[4].Substring(3, 2);
            result = String.Join(" ", strTemps);

            DateTime dt = DateTime.ParseExact(result, "ddd MMM dd HH:mm:ff zzz yyyy", cInfo);
            DateTimeOffset of = new DateTimeOffset(dt);
            result = dt.ToString("yy/MM/dd");
            return result;
        }

        public static DateTimeOffset GetSinaTimeFullObject(string sinaFormat)
        {
            string result;
            string[] strTemps = sinaFormat.Split(' ');
            strTemps[4] = strTemps[4].Substring(0, 3) + ":" + strTemps[4].Substring(3, 2);
            result = String.Join(" ", strTemps);

            DateTime dt = DateTime.ParseExact(result, "ddd MMM dd HH:mm:ff zzz yyyy", cInfo);
            DateTimeOffset off = new DateTimeOffset(dt);
            result = dt.ToString("yy/MM/dd");
            return off;
        }

        public static string TimeObjectToString(DateTimeOffset offset)
        {
            return offset.LocalDateTime.ToString("yy/MM/dd HH:mm:ff");                
        }
    }
}
