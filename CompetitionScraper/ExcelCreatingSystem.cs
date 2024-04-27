using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompetitionScraper
{
    internal class ExcelCreatingSystem
    {
        private static string filePath = @"C:\Users\mzkwcim\desktop\testwyniki.xlsx";
        internal void CreatExcelWoorkbook()
        {
            ListManager listManager = new ListManager();
            List<CustomStructure> list = listManager.GetList();
            List<string> competitionDates = new List<string>();
            foreach(var cs in list)
            {
                foreach(var item in cs.Date)
                {
                    if (!competitionDates.Contains(item.Key))
                    {
                        competitionDates.Add(item.Key); 
                        Console.WriteLine(item.Key);
                    }
                }
            }
            int merger = 3 + competitionDates.Count*3;
            /*
            GroupingSystem groupingSystem = new GroupingSystem();
            Dictionary<string, List<string>> results = groupingSystem.GetGroupedData();
            using (var wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add("Dane");
                ws.Range("A4:A7").Merge();
                ws.Range("B4:B7").Merge();
                ws.Range("C4:C7").Merge();
                int rowNum = 1;
                int colNum = 1; 

                foreach (var kvp in results)
                {
                    ws.Cell(rowNum, 1).Value = kvp.Key;

                    foreach (var value in kvp.Value)
                    {
                        ws.Cell(rowNum + 1, colNum).Value = value;
                        rowNum++; 
                    }

                    colNum++; 
                    rowNum = 1; 
                }

                wb.SaveAs(filePath);
            }
            */
            Console.WriteLine("Plik Excela został utworzony pomyślnie.");
        }
    }
}
