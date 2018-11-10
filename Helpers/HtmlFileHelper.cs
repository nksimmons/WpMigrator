using System.Collections.Generic;
using System.IO;
using System.Linq;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;

namespace WpMigrator.Helpers
{
    public class HtmlFileHelper
    {
        public static IList<string> GetHtmlFiles(DirectoryInfo mainDir)
        {
            var files = Directory.GetFiles(mainDir.FullName, "*.*", SearchOption.AllDirectories).ToList();

            var htmlFiles = files.Where(x => x.EndsWith(".html")).ToList();

            htmlFiles.AddRange(files.Where(x => x.EndsWith(".htm")).ToList());

            return htmlFiles.ToList();
        }

        public static HtmlNode GetMainContent(string fileName, HtmlDocument doc, out string title)
        {
            doc.Load(fileName);

            var document = doc.DocumentNode;

            title = document.QuerySelectorAll("h1")?.FirstOrDefault()?.InnerText;

            var content = document.QuerySelectorAll(MigratorConfig.CssSelectorToTargetMainContent);

            if (!content.Any())
            {
                content = document.QuerySelectorAll("body");
            }

            return content?.FirstOrDefault();
        }
    }
}
