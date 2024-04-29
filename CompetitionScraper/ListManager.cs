using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompetitionScraper
{
    internal class ListManager
    {
        internal List<CustomStructure> GetList()
        {
            DictionaryManager dictionaryManager = new DictionaryManager();
            Dictionary<string, string> swimmers = dictionaryManager.GetUrls();
            List<CustomStructure> test = new List<CustomStructure>();
            Dictionary<string, int> maxValue = new Dictionary<string, int>();
            Dictionary<string, List<string>> helper = new Dictionary<string, List<string>>();
            Dictionary<string, Dictionary<string, string>> hugeDates = new Dictionary<string, Dictionary<string, string>>();

            List<string> allResults = new List<string>();
            foreach (var pair in swimmers)
            {
                var rawAge = ScrapingSystem.Loader(pair.Value).DocumentNode.SelectSingleNode("//div[@id='athleteinfo']/div[@id='name']").InnerText;
                System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(rawAge, @"\((\d{4})");
                Console.WriteLine(match.Groups[1].Value);
                string category = Category(Convert.ToInt32(match.Groups[1].Value.Trim()));
                var distances = ScrapingSystem.Loader(pair.Value).DocumentNode.SelectNodes("//td[@class='event']//a");
                foreach (var distance in distances)
                {
                    var events = ScrapingSystem.Loader("https://www.swimrankings.net/index.php" + distance.GetAttributeValue("href", "").Replace("amp;", "")).DocumentNode.SelectNodes("//a[@class='time']");
                    var dates = ScrapingSystem.Loader("https://www.swimrankings.net/index.php" + distance.GetAttributeValue("href", "").Replace("amp;", "")).DocumentNode.SelectNodes("//td[@class='date']");
                    var cities = ScrapingSystem.Loader("https://www.swimrankings.net/index.php" + distance.GetAttributeValue("href", "").Replace("amp;", "")).DocumentNode.SelectNodes("//td[@class='city']//a");
                    for (int e = 0; e < events.Count; e++)
                    {
                        if (Convert.ToInt32(dates[e].InnerText.Split("&nbsp;")[2]) == 2024)
                        {
                            var placeholder = ScrapingSystem.Loader(("https://www.swimrankings.net/index.php" + events[e].GetAttributeValue("href", "")).Replace("amp;", "")).DocumentNode.SelectNodes("//td")[13].InnerText;
                            if (!placeholder.Contains("Lap"))
                            {
                                var singlEvent = ScrapingSystem.Loader(("https://www.swimrankings.net/index.php" + events[e].GetAttributeValue("href", "")).Replace("amp;", "")).DocumentNode.SelectNodes("//td[@class='h4']//a")[1];
                                var competitionDate = ScrapingSystem.Loader(("https://www.swimrankings.net/index.php" + singlEvent.GetAttributeValue("href", "")).Replace("amp;", "")).DocumentNode.SelectNodes("//td[@class='titleRight']")[1].InnerText;
                                var place = ScrapingSystem.Loader(("https://www.swimrankings.net/index.php" + events[e].GetAttributeValue("href", "")).Replace("amp;", "")).DocumentNode.SelectNodes("//td")[15].InnerText;
                                StringBuilder result = new StringBuilder();
                                Dictionary<string, string> properties = new Dictionary<string, string>();
                                properties.Add(DataFormatingSystem.DateTranslation(competitionDate), cities[e].InnerText);
                                foreach (char c in place)
                                {
                                    result.Append(char.IsDigit(c) ? c : "");
                                }
                                string line = $"{DataFormatingSystem.ToTitleString(pair.Key)} {result} miejsce na {DataFormatingSystem.TranslateStroke(placeholder)} {singlEvent.InnerText} dnia {DataFormatingSystem.DateTranslation(dates[e].InnerText)}";
                                if (Convert.ToInt32(result.ToString()) != 450 && !allResults.Contains(line))
                                {
                                    if (!hugeDates.ContainsKey(singlEvent.InnerText))
                                    {
                                        hugeDates.Add(singlEvent.InnerText, properties);
                                    }
                                    if (helper.ContainsKey(singlEvent.InnerText))
                                    {
                                        helper[singlEvent.InnerText].Add($"{result} miejsce na {DataFormatingSystem.TranslateStroke(placeholder)}");
                                    }
                                    else
                                    {
                                        helper.Add(singlEvent.InnerText, new List<string> { $"{result} miejsce na {DataFormatingSystem.TranslateStroke(placeholder)}" });
                                    }
                                    allResults.Add(line);
                                    Console.WriteLine(line);
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
                //every line regarding CustomStructure is in testing stage
                CustomStructure cs = new CustomStructure(DataFormatingSystem.ToTitleString(pair.Key), helper,maxValue.Values.Max(), hugeDates, category);
                test.Add(cs);
            }
            return test;
        }

        public static string Category(int birth)
        {
            int today = DateTime.Today.Year;
            string category = "";
            int age = today - birth;
            if (age >= 0 && age <= 11)
            {
                category = "Dziecko";
            }
            else if (age >= 12 && age <= 13)
            {
                category = "Młodzik";
            }
            else if (age >= 14 && age <= 16)
            {
                category = "Junior Młodszy";
            }
            else if (age >= 17 && age <= 18)
            {
                category = "Junior";
            }
            else if (age >= 19 && age <= 23)
            {
                category = "Młodzieżowiec";
            }
            else if (age >= 24 && age <= 120)
            {
                category = "Senior";
            }
            else
            {
                category = "Nieznana category"; 
            }

            return category;
        }
    }
    public class CustomStructure
    {
        public string Text { get; set; }
        public Dictionary<string, List<string>> StringList { get; set; }
        public int Number { get; set; }
        public Dictionary<string, Dictionary<string, string>> Date { get; set; }
        public string Age  { get; set; }

        public CustomStructure(string text, Dictionary<string, List<string>> stringList, int number, Dictionary<string, Dictionary<string, string>> date, string age)
        {
            Text = text;
            StringList = stringList;
            Number = number;
            Date = date;
            Age = age;
        }
    }

}
