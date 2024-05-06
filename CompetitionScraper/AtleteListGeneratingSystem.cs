using HtmlAgilityPack;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace CompetitionScraper
{
    internal class AtleteListGeneratingSystem
    {
        public static string urlPrefix = "https://www.swimrankings.net/index.php";

        public static async Task<Dictionary<string, string>> GetUrls(string link)
        {
            List<string> genders = new List<string> { "gender=1", "gender=2" };
            List<string> poolLengths = new List<string> { "course=LCM", "course=SCM" };

            var athletes = new ConcurrentDictionary<string, string>();

            List<Task> tasks = new List<Task>();
            Parallel.ForEach(genders, gender =>
            {
                Parallel.ForEach(poolLengths, pool =>
                {
                    string modifiedLink = link.Replace("course=LCM", pool).Replace("gender=1", gender);
                    var distances = ScrapingSystem.Loader(modifiedLink).DocumentNode.SelectNodes("//td[@class='swimstyle']//a");
                    Parallel.ForEach(distances, distance =>
                    {
                        string href = distance.GetAttributeValue("href", "").Replace("amp;", "");
                        var urlToProcess = $"{AtleteListGeneratingSystem.urlPrefix}{href}";
                        tasks.Add(Task.Run(() => ProcessUrl(urlToProcess, athletes)));
                    });
                });
            });

            await Task.WhenAll(tasks);
            Dictionary<string, string> swimmers = new Dictionary<string, string>(athletes);

            return swimmers;
        }

        public static async Task ProcessUrl(string url, ConcurrentDictionary<string, string> athletes)
        {
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var page = htmlDocument.DocumentNode.SelectNodes("//td[@class='navigation']")?[9]?.InnerText?
                .Split(" ");

            if (page != null && page.Length > 0 && double.TryParse(page[page.Length - 1], out double lastPage))
            {
                int number = (int)Math.Ceiling(lastPage / 25);

                for (int i = 0; i < number; i++)
                {
                    var modifiedHref = url.Replace("firstPlace=1", $"firstPlace={(25 * i) + 1}");
                    var fullname = htmlDocument.DocumentNode.SelectNodes("//td[@class='fullname']//a");

                    if (fullname != null)
                    {
                        Parallel.ForEach(fullname, name =>
                        {
                            string athlete = name.InnerText;

                            athletes.AddOrUpdate(DataFormatingSystem.ToTitleString(athlete), AtleteListGeneratingSystem.urlPrefix + name.GetAttributeValue("href", "").Replace("amp;",""), (key, oldValue) => oldValue);
                        });
                    }
                }
            }
        }
    }
}
