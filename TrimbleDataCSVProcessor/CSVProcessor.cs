using System;
using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;

namespace TrimbleDataCSVProcessor
{
    public class CsvProcessor
    {
        /// <summary>
        /// Load the reference Excel file into memory and a lookup dictionary which has key EquipmentId, value AssetId
        /// </summary>
        /// <param name="path">Path to excel containing two columns - AssetId and EquipmentId</param>
        /// <returns>A sorted dictionary</returns>
        public SortedDictionary<string, string> ParseIntoDictionaryFromFile(string path)
        {
            var result = new SortedDictionary<string, string>();
            var fi = new FileInfo(path);
            using (var p = new ExcelPackage(fi))
            {
                var ws = p.Workbook.Worksheets[0];
                var end = ws.Dimension.Rows;
                for (var i = 2; i <= end; i++)
                {
                    result.Add(ws.Cells[$"B{i}"].Text, ws.Cells[$"A{i}"].Text);
                }
            }
            return result;
        }
    }
}
