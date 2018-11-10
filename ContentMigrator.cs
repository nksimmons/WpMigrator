using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using WordPressPCL;
using WpMigrator.Extensions;
using WpMigrator.Helpers;

namespace WpMigrator
{
    public class ContentMigrator
    {
        private readonly WordPressClient _wordPressClient;

        public ContentMigrator(WordPressClient wpRestClient) => _wordPressClient = wpRestClient;

        public async Task MigrateContent()
        {
            await _wordPressClient.RequestJWToken(MigratorConfig.WordPressUsername, MigratorConfig.WordPressPassword);

            var isValidToken = await _wordPressClient.IsValidJWToken();
            if (!isValidToken) { throw new Exception("Token is not valid."); }

            Console.WriteLine("Clear existing pages? y / n");
            if (Console.ReadLine()?.ToLower() == "y") await WpContentHelper.ClearExistingPages(_wordPressClient, MigratorConfig.NumThreads);

            var htmlFiles = HtmlFileHelper.GetHtmlFiles(MigratorConfig.MainDir);
            Console.WriteLine($"Number of HTML files: {htmlFiles.Count}");

            using (var semaphore = new SemaphoreSlim(MigratorConfig.NumThreads))
            {
                foreach (var htmlFile in htmlFiles.Select((value, index) => new { index, value }))
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        var innerContent = HtmlFileHelper.GetMainContent(htmlFile.value, new HtmlDocument(), out var title);
                        if (innerContent == null) continue;

                        Console.WriteLine($"File number: {htmlFile.index}. Path: {htmlFile.value}");

                        innerContent.TranFormLinks(MigratorConfig.AbsHrefPath);
                        innerContent.SanitizeTags();

                        try
                        {
                            await _wordPressClient.Pages.Create(WpContentHelper.BuildPage(innerContent, title, htmlFile.value));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Exception: {e}");
                        }
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }
            }

            Console.ReadLine();
        }
    }
}
