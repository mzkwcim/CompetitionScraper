using HtmlAgilityPack;
internal class Program
{

    private static void Main(string[] args)
    {
        string link = "https://www.swimrankings.net/index.php?page=rankingDetail&clubId=65773&gender=1&season=2024&course=LCM&stroke=0&agegroup=14014";

        var links = Loader(link).DocumentNode.SelectNodes("//td[@class='swimstyle']//a");
        
        string url = "";
        Dictionary<string, string> swimmers = new Dictionary<string, string>();
        for (int i = 0; i < 2; i++)
        {
            if (i == 1)
            {
                links =  Loader(link.Replace("course=LCM", "course=SCM")).DocumentNode.SelectNodes("//td[@class='swimstyle']//a");
            }
            foreach (var linky in links)
            {
                url = ("https://www.swimrankings.net/index.php" + linky.GetAttributeValue("href", "")).Replace("amp;", "");
                Console.WriteLine(url);
                var athletes = Loader(url).DocumentNode.SelectNodes("//td[@class='fullname']//a");
                foreach (var athlete in athletes)
                {
                    Console.WriteLine(athlete.InnerText);
                    if (!swimmers.ContainsKey(athlete.InnerText))
                    {
                        swimmers.Add(athlete.InnerText, ("https://www.swimrankings.net/index.php" + athlete.GetAttributeValue("href", "")).Replace("amp;", ""));
                    }
                }
            }
        }
        
        foreach (var pair in swimmers)
        {
            Console.WriteLine($"Name: {pair.Key}, Favorite sport: {pair.Value}");
        }
    }
    public static HtmlAgilityPack.HtmlDocument Loader(string url)
    {
        var httpClient = new HttpClient();
        var html = httpClient.GetStringAsync(url).Result;
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(html);
        return htmlDocument;
    }
    public static string ToTitleString(string fullname)
    {
        string[] words = fullname.Split(' ');
        for (int i = 0; i < words.Length; i++)
        {
            words[i] = char.ToUpper(words[i][0]) + words[i][1..].ToLower();
        }
        return string.Join(" ", words).Replace(",", "");
    }
}