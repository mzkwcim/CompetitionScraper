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

            Dictionary<string, int> maxValue = new Dictionary<string, int>();

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
                                    result.Append(char.IsDigit(c) ? c : "");
                                }
                                string line = $"{DataFormatingSystem.ToTitleString(pair.Key)} {result} miejsce na {DataFormatingSystem.TranslateStroke(placeholder)} {singlEvent.InnerText} dnia {DataFormatingSystem.DateTranslation(dates[e].InnerText)}";
                                if (Convert.ToInt32(result.ToString()) != 450 && !allResults.Contains(line))
                                {
                                    allResults.Add($"{result} miejsce na {DataFormatingSystem.TranslateStroke(placeholder)}");
                                    if (!maxValue.ContainsKey(singlEvent.InnerText))
                                    {
                                        maxValue.Add(singlEvent.InnerText, 1);
                                    }
                                    else
                                    {
                                        maxValue[singlEvent.InnerText]++;
                                    }
                                }
                            }
                        }
                    }
                }
                CustomStructure cs = new CustomStructure(DataFormatingSystem.ToTitleString(pair.Key), allResults,maxValue.Values.Max());
                Console.WriteLine(cs.Text);
                Console.WriteLine(cs.Number);
                foreach(var s in cs.StringList)
                {
                    Console.WriteLine(s);
                }
                Console.ReadKey();
            }
            return allResults;
        }
    }
    public class CustomStructure
    {
        public string Text { get; set; }
        public List<string> StringList { get; set; }
        public int Number { get; set; }

        public CustomStructure(string text, List<string> stringList, int number)
        {
            Text = text;
            StringList = stringList;
            Number = number;
        }
    }

}
