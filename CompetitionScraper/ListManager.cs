using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompetitionScraper
{
    internal class ListManager
    {
        internal List<string> GetList()
        {
            DictionaryManager dictionaryManager = new DictionaryManager();
            Dictionary<string, string> swimmers = dictionaryManager.GetUrls();


            List<string> allResults = new List<string>();
            foreach (var pair in swimmers)
            {
                var distances = ScrapingSystem.Loader(pair.Value).DocumentNode.SelectNodes("//td[@class='event']//a");
                foreach (var distance in distances)
                {
                    var events = ScrapingSystem.Loader("https://www.swimrankings.net/index.php" + distance.GetAttributeValue("href", "").Replace("amp;", "")).DocumentNode.SelectNodes("//a[@class='time']");
                    var dates = ScrapingSystem.Loader("https://www.swimrankings.net/index.php" + distance.GetAttributeValue("href", "").Replace("amp;", "")).DocumentNode.SelectNodes("//td[@class='date']");
                    for (int e = 0; e < events.Count; e++)
                    {
                        if (Convert.ToInt32(dates[e].InnerText.Split("&nbsp;")[2]) == 2024)
                        {
                            var placeholder = ScrapingSystem.Loader(("https://www.swimrankings.net/index.php" + events[e].GetAttributeValue("href", "")).Replace("amp;", "")).DocumentNode.SelectNodes("//td")[13].InnerText;
                            if (!placeholder.Contains("Lap"))
                            {
                                var singlEvent = ScrapingSystem.Loader(("https://www.swimrankings.net/index.php" + events[e].GetAttributeValue("href", "")).Replace("amp;", "")).DocumentNode.SelectNodes("//td[@class='h4']//a")[1];
                                var place = ScrapingSystem.Loader(("https://www.swimrankings.net/index.php" + events[e].GetAttributeValue("href", "")).Replace("amp;", "")).DocumentNode.SelectNodes("//td")[15].InnerText;
                                StringBuilder result = new StringBuilder();
                                foreach (char c in place)
                                {
                                    if (char.IsDigit(c))
                                    {
                                        result.Append(c);
                                    }
                                }
                                string line = $"{DataFormatingSystem.ToTitleString(pair.Key)} {result} miejsce na {DataFormatingSystem.TranslateStroke(placeholder)} {singlEvent.InnerText} dnia {DataFormatingSystem.DateTranslation(dates[e].InnerText)}";
                                if (Convert.ToInt32(result.ToString()) != 450 && !allResults.Contains(line))
                                {
                                    allResults.Add(line);
                                }
                            }
                        }
                    }
                }
            }
            return allResults;
        }
    }
}
