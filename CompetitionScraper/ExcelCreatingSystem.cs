using ClosedXML.Excel;

namespace CompetitionScraper
{
    internal class ExcelCreatingSystem
    {
        private static string filePath = @"$home\desktop\testwyniki.xlsx";
        internal void CreatExcelWoorkbook()
        {
            ListManager listManager = new ListManager();
            List<CustomStructure> swimmersList = listManager.GetList();
            List<string> competitionDates = new List<string>();
            using (var woorkBook = new XLWorkbook())
            {
                var woorkSheet = woorkBook.Worksheets.Add("Dane");
                woorkSheet.Cell("A4").Value = "L.p. (*)";
                woorkSheet.Cell("B4").Value = $"Imię i nazwisko zawodnika objętego szkoleniem w {DateTime.Today.Year} roku";
                woorkSheet.Cell("C4").Value = "Kategoria wiekowa";
                woorkSheet.Range("A4:A7").Merge();
                woorkSheet.Range("B4:B7").Merge();
                woorkSheet.Range("C4:C7").Merge();
                woorkSheet.Cell("A1").Value = $"UCZESTNICTWO W ZAWODACH OBJĘTYCH SYSTEMEM WSPÓŁZAWODNICTWA SPORTOWEGO W {DateTime.Today.Year} ROKU";
                woorkSheet.Cell("A2").Value = "Nazwa klubu: KS POSNANIA";
                woorkSheet.Cell("A3").Value = "Dyscyplina: PŁYWANIE";
                int startCollumn = 4;
                foreach(var swimmer in swimmersList)
                {
                    foreach (var competitionName in swimmer.CompetitionNameCityAndName)
                    {
                        if (!competitionDates.Contains(competitionName.Key))
                        {
                            competitionDates.Add(competitionName.Key);
                            string firstCollumnInInterval = DataFormatingSystem.NumberToExcelColumn(startCollumn);
                            string lastCollumnInInterval = DataFormatingSystem.NumberToExcelColumn(startCollumn + 2);
                            woorkSheet.Range($"{firstCollumnInInterval}4:{lastCollumnInInterval}4").Merge();
                            woorkSheet.Range($"{firstCollumnInInterval}5:{lastCollumnInInterval}5").Merge();
                            woorkSheet.Range($"{firstCollumnInInterval}6:{lastCollumnInInterval}6").Merge();
                            woorkSheet.Cell($"{firstCollumnInInterval}4").Value = competitionName.Key;
                            woorkSheet.Range($"{firstCollumnInInterval}7").Value = "udział (*)";
                            woorkSheet.Range($"{DataFormatingSystem.NumberToExcelColumn(startCollumn + 1)}7").Value = "konkurencja";
                            woorkSheet.Range($"{lastCollumnInInterval}7").Value = "zajęte miejsce";
                            foreach (var date in competitionName.Value)
                            {
                                woorkSheet.Cell($"{firstCollumnInInterval}5").Value = date.Key;
                                woorkSheet.Cell($"{firstCollumnInInterval}6").Value = date.Value;
                            }
                            startCollumn += 3;
                        }
                    }
                }
                int lastCollumnInt = 3 + competitionDates.Count * 3;
                string lastCollumnInWoorkSheet = DataFormatingSystem.NumberToExcelColumn(lastCollumnInt);
                woorkSheet.Range($"A1:{lastCollumnInWoorkSheet}1").Merge();
                woorkSheet.Range($"A2:{lastCollumnInWoorkSheet}2").Merge();
                woorkSheet.Range($"B3:{lastCollumnInWoorkSheet}3").Merge();
                int counter = 8;
                int athleteCounter = 1;
                int AthleteStartsCounter = 0;
                foreach (var athlete in swimmersList)
                {
                    woorkSheet.Cell($"A{counter}").Value = athleteCounter;
                    woorkSheet.Cell($"B{counter}").Value = athlete.AthleteName;
                    woorkSheet.Cell($"C{counter}").Value = athlete.AthleteCategory;
                    foreach (var competition in athlete.ListOfStartsOnCompetition)
                    {
                        foreach (IXLColumn column in woorkSheet.ColumnsUsed())
                        {
                            if (column.CellsUsed().Any(cell => cell.Value.ToString() == competition.Key))
                            {
                                int collumnNumber = column.ColumnNumber();
                                foreach(var swimmingEvent in competition.Value)
                                {
                                    Console.WriteLine(athlete.AthleteName);
                                    Console.WriteLine(String.Join(" ", swimmingEvent.Split(" ")[3..]));
                                    Console.WriteLine(swimmingEvent.Split(" ")[0]);
                                    woorkSheet.Cell($"{DataFormatingSystem.NumberToExcelColumn(collumnNumber)}{counter + AthleteStartsCounter}").Value = 1;
                                    woorkSheet.Cell($"{DataFormatingSystem.NumberToExcelColumn(collumnNumber + 1)}{counter + AthleteStartsCounter}").Value = String.Join(" ",swimmingEvent.Split(" ")[3..]);
                                    woorkSheet.Cell($"{DataFormatingSystem.NumberToExcelColumn(collumnNumber + 2)}{counter + AthleteStartsCounter}").Value = Convert.ToInt32(swimmingEvent.Split(" ")[0]);
                                    AthleteStartsCounter++;
                                }
                                AthleteStartsCounter = 0;
                            }
                        }
                    }
                    counter += athlete.MaximumNumberOfStartsOnCompetition;
                    athleteCounter++;
                }
                var usedSizeOfSpreadSheet = woorkSheet.Range($"A1:{lastCollumnInWoorkSheet}{counter}");
                usedSizeOfSpreadSheet.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                usedSizeOfSpreadSheet.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                usedSizeOfSpreadSheet.Style.Alignment.WrapText = true;
                woorkSheet.RangeUsed().Style.Border.TopBorder = XLBorderStyleValues.Thin;
                woorkSheet.RangeUsed().Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                woorkSheet.RangeUsed().Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                woorkSheet.RangeUsed().Style.Border.RightBorder = XLBorderStyleValues.Thin;
                woorkSheet.Columns().AdjustToContents();
                woorkSheet.Rows().AdjustToContents();
                woorkBook.SaveAs(filePath);
            }
            Console.WriteLine("Plik Excela został utworzony pomyślnie.");
        }
    }
}
