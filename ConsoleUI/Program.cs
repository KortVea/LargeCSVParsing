using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using TrimbleDataCSVProcessor;

namespace ConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {
            var pathSource = @"C:\Users\Yishi_Liu\Documents\TTLDOG-1517.csv";
            var pathRef = @"C:\Users\Yishi_Liu\Documents\AU-AssetList.xlsx";
            var pathOutput = @"C:\Users\Yishi_Liu\Documents\CSVTEST";

            Console.Write($"Reading {pathRef} ...");
            var refDic = new CSVProcessor().ParseIntoDictionaryFromFile(pathRef);
            Console.WriteLine("Done.");

            var sw = new Stopwatch();
            sw.Start();
            Console.Write("Reading total number of lines in the CSV file: ");
            var totalCount = File.ReadLines(pathSource).Count();
            Console.WriteLine(totalCount);

            var fileNameList = new List<string>();
            var index = 0;
            int errorCount = 0;
            using (var writer = new CSVFilesWriteDispatcher(pathOutput))
            using (var sr = new StreamReader(pathSource))
            {
                string line = null;
                var firstLine = true;
                while ((line = sr.ReadLine()) != null)
                {
                    index++;
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
                    if (index % 10000 == 0)
                    {
                        drawTextProgressBar(index, totalCount);
                    }
                }
                drawTextProgressBar(index, totalCount);

                Console.WriteLine("Generating Report...");
                fileNameList = writer.GetNamesOfGeneratedFile();
                Console.WriteLine("Done.");
            }

            sw.Stop();
            Console.WriteLine("Total Eplased Time: " + sw.Elapsed.ToString());
            Console.WriteLine("Error Count: " + errorCount);

            sw.Restart();
            Console.WriteLine("Generating Report ...");
            GenerateFileReport(fileNameList, pathOutput);
            sw.Stop();
            Console.WriteLine("Total Eplased Time: " + sw.Elapsed.ToString());

            Console.ReadLine();
        }

        private static void GenerateFileReport(List<string> fileNameList, string pathOutput)
        {
            var reportPath = Path.Combine(pathOutput, $"Report-{DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss", CultureInfo.InvariantCulture)}.txt");
            using (var sw = new StreamWriter(reportPath))
            {
                var count = fileNameList.Count();
                for (int i = 0; i < count; i++)
                {
                    var msg = $"{i + 1}/{count + 1}\tProcessing {fileNameList[i]}\t ";
                    Console.Write(msg);
                    sw.Write(msg);
                    var path = Path.Combine(pathOutput, fileNameList[i]);
                    var lines = File.ReadLines(path);
                    var lineCount = lines.Count();
                    msg = $"Total lines: {lineCount}\t";
                    Console.Write(msg);
                    sw.Write(msg);
                    var uniqueLineCount = lines.Distinct().Count();
                    msg = $"Unique: {uniqueLineCount}";
                    Console.WriteLine(msg);
                    sw.WriteLine(msg);
                }
            }
            
        }

        private static void drawTextProgressBar(int progress, int total)
        {
            //draw empty progress bar
            Console.CursorLeft = 0;
            Console.Write("["); //start
            Console.CursorLeft = 32;
            Console.Write("]"); //end
            Console.CursorLeft = 1;
            float onechunk = 30.0f / total;

            //draw filled part
            int position = 1;
            for (int i = 0; i < onechunk * progress; i++)
            {
                Console.BackgroundColor = ConsoleColor.Green;
                Console.CursorLeft = position++;
                Console.Write(" ");
            }

            //draw unfilled part
            for (int i = position; i <= 31; i++)
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.CursorLeft = position++;
                Console.Write(" ");
            }

            //draw totals
            Console.CursorLeft = 35;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write(progress.ToString() + " of " + total.ToString() + "    "); //blanks at the end remove any excess
        }
    }
}
