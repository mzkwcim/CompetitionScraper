using System.Text.RegularExpressions;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;

namespace CompetitionScraper
{
    internal class ClubUrlGettingSystem
    {
        internal string GetClubName()
        {
            Console.WriteLine("Podaj Nazwę klubu: ");
            var insertedClubName = Console.ReadLine();
            IWebDriver driver = new ChromeDriver();
            driver.Navigate().GoToUrl("https://www.swimrankings.net/index.php?page=clubSelect");
            var clubName = driver.FindElement(By.XPath("//*[@id=\"club_name\"]"));
            clubName.SendKeys(insertedClubName);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            var elements = driver.FindElements(By.XPath($"//a[contains(translate(text(), 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), translate('{insertedClubName}', 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'))]"));
            string url = "";
            if (elements.Count > 0)
            {
                // Jeśli znaleziono elementy, pobierz URL pierwszego elementu
                url = elements[0].GetAttribute("href");
                driver.Quit();
            }
            else
            {
                // Jeśli nie znaleziono elementów, zwróć odpowiednią wartość
                Console.WriteLine("Nie znaleziono elementów na stronie.");
                driver.Quit();
                url = "Nie";
            }
            if (url == "Nie")
            {
                Console.WriteLine("Program zostanie zakończony");
                return null;
            }
            else
            {
                var first = ScrapingSystem.Loader(url).DocumentNode.SelectSingleNode("//td//a");
                string urlPrefix = "https://www.swimrankings.net/index.php?page=rankingDetail&clubId=";
                string clubId = Regex.Replace(first.GetAttributeValue("href", ""), @"^.+clubId=|&.+", "");
                string urlSufix = "&gender=1&course=SCM&agegroup=0";
                string newUrl = $"{urlPrefix}{clubId}{urlSufix}";

                return newUrl;
            }
        }
    }
}
