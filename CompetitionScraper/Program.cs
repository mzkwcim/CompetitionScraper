using HtmlAgilityPack;
using System.Text;
internal class Program
{

    private static void Main(string[] args)
    {
        string link = "https://www.swimrankings.net/index.php?page=rankingDetail&clubId=65773&gender=1&season=2024&course=LCM&stroke=0&agegroup=14014";

        var links = Loader(link).DocumentNode.SelectNodes("//td[@class='swimstyle']//a");
        
        string url = "";
        Dictionary<string, string> swimmers = new Dictionary<string, string>();
        for(int j = 0; j < 2; j++)
        {
            if (j == 1)
            {
                links = Loader(link.Replace("gender=1", "gender=2")).DocumentNode.SelectNodes("//td[@class='swimstyle']//a");
            }
            for (int i = 0; i < 2; i++)
            {
                if (i == 1)
                {
                    links = Loader(link.Replace("course=LCM", "course=SCM")).DocumentNode.SelectNodes("//td[@class='swimstyle']//a");
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
        }

        int counter = 0;
        foreach (var pair in swimmers)
        {
            Console.WriteLine($"{pair.Key}");
            var distances = Loader(pair.Value).DocumentNode.SelectNodes("//td[@class='event']//a");
            foreach (var distance in distances)
            {
                Console.WriteLine(distance.GetAttributeValue("href", ""));
                var events = Loader("https://www.swimrankings.net/index.php" + distance.GetAttributeValue("href","").Replace("amp;", "")).DocumentNode.SelectNodes("//a[@class='time']");
                foreach (var ev in events)
                {
                    Console.WriteLine("https://www.swimrankings.net/index.php" + ev.GetAttributeValue("href", "").Replace("amp;", ""));
                    var singlEvent = Loader(("https://www.swimrankings.net/index.php" + ev.GetAttributeValue("href", "")).Replace("amp;","")).DocumentNode.SelectNodes("//td[@class='h4']//a")[1];
                    var placeholder = Loader(("https://www.swimrankings.net/index.php" + ev.GetAttributeValue("href", "")).Replace("amp;", "")).DocumentNode.SelectNodes("//td")[13].InnerText;
                    var place = Loader(("https://www.swimrankings.net/index.php" + ev.GetAttributeValue("href", "")).Replace("amp;", "")).DocumentNode.SelectNodes("//td")[15].InnerText;
                    Console.WriteLine(singlEvent.InnerText);
                    Console.ReadKey();
                    StringBuilder result = new StringBuilder();

                    // Iterujemy przez każdy znak w ciągu wejściowym
                    foreach (char c in place)
                    {
                        // Sprawdzamy, czy znak jest cyfrą (liczbą)
                        if (char.IsDigit(c))
                        {
                            // Jeśli tak, dodajemy go do wyniku
                            result.Append(c);
                        }
                    }
                    Console.WriteLine($"{pair.Key} {result} miejsce na {placeholder} na {singlEvent.InnerText}");
                    counter++;
                }
                counter = 0;
            }

            
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
    public static string TranslateStroke(string key)
    {

        switch (key)
        {
            case "Freestyle": return " Dowolnym";
            case "Backstroke": return " Grzbietowym";
            case "Breaststroke": return " Klasycznym";
            case "Butterfly": return " Motylkowym";
            case "Medley": return " Zmiennym";
            default: return "";
        }
    }
}