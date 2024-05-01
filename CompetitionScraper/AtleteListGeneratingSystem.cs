using System.Text;

namespace CompetitionScraper
{
    internal class AtleteListGeneratingSystem
    {
        internal Dictionary<string, string> GetUrls()
        {
            List<string> athletesCategories = ["agegroup=12012", "agegroup=13013", "agegroup=14014", "agegroup=15015", "agegroup=16016", "agegroup=17017", "agegroup=18018", "agegroup=19000", "agegroup=0"];
            List<string> genderList = ["gender=1", "gender=2"];
            List<string> poolLength = ["course=LCM", "course=SCM"];
            string link = "https://www.swimrankings.net/index.php?page=rankingDetail&clubId=65773&gender=1&season=2024&course=LCM&stroke=0&agegroup=13013";
            string swimrankingsPrefix = "https://www.swimrankings.net/index.php";
            Dictionary<string, string> swimmers = new Dictionary<string, string>();
            Parallel.ForEach(athletesCategories, athlete =>
            {
                for (int j = 0; j < 2; j++)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        string changedLink = link.Replace("course=LCM", $"{poolLength[i]}").Replace("gender=1", $"{genderList[j]}").Replace("agegroup=13013", $"{athlete}");
                        var links = ScrapingSystem.Loader(changedLink).DocumentNode.SelectNodes("//td[@class='swimstyle']//a");
                        Parallel.ForEach(links, (linky) =>
                        {
                            var athletes = ScrapingSystem.Loader((swimrankingsPrefix + linky.GetAttributeValue("href", "")).Replace("amp;", ""))
                                                          .DocumentNode.SelectNodes("//td[@class='fullname']//a");

                            if (athletes != null)
                            {
                                foreach (var athlete in athletes)
                                {
                                    lock (swimmers)
                                    {
                                        if (!swimmers.ContainsKey(athlete.InnerText))
                                        {
                                            swimmers.Add(athlete.InnerText, (swimrankingsPrefix + athlete.GetAttributeValue("href", "")).Replace("amp;", ""));
                                        }
                                    }
                                }
                            }
                        });
                    }
                }
            });
           

            return swimmers;
        }
    }
}
