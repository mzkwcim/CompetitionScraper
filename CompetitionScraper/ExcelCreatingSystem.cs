using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
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
            using (var wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add("Dane");
                ws.Cell("A4").Value = "L.p. (*)";
                ws.Cell("B4").Value = "Imię i nazwisko zawodnika objętego szkoleniem w 2023 roku";
                ws.Cell("C4").Value = "Kategoria wiekowa";
                ws.Range("A4:A7").Merge();
                ws.Range("B4:B7").Merge();
                ws.Range("C4:C7").Merge();
                ws.Cell("A1").Value = "UCZESTNICTWO W ZAWODACH OBJĘTYCH SYSTEMEM WSPÓŁZAWODNICTWA SPORTOWEGO W 2023 ROKU";
                ws.Cell("A2").Value = "Nazwa klubu: KS POSNANIA";
                ws.Cell("A3").Value = "Dyscyplina: PŁYWANIE";
                int start = 4;
                foreach(var cs in list)
                {
                    foreach (var item in cs.Date)
                    {
                        if (!competitionDates.Contains(item.Key))
                        {
                            competitionDates.Add(item.Key);
                            string startRange = DataFormatingSystem.NumberToExcelColumn(start);
                            string endRange = DataFormatingSystem.NumberToExcelColumn(start + 2);
                            ws.Range($"{startRange}4:{endRange}4").Merge();
                            ws.Range($"{startRange}5:{endRange}5").Merge();
                            ws.Range($"{startRange}6:{endRange}6").Merge();
                            ws.Cell($"{startRange}4").Value = item.Key;
                            ws.Range($"{startRange}7").Value = "udział (*)";
                            ws.Range($"{DataFormatingSystem.NumberToExcelColumn(start+1)}7").Value = "konkurencja";
                            ws.Range($"{endRange}7").Value = "zajęte miejsce";
                            foreach (var date in item.Value)
                            {
                                ws.Cell($"{startRange}5").Value = date.Key;
                                ws.Cell($"{startRange}6").Value = date.Value;
                            }
                            start += 3;
                        }
                    }
                }
                int merger = 3 + competitionDates.Count * 3;
                string excelChar = DataFormatingSystem.NumberToExcelColumn(merger);
                ws.Range($"A1:{excelChar}1").Merge();
                ws.Range($"A2:{excelChar}2").Merge();
                ws.Range($"B3:{excelChar}3").Merge();
                int counter = 8;
                int athleteCounter = 1;
                int innerAdder = 0;
                foreach (var athlete in list)
                {
                    ws.Cell($"A{counter}").Value = athleteCounter;
                    ws.Cell($"B{counter}").Value = athlete.Text;
                    ws.Cell($"C{counter}").Value = athlete.Age;
                    foreach (var competition in athlete.StringList)
                    {
                        foreach (IXLColumn column in ws.ColumnsUsed())
                        {
                            if (column.CellsUsed().Any(cell => cell.Value.ToString() == competition.Key))
                            {
                                int collumnNumber = column.ColumnNumber();
                                foreach(var swimmingEvent in competition.Value)
                                {
                                    ws.Cell($"{DataFormatingSystem.NumberToExcelColumn(collumnNumber)}{counter + innerAdder}").Value = 1;
                                    ws.Cell($"{DataFormatingSystem.NumberToExcelColumn(collumnNumber + 1)}{counter + innerAdder}").Value = String.Join(" ",swimmingEvent.Split(" ")[3..]);
                                    ws.Cell($"{DataFormatingSystem.NumberToExcelColumn(collumnNumber + 2)}{counter + innerAdder}").Value = Convert.ToInt32(swimmingEvent.Split(" ")[0]);
                                    innerAdder++;
                                }
                                innerAdder = 0;
                            }
                        }
                    }
                    counter += athlete.Number;
                    athleteCounter++;
                }
                var range = ws.Range($"A1:{excelChar}{counter}");
                range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                range.Style.Alignment.WrapText = true;
                ws.RangeUsed().Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.RangeUsed().Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.RangeUsed().Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.RangeUsed().Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Columns().AdjustToContents(); // Dostosuj szerokość wszystkich kolumn
                ws.Rows().AdjustToContents();



                wb.SaveAs(filePath);
            }
            Console.WriteLine("Plik Excela został utworzony pomyślnie.");
        }
    }
}
