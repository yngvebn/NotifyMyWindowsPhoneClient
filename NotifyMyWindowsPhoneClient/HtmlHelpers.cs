using System.Xml.Linq;

namespace NotifyMyWindowsPhoneClient
{
    public static class HtmlHelpers
    {
        public static string HtmlEncode(this string s)
        {
            return System.Web.HttpUtility.HtmlEncode(s);
        }

        public static int SafeInt(this XElement x)
        {
            int i = 0;
            if (x == null) return i;
            int.TryParse(x.Value, out i);

            return i;
        }
        public static int SafeInt(this XAttribute x)
        {
            int i = 0;
            if (x == null) return i;
            int.TryParse(x.Value, out i);

            return i;
        }
    }
}