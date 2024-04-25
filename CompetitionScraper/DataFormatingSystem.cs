using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompetitionScraper
{
    internal class DataFormatingSystem
    {
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
            string[] divide = key.Split(' ');
            switch (divide[1])
            {
                case "Freestyle": return $"{divide[0]} stylem dowolnym";
                case "Backstroke": return $"{divide[0]} stylem grzbietowym";
                case "Breaststroke": return $"{divide[0]} stylem klasycznym";
                case "Butterfly": return $"{divide[0]} stylem motylkowym";
                case "Medley": return $"{divide[0]} stylem zmiennym";
                default: return "";
            }
        }
        private static string GetMonthTranslation(string partsOfEnglishDate)
        {
            switch (partsOfEnglishDate)
            {
                case "Jan": return "Stycznia";
                case "Feb": return "Lutego";
                case "Mar": return "Marca";
                case "Apr": return "Kwietnia";
                case "May": return "Maja";
                case "Jun": return "Czerwca";
                case "Jul": return "Lipca";
                case "Aug": return "Sierpnia";
                case "Sep": return "Września";
                case "Oct": return "Października";
                case "Nov": return "Listopada";
                case "Dec": return "Grudnia";
                default: return "";
            }
        }
        public static string DateTranslation(string englishDate)
        {
            string[] partsOfEnglishDate = englishDate.Split("&nbsp;");
            return (partsOfEnglishDate.Length == 4) ? $"{partsOfEnglishDate[1]} {GetMonthTranslation(partsOfEnglishDate[2])} {partsOfEnglishDate[3]}" : $"{partsOfEnglishDate[0]} {GetMonthTranslation(partsOfEnglishDate[1])} {partsOfEnglishDate[2]}";
        }
    }
}
