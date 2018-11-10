using System;
using System.Threading.Tasks;
using WordPressPCL;
using WordPressPCL.Models;

namespace WpMigrator
{
    public class Program
    {
        private static readonly ContentMigrator Migrator =
            new ContentMigrator(new WordPressClient(MigratorConfig.WordPressBaseUrl) {AuthMethod = AuthMethod.JWT});

        public static async Task Main(string[] args)
        {
            try
            {
                await Migrator.MigrateContent();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
