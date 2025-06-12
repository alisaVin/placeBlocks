using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using placing_block.src.Models;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace placing_block.src
{
    public class ExcelReader
    {
        public List<BlockDataModel> ReadInputData(string coordPathFile, string blockName, string etage)
        {
            //string coordPathFile = "C:\\Projects\\Bundesverwaltungsgericht\\coord_UG_test.xlsx";
            List<BlockDataModel> blockData = new List<BlockDataModel>();

            using (SpreadsheetDocument doc = SpreadsheetDocument.Open(coordPathFile, true))
            {
                WorkbookPart workbookPart = doc.WorkbookPart;
                var sheet = workbookPart.Workbook.Descendants<Sheet>()
                                    .First(s => s.Name == "Technische Anlage");

                WorksheetPart worksheetPart = workbookPart.GetPartById(sheet.Id) as WorksheetPart;
                var rows = worksheetPart.Worksheet.GetFirstChild<SheetData>()
                                                    .Elements<Row>();
                var dataRows = rows.Skip(1);
                var clearRows = dataRows.Where(row => !row.Elements<Cell>()
                                       .Any(cell => cell.DataType?.Value == CellValues.Error))
                                       .ToList();

                foreach (Row row in clearRows)
                {
                    BlockDataModel blockRecord = new BlockDataModel();

                    var cells = row.Elements<Cell>();
                    var sst = workbookPart.SharedStringTablePart?
                                                  .SharedStringTable
                                                  .Elements<SharedStringItem>()
                                                  .ToList();

                    var nameCell = cells.FirstOrDefault(c => string.Compare(GetColumnName(c.CellReference), "O", true) == 0);
                    if (nameCell == null) continue;

                    var nameValue = GetCellText(nameCell, workbookPart);
                    if (nameValue != blockName) continue;

                    var etCell = cells.FirstOrDefault(c => GetColumnName(c.CellReference) == "L");
                    string etageInp = GetCellText(etCell, workbookPart);
                    blockRecord.Etage = etageInp;

                    if (etageInp == etage)
                    {
                        var bezCell = cells.FirstOrDefault(c => GetColumnName(c.CellReference) == "B");
                        var bezValue = GetCellText(bezCell, workbookPart);
                        blockRecord.TABezeichnung = bezValue;

                        var xCell = cells.FirstOrDefault(c => GetColumnName(c.CellReference) == "I");
                        var xValue = GetCellText(xCell, workbookPart);
                        if (xValue != "-1" && xValue != null)
                        {
                            double xCoord = double.Parse(xValue, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture);
                            blockRecord.X = xCoord;
                        }

                        var yCell = cells.FirstOrDefault(c => GetColumnName(c.CellReference) == "J");
                        var yValue = GetCellText(yCell, workbookPart);
                        if (yValue != "-1" && yValue != null)
                        {
                            double yCoord = double.Parse(yValue, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture);
                            blockRecord.Y = yCoord;
                        }
                    }


                    //foreach (Cell cell in cells)
                    //{
                    //if (sst != null && int.TryParse(cell.CellValue.InnerText, out int idx) && idx < sst.Count)
                    //{

                    //
                    //    return sst[idx].InnerText;

                    //string col = GetColumnName(cell.CellReference);
                    //string text = GetCellText(cell, workbookPart);


                    //switch (col)
                    //{
                    //    //case "A":
                    //    //    string taId = cell.CellValue.InnerText;
                    //    //    if (taId != "" || taId != null)
                    //    //        blockRecord.TAId = taId;
                    //    //    break;

                    //    case "B":
                    //        blockRecord.TABezeichnung = text;
                    //        break;

                    //    //case "C":
                    //    //    string taGroup = cell.CellValue.InnerText;
                    //    //    blockRecord.TAGruppe = taGroup;
                    //    //    break;

                    //    //case "E":
                    //    //    string pointNum = cell.CellValue.InnerText;
                    //    //    blockRecord.PunktNum = pointNum;
                    //    //    break;

                    //    // Y in der Tabelle geschrieben; X - in ares
                    //    case "I":
                    //        string rawXCoord = GetCellText(cell, workbookPart);
                    //        if (rawXCoord != "-1" && rawXCoord != null)
                    //        {
                    //            double xCoord = double.Parse(rawXCoord, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture);
                    //            blockRecord.X = xCoord;
                    //        }
                    //        break;
                    //    // X in der Tabelle geschrieben; Y - in ares
                    //    case "J":
                    //        string rawYCoord = GetCellText(cell, workbookPart);
                    //        if (rawYCoord != "-1" && rawYCoord != null)
                    //        {
                    //            double yCoord = double.Parse(rawYCoord, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture);
                    //            blockRecord.Y = yCoord;
                    //        }
                    //        break;

                    //    case "L":

                    //        string etageInp = GetCellText(cell, workbookPart);
                    //        blockRecord.Etage = etageInp;
                    //        break;
                    //}
                    //}
                    //}
                    blockData.Add(blockRecord);
                }
            }
            return blockData;
        }

        private static string GetColumnName(string cellReference)
        {
            return new string(cellReference
                .TakeWhile(c => char.IsLetter(c))
                .ToArray());
        }

        private string GetCellText(Cell cell, WorkbookPart wbPart)
        {
            // Wenn kein Wert vorhanden, leer zurückgeben
            if (cell.CellValue == null)
                return string.Empty;

            string raw = cell.CellValue.Text;

            // SharedString-Lookup
            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                // Index parsen
                if (int.TryParse(raw, out int ssid))
                {
                    var sstPart = wbPart.GetPartsOfType<SharedStringTablePart>()
                                        .FirstOrDefault();
                    if (sstPart?.SharedStringTable != null)
                    {
                        // InnerText ist der tatsächliche Zell­inhalt
                        return sstPart.SharedStringTable
                                      .ElementAt(ssid)
                                      .InnerText;
                    }
                }
            }
            return raw;
        }
    }
}

//WorkbookPart workbookPart = doc.WorkbookPart;
//SharedStringTablePart sstpart = workbookPart.GetPartsOfType<SharedStringTablePart>().First();
//SharedStringTable sst = sstpart.SharedStringTable;

//WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
//Worksheet sheet = worksheetPart.Worksheet;


//var cells = sheet.Descendants<Cell>();
//var rows = sheet.Descendants<Row>(); 


//private IEnumerable<Row> GetDataRows(string coordPathFile, string sheetName)
//{
//    using (SpreadsheetDocument doc = SpreadsheetDocument.Open(coordPathFile, true))
//    {
//        //WorkbookPart workbookPart = doc.WorkbookPart;
//        //SharedStringTablePart sstpart = workbookPart.GetPartsOfType<SharedStringTablePart>().First();
//        //SharedStringTable sst = sstpart.SharedStringTable;

//        //WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
//        //Worksheet sheet = worksheetPart.Worksheet;


//        //var cells = sheet.Descendants<Cell>();
//        //var rows = sheet.Descendants<Row>(); 

//        WorkbookPart workbookPart = doc.WorkbookPart;
//        var sheet = workbookPart.Workbook.Descendants<Sheet>()
//                                            .First(s => s.Name == sheetName);

//        WorksheetPart worksheetPart = workbookPart.GetPartById(sheet.Id) as WorksheetPart;
//        var rows = worksheetPart.Worksheet.GetFirstChild<SheetData>()
//                                            .Elements<Row>();
//        var dataRows = rows.Skip(1);
//        return dataRows;
//    }
//}

//private static string GetCellValue(Cell cell, WorkbookPart wbPart)
//{
//    if (cell.CellValue == null)
//        return string.Empty;

//    string value = cell.CellValue.InnerText;

//    // Shared String? Dann in der SharedStringTable nachschlagen
//    if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
//    {
//        var sst = wbPart.SharedStringTablePart?
//                     .SharedStringTable
//                     .Elements<SharedStringItem>()
//                     .ToList();
//        if (sst != null && int.TryParse(value, out int idx) && idx < sst.Count)
//            return sst[idx].InnerText;
//    }

//    return value;
//}
