using System;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using WordPressPCL;
using WordPressPCL.Models;
using WpMigrator.Extensions;

namespace WpMigrator.Helpers
{
    public class WpContentHelper
    {
        public static Page BuildPage(HtmlNode innerContent, string title, string htmlFile)
        {
            return new Page
            {
                Author = 1,
                Content = new Content { Raw = innerContent.InnerHtml },
                Date = DateTime.Now,
                CommentStatus = OpenStatus.Open,
                DateGmt = DateTime.Now,
                Status = Status.Publish,
                Title = new Title(title ?? ElementExtensions.BuildPageName(htmlFile))
            };
        }

        public static async Task ClearExistingPages(WordPressClient wpRestClient, int numThreads)
        {
            var pages = await wpRestClient.Pages.GetAll(useAuth: true);

            using (var semaphore = new SemaphoreSlim(numThreads))
            {
                foreach (var page in pages)
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        Console.WriteLine($"Deleting post id: {page.Id}");
                        await wpRestClient.Pages.Delete(page.Id);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }
            }

            Console.WriteLine("Post deletion completed.");
        }
    }
}
