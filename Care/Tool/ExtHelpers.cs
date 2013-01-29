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
            // 新浪的祼格式是这样的
            // Fri Oct 05 11:38:16 +0800 2012
            string result;
            string[] strTemps = sinaFormat.Split(' ');
            strTemps[4] = strTemps[4].Substring(0, 3) + ":" + strTemps[4].Substring(3, 2);
            result = String.Join(" ", strTemps);

            DateTime dt = DateTime.ParseExact(result, "ddd MMM dd HH:mm:ff zzz yyyy", cInfo);
            DateTimeOffset off = new DateTimeOffset(dt);
            result = dt.ToString("yy/MM/dd");
            return off;
        }

        public static DateTimeOffset GetRenrenTimeFullObject(string renrenFormat)
        {
            // 人人的裸格式是这样的
            // 2012-10-03 11:25:26
            renrenFormat += " +08:00";
            DateTime dt = DateTime.ParseExact(renrenFormat, "yyyy-MM-dd HH:mm:ff zzz", cInfo);
            DateTimeOffset off = new DateTimeOffset(dt);
            return off;
        }

        public static DateTimeOffset GetDoubanTimeFullObject(string doubanFormat)
        {
            // 豆瓣的裸格式是这样的
            // 2012-10-03 11:25:26
            doubanFormat += " +08:00";
            DateTime dt = DateTime.ParseExact(doubanFormat, "yyyy-MM-dd HH:mm:ff zzz", cInfo);
            DateTimeOffset off = new DateTimeOffset(dt);
            return off;
            // 嗯嗯，与上面那位老兄长得一模一样似乎也没关系的样子呢 ^_^
        }


        public static string TimeObjectToString(DateTimeOffset offset)
        {
            return offset.LocalDateTime.ToString("yy-MM-dd HH:mm:ff");                
        }
    }
}
