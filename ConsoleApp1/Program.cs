using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TrimbleDataCSVProcessor;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            int errorCount = 0;
            var pathSource = @"C:\Users\Yishi_Liu\Documents\TTLDOG-1517.csv";
            var pathRef = @"C:\Users\Yishi_Liu\Documents\AU-AssetList.xlsx";
            var pathOutput = @"C:\Users\Yishi_Liu\Documents\CSVTEST";

            var refDic = new CSVProcessor().ParseIntoDictionaryFromFile(pathRef);

            var sw = new Stopwatch();
            sw.Start();
            Console.Write("Reading total number of lines in the CSV file: ");
            var totalCount = File.ReadLines(pathSource).Count();
            Console.WriteLine(totalCount);

            using (var writer = new CSVFilesWriteDispatcher(pathOutput))
            using (var sr = new StreamReader(pathSource))
            {
                string line = null;
                var firstLine = true;
                while ((line = sr.ReadLine()) != null)
                {
                    if (firstLine)
                    {
                        firstLine = false;
                        continue;
                    }
                    var item = new ReportModel(line, refDic);
                    if (item.IsValid)
                    {
                        writer.WriteToCSVFilesAccordingToMonth(item);
                    }
                    else
                    {
                        errorCount++;
                    }
                }

            }

            sw.Stop();
            Console.WriteLine("Eplased time: " + sw.Elapsed.Seconds);
            Console.WriteLine("Error Count: " + errorCount);
            Console.ReadLine();
        }
    }
}
