using CompetitionScraper;
using System.Diagnostics;
internal class Program
{
    private static void Main(string[] args)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        ExcelCreatingSystem excelCreatingSystem = new ExcelCreatingSystem();
        excelCreatingSystem.CreatExcelWoorkbook();
        stopwatch.Stop();
        Console.WriteLine($"czas wykonania: {stopwatch.Elapsed.TotalSeconds:F3} s");
    }
}
