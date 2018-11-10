using System.Configuration;
using System.IO;

namespace WpMigrator
{
    public static class MigratorConfig
    {
        public static readonly string WordPressUsername = ConfigurationManager.AppSettings["WordPressUsername"];
        public static readonly string WordPressPassword = ConfigurationManager.AppSettings["WordPressPassword"];
        public static readonly string AbsHrefPath = ConfigurationManager.AppSettings["DomainOfAbsoluteHrefPathToTransform"];

        public static readonly string RootPath = ConfigurationManager.AppSettings["StaticFilesPath"];
        public static readonly string WordPressBaseUrl = ConfigurationManager.AppSettings["WordPressBaseUrl"];

        public const int NumThreads = 5;

        public static readonly DirectoryInfo MainDir = new DirectoryInfo(RootPath);
    }
}
