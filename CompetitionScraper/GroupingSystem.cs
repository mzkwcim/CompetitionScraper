using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompetitionScraper
{
    internal class GroupingSystem
    {
        internal Dictionary<string, List<string>> GetGroupedData()
        {
            ListManager listManager = new ListManager();
            List<string> allResults = listManager.GetList();
            var results = new Dictionary<string, List<string>>();

            foreach (string line in allResults)
            {
                string[] parts = line.Split(' ');
                int startIndex = 8; // Indeks pierwszego słowa, które ma być kluczem
                int endIndex = parts.Length - 5; // Indeks ostatniego słowa, które ma być kluczem

                // Utwórz klucz zawierający słowa od startIndex do endIndex
                string key = string.Join(" ", parts.Skip(startIndex).Take(endIndex - startIndex + 1));

                // Utwórz wartość zawierającą pierwsze 8 słów
                string value = string.Join(" ", parts.Take(8));

                if (!results.ContainsKey(key))
                {
                    results[key] = new List<string>();
                }
                results[key].Add(value);
            }

            foreach (var kvp in results)
            {
                Console.WriteLine($"{kvp.Key}: {string.Join(", ", kvp.Value)}");
            }
            return results;
        }
    }
}
