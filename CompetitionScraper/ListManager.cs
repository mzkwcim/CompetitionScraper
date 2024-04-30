using System.Text;

namespace CompetitionScraper
{
    internal class ListManager
    {
        internal List<CustomStructure> GetList()
        {
            DictionaryManager dictionaryManager = new DictionaryManager();
            Dictionary<string, string> swimmers = dictionaryManager.GetUrls();
            List<CustomStructure> test = new List<CustomStructure>();
            List<string> helperStringList = new List<string>();
            string liveTimingPrefix = "https://www.swimrankings.net/index.php";
            foreach (var swimmer in swimmers)
            {
                Dictionary<string, int> maximumNumberOfStartsOnCompetition = new Dictionary<string, int>();
                Dictionary<string, List<string>> listOfStartsOnCompetition = new Dictionary<string, List<string>>();
                Dictionary<string, Dictionary<string, string>> competitionCityAndDate = new Dictionary<string, Dictionary<string, string>>();
                var rawAge = ScrapingSystem.Loader(swimmer.Value).DocumentNode.SelectSingleNode("//div[@id='athleteinfo']/div[@id='name']").InnerText;
                System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(rawAge, @"\((\d{4})");
                Console.WriteLine(match.Groups[1].Value);
                string category = DataFormatingSystem.Category(Convert.ToInt32(match.Groups[1].Value.Trim()));
                var distances = ScrapingSystem.Loader(swimmer.Value).DocumentNode.SelectNodes("//td[@class='event']//a");
                foreach (var distance in distances)
                {
                    var events = ScrapingSystem.Loader(liveTimingPrefix + distance.GetAttributeValue("href", "").Replace("amp;", "")).DocumentNode.SelectNodes("//a[@class='time']");
                    var dates = ScrapingSystem.Loader(liveTimingPrefix + distance.GetAttributeValue("href", "").Replace("amp;", "")).DocumentNode.SelectNodes("//td[@class='date']");
                    var cities = ScrapingSystem.Loader(liveTimingPrefix + distance.GetAttributeValue("href", "").Replace("amp;", "")).DocumentNode.SelectNodes("//td[@class='city']//a");
                    for (int e = 0; e < events.Count; e++)
                    {
                        if (Convert.ToInt32(dates[e].InnerText.Split("&nbsp;")[2]) == 2024)
                        {
                            var swimmingDistance = ScrapingSystem.Loader((liveTimingPrefix + events[e].GetAttributeValue("href", "")).Replace("amp;", "")).DocumentNode.SelectNodes("//td")[13].InnerText;
                            if (!swimmingDistance.Contains("Lap"))
                            {
                                var competitionName = ScrapingSystem.Loader((liveTimingPrefix + events[e].GetAttributeValue("href", "")).Replace("amp;", "")).DocumentNode.SelectNodes("//td[@class='h4']//a")[1];
                                var competitionDate = ScrapingSystem.Loader((liveTimingPrefix + competitionName.GetAttributeValue("href", "")).Replace("amp;", "")).DocumentNode.SelectNodes("//td[@class='titleRight']")[1].InnerText;
                                var place = ScrapingSystem.Loader((liveTimingPrefix + events[e].GetAttributeValue("href", "")).Replace("amp;", "")).DocumentNode.SelectNodes("//td")[15].InnerText;
                                StringBuilder placeTaken = new StringBuilder();
                                foreach (char c in place)
                                {
                                    placeTaken.Append(char.IsDigit(c) ? c : "");
                                }
                                string line = $"{DataFormatingSystem.ToTitleString(swimmer.Key)} {placeTaken} miejsce na {DataFormatingSystem.TranslateStroke(swimmingDistance)} " +
                                              $"{competitionName.InnerText} dnia {DataFormatingSystem.DateTranslation(dates[e].InnerText)}";
                                if (Convert.ToInt32(placeTaken.ToString()) != 450 && !helperStringList.Contains(line))
                                {
                                    if (!competitionCityAndDate.ContainsKey(competitionName.InnerText))
                                    {
                                        competitionCityAndDate.Add(competitionName.InnerText, new Dictionary<string, string> { { DataFormatingSystem.DateTranslation(competitionDate), cities[e].InnerText } });
                                    }
                                    if (listOfStartsOnCompetition.ContainsKey(competitionName.InnerText))
                                    {
                                        listOfStartsOnCompetition[competitionName.InnerText].Add($"{placeTaken} miejsce na {DataFormatingSystem.TranslateStroke(swimmingDistance)}");
                                    }
                                    else
                                    {
                                        listOfStartsOnCompetition.Add(competitionName.InnerText, new List<string> { $"{placeTaken} miejsce na {DataFormatingSystem.TranslateStroke(swimmingDistance)}" });
                                    }
                                    helperStringList.Add(line);
                                    Console.WriteLine(line);
                                    if (!maximumNumberOfStartsOnCompetition.ContainsKey(competitionName.InnerText))
                                    {
                                        maximumNumberOfStartsOnCompetition.Add(competitionName.InnerText, 1);
                                    }
                                    else
                                    {
                                        maximumNumberOfStartsOnCompetition[competitionName.InnerText]++;
                                    }
                                }
                            }
                        }
                    }
                }
                if (listOfStartsOnCompetition.Count > 0)
                {
                    CustomStructure cs = new CustomStructure(DataFormatingSystem.ToTitleString(swimmer.Key), listOfStartsOnCompetition, maximumNumberOfStartsOnCompetition.Values.Max(), competitionCityAndDate, category);
                    test.Add(cs);
                }
            }
            return test;
        }
    }
}
