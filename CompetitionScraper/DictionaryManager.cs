using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompetitionScraper
{
    internal class DictionaryManager
    {
        private List<string> ourSwimmers = ["Bartoszewska Marta", "Berg Maria", "Dopierala Piotr"];
        internal Dictionary<string,string> GetUrls()
        {
            string link = "https://www.swimrankings.net/index.php?page=rankingDetail&clubId=65773&gender=1&season=2024&course=LCM&stroke=0&agegroup=13013";

            var links = ScrapingSystem.Loader(link).DocumentNode.SelectNodes("//td[@class='swimstyle']//a");
            Dictionary<string, string> swimmers = new Dictionary<string, string>();
            for (int a = 0; a < 3; a++)
            {
                for (int j = 0; j < 2; j++)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        if (i == 1)
                        {
                            links = ScrapingSystem.Loader(link.Replace("course=LCM", "course=SCM").Replace("gender=1", $"gender={1 + j}").Replace("agegroup=13013", $"agegroup={13 + a}0{13 + a}")).DocumentNode.SelectNodes("//td[@class='swimstyle']//a");
                        }
                        foreach (var linky in links)
                        {
                            var athletes = ScrapingSystem.Loader(("https://www.swimrankings.net/index.php" + linky.GetAttributeValue("href", "")).Replace("amp;", "")).DocumentNode.SelectNodes("//td[@class='fullname']//a");
                            foreach (var athlete in athletes)
                            {
                                if (!swimmers.ContainsKey(athlete.InnerText) && ourSwimmers.Any(x => x == DataFormatingSystem.ToTitleString(athlete.InnerText)))
                                {
                                    swimmers.Add(athlete.InnerText, ("https://www.swimrankings.net/index.php" + athlete.GetAttributeValue("href", "")).Replace("amp;", ""));
                                }
                            }
                        }
                    }
                }
            }
            return swimmers;
        }
    }
}
