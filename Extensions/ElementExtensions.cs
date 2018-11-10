using System;
using System.Linq;
using HtmlAgilityPack;

namespace WpMigrator.Extensions
{
    public static class ElementExtensions
    {
        public static void TranFormLinks(this HtmlNode innerContent, string absPath)
        {
            var links = innerContent.SelectNodes("//a");
            if (links == null) return;

            foreach (var link in links)
            {
                var hrefAtt = link.Attributes["href"];
                var hrefVal = hrefAtt?.Value;

                if (hrefVal != null && (!hrefVal.Contains("http") || !hrefVal.Contains("www") && hrefVal.EndsWith(".htm")))
                {
                    link.SetAttributeValue("href", TranformPageName(hrefVal).ToLower());
                    continue;
                }

                if (hrefVal == null || !hrefVal.Contains(absPath) || !hrefVal.EndsWith(".htm")) continue;
                var url = new Uri(hrefVal);

                if (url.Segments.Length > 1)
                    link.SetAttributeValue("href", BuildPageName(hrefVal, true));
            }
        }

        public static string TranformPageName(this string pageName)
        {
            pageName = pageName.Replace(".html", string.Empty);
            pageName = pageName.Replace(".htm", string.Empty);
            pageName = pageName.Replace("%20", string.Empty);

            return pageName;
        }

        public static void SanitizeTags(this HtmlNode innerContent)
        {
            var tags = innerContent.SelectNodes("//*");
            if (tags == null) return;

            foreach (var tag in tags)
            {
                tag.Attributes.Remove("style");
                tag.Attributes.Remove("bgcolor");
                tag.RemoveNodesButKeepChildren("//font");
                tag.RemoveNodesButKeepChildren("//body");
            }
        }

        public static string BuildPageName(string path, bool isHrefTransform = false)
        {
            var segmentsToSkip = 6;
            if (isHrefTransform) segmentsToSkip = 1;

            var pageName = string.Join("-", new Uri(path).Segments.Skip(segmentsToSkip)).ToLower();
            pageName = pageName.TranformPageName();
            pageName = pageName.Replace("/", string.Empty);

            return pageName;
        }
    }
}
