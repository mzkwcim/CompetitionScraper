﻿using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;

namespace CompetitionScraper
{
    internal class ListManager
    {
        internal List<CustomStructure> GetList(string clubUrl)
        {
            AtleteListGeneratingSystem dictionaryManager = new AtleteListGeneratingSystem();
            Dictionary<string, string> swimmers = AtleteListGeneratingSystem.GetUrls(clubUrl).GetAwaiter().GetResult();
            List<CustomStructure> test = new List<CustomStructure>();
            List<string> helperStringList = new List<string>();
            string liveTimingPrefix = "https://www.swimrankings.net/index.php";
            Parallel.ForEach(swimmers, swimmer =>
            {
                Dictionary<string, int> maximumNumberOfStartsOnCompetition = new Dictionary<string, int>();
                Dictionary<string, List<string>> listOfStartsOnCompetition = new Dictionary<string, List<string>>();
                Dictionary<string, Dictionary<string, string>> competitionCityAndDate = new Dictionary<string, Dictionary<string, string>>();
                var baseNode = ScrapingSystem.Loader(swimmer.Value);
                var rawAge = baseNode.DocumentNode.SelectSingleNode("//div[@id='athleteinfo']/div[@id='name']").InnerText;
                System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(rawAge, @"\((\d{4})");
                Console.WriteLine(match.Groups[1].Value);
                string category = DataFormatingSystem.Category(Convert.ToInt32(match.Groups[1].Value.Trim()));
                var distances = baseNode.DocumentNode.SelectNodes("//td[@class='event']//a");
                foreach (var distance in distances)
                {
                    var livetimingNode = ScrapingSystem.Loader(liveTimingPrefix + distance.GetAttributeValue("href", "").Replace("amp;", ""));
                    var events = livetimingNode.DocumentNode.SelectNodes("//a[@class='time']");
                    var dates = livetimingNode.DocumentNode.SelectNodes("//td[@class='date']");
                    var cities = livetimingNode.DocumentNode.SelectNodes("//td[@class='city']//a");
                    for (int e = 0; e < events.Count; e++)
                    {
                        if (Convert.ToInt32(dates[e].InnerText.Split("&nbsp;")[2]) == 2024)
                        {
                            var eventNode = ScrapingSystem.Loader((liveTimingPrefix + events[e].GetAttributeValue("href", "")).Replace("amp;", ""));
                            var swimmingDistance = eventNode.DocumentNode.SelectNodes("//td")[13].InnerText;
                            if (!swimmingDistance.Contains("Lap"))
                            {
                                var competitionName = eventNode.DocumentNode.SelectNodes("//td[@class='h4']//a")[1];
                                string competitionNameString = competitionName.InnerText;
                                var competitionDate = ScrapingSystem.Loader((liveTimingPrefix + competitionName.GetAttributeValue("href", "")).Replace("amp;", "")).DocumentNode.SelectNodes("//td[@class='titleRight']")[1].InnerText;
                                var place = eventNode.DocumentNode.SelectNodes("//td")[15].InnerText;
                                StringBuilder placeTaken = new StringBuilder();
                                foreach (char c in place)
                                {
                                    placeTaken.Append(char.IsDigit(c) ? c : "");
                                }
                                string translatedStroke = DataFormatingSystem.TranslateStroke(swimmingDistance);
                                string translatedDate = DataFormatingSystem.DateTranslation(competitionDate);
                                string line = $"{swimmer.Key} {placeTaken} miejsce na {translatedStroke} " +
                                              $"{competitionNameString} dnia {translatedDate}";
                                if (Convert.ToString(placeTaken) != "" && Convert.ToInt32(placeTaken.ToString()) != 450 && Convert.ToInt32(placeTaken.ToString()) != 800 && !helperStringList.Contains(line))
                                {
                                    if (!competitionCityAndDate.ContainsKey(competitionNameString))
                                    {
                                        competitionCityAndDate.Add(competitionNameString, new Dictionary<string, string> { { translatedDate, cities[e].InnerText } });
                                    }
                                    if (listOfStartsOnCompetition.ContainsKey(competitionNameString))
                                    {
                                        listOfStartsOnCompetition[competitionNameString].Add($"{placeTaken} miejsce na {translatedStroke}");
                                    }
                                    else
                                    {
                                        listOfStartsOnCompetition.Add(competitionNameString, new List<string> { $"{placeTaken} miejsce na {translatedStroke}" });
                                    }
                                    helperStringList.Add(line);
                                    Console.WriteLine(line);
                                    if (!maximumNumberOfStartsOnCompetition.ContainsKey(competitionNameString))
                                    {
                                        maximumNumberOfStartsOnCompetition.Add(competitionNameString, 1);
                                    }
                                    else
                                    {
                                        maximumNumberOfStartsOnCompetition[competitionNameString]++;
                                    }
                                }
                            }
                        }
                    }
                }
                lock (test)
                {
                    if (listOfStartsOnCompetition.Count > 0)
                    {
                        CustomStructure cs = new CustomStructure(swimmer.Key, listOfStartsOnCompetition, maximumNumberOfStartsOnCompetition.Values.Max(), competitionCityAndDate, category);
                        test.Add(cs);
                    }
                }
            });
            test = test.AsParallel().OrderBy(x => x.AthleteName).ToList();

            return test;
        }
    }
}
