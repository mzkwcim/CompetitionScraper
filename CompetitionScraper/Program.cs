using HtmlAgilityPack;
using System.Text;
using CompetitionScraper;
using ClosedXML;
using ClosedXML.Excel;
internal class Program
{
    private static void Main(string[] args)
    {
        ExcelCreatingSystem excelCreatingSystem = new ExcelCreatingSystem();
        excelCreatingSystem.CreatExcelWoorkbook();
    }
}
