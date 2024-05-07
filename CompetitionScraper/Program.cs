using CompetitionScraper;
using System.Diagnostics;
internal class Program
{
    private static void Main(string[] args)
    {

        Stopwatch sw = new Stopwatch();
        sw.Start();
        ExcelCreatingSystem excelCreatingSystem = new ExcelCreatingSystem();
        excelCreatingSystem.CreatExcelWoorkbook();
        sw.Stop();
        Console.WriteLine(sw.ElapsedMilliseconds);
    }
}
