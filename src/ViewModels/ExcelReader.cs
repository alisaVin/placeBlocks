using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;

namespace placing_block.src
{
    public class ExcelReader
    {
        public List<BlockDataModel> ReadInputData()
        {
            string coordPathFile = "C:\\Projects\\Bundesverwaltungsgericht\\coord_UG_test.xlsx";
            List<BlockDataModel> blockData = new List<BlockDataModel>();

            using (SpreadsheetDocument doc = SpreadsheetDocument.Open(coordPathFile, true))
            {
                //WorkbookPart workbookPart = doc.WorkbookPart;
                //SharedStringTablePart sstpart = workbookPart.GetPartsOfType<SharedStringTablePart>().First();
                //SharedStringTable sst = sstpart.SharedStringTable;

                //WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
                //Worksheet sheet = worksheetPart.Worksheet;


                //var cells = sheet.Descendants<Cell>();
                //var rows = sheet.Descendants<Row>(); 

                WorkbookPart workbookPart = doc.WorkbookPart;
                var sheet = workbookPart.Workbook.Descendants<Sheet>()
                                                    .First(s => s.Name == "UG");

                WorksheetPart worksheetPart = workbookPart.GetPartById(sheet.Id) as WorksheetPart;
                var rows = worksheetPart.Worksheet.GetFirstChild<SheetData>()
                                                    .Elements<Row>();
                var dataRows = rows.Skip(1);

                foreach (Row row in dataRows)
                {
                    BlockDataModel blockRecord = new BlockDataModel();

                    var cells = row.Elements<Cell>();
                    //var sst = workbookPart.SharedStringTablePart?
                    //                              .SharedStringTable
                    //                              .Elements<SharedStringItem>()
                    //                              .ToList();

                    foreach (Cell cell in cells)
                    {
                        string col = GetColumnName(cell.CellReference);
                        //if (sst != null && int.TryParse(cell.CellValue.InnerText, out int idx) && idx < sst.Count)
                        //{

                        //
                        //    return sst[idx].InnerText;

                        switch (col)
                        {
                            case "A":
                                string point = cell.CellValue.InnerText;
                                if (point != "" || point != null)
                                    blockRecord.PunktNum = point;
                                break;

                            case "B":
                                double xCoord = double.Parse(cell.CellValue.Text);
                                blockRecord.X = xCoord;
                                break;

                            case "C":
                                double yCoord = double.Parse(cell.CellValue.Text);
                                blockRecord.Y = yCoord;
                                break;
                        }
                        //}
                    }

                    blockData.Add(blockRecord);
                }
            }

            foreach (var blData in blockData)
            {
                Console.WriteLine($"Point Num: {blData.PunktNum}\tX_Coord: {blData.X}\tY_Coord: {blData.Y}");
            }
            return blockData;
        }

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

        private static string GetColumnName(string cellReference)
        {
            return new string(cellReference
                .TakeWhile(c => char.IsLetter(c))
                .ToArray());
        }
    }
}
