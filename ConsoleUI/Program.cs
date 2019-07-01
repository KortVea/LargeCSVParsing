using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using TrimbleDataCSVProcessor;

namespace ConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {
            //Source Format 1 Illustration:
            //  "Vehicle","DateTime_UTC","latitude","longitude","speed","heading","mileage"
            //  "Democase - Anthony","2018-05-01 00:12:36.488000",-3378421,15113029,0,196,2438068

            //var pathSource = @"C:\Users\Yishi_Liu\Documents\TTLDOG-1517.csv";
            //var pathRef = @"C:\Users\Yishi_Liu\Documents\AU-AssetList.xlsx";
            //var pathOutput = @"C:\Users\Yishi_Liu\Documents\CSVTEST";
            //ProcessFormat1(pathSource, pathOutput, pathRef);

            //Source Format 2 Illustration:
            //  "Ref(AssetId)","Vehicle(EquipmentId)","DateTime_UTC","Latitude","Longitude"
            //  "BM113,13139026,ENSH","BM113","2014-12-31 23:00:34.029000",-2185689,14842776

            var pathSourceFolder = @"C:\Users\Yishi_Liu\Downloads\1unzip";//folder path containing all the .csv to parse. Search through all subfolders.
            ProcessFormat2(pathSourceFolder);

            Console.ReadLine();
        }

        private static void ProcessFormat2(string pathSourceFolder)
        {
            var pathResult = Path.Combine(pathSourceFolder, "Result");
            if (Directory.Exists(pathResult))
            {
                try
                {
                    Directory.Delete(pathResult);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
            Directory.CreateDirectory(pathResult);

            var files = Directory.EnumerateFiles(pathSourceFolder, "*.csv", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                Console.WriteLine($"Processing {file}");
                ProcessFormat1(file, pathResult);
            }

        }

        private static void ProcessFormat1(string pathSource, string pathOutput, string pathRef = null)
        {
            var refDic = new SortedDictionary<string, string>();
            if (pathRef != null)
            {
                Console.Write($"Reading {pathRef} ...");
                refDic = new CsvProcessor().ParseIntoDictionaryFromFile(pathRef);
                Console.WriteLine("Done.");
            }

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
                    var item = refDic.Count == 0? new ReportModel(line) : new ReportModel(line, refDic);
                    if (item.IsValid)
                    {
                        writer.WriteToCsvFilesAccordingToMonth(item);
                    }
                    else
                    {
                        errorCount++;
                    }
                    if (index % 10000 == 0)
                    {
                        DrawTextProgressBar(index, totalCount);
                    }
                }
                DrawTextProgressBar(index, totalCount);

                Console.WriteLine("Generating Report...");
                fileNameList = writer.GetNamesOfGeneratedFile();
                Console.WriteLine("Done.");
            }

            sw.Stop();
            Console.WriteLine("Total Elapsed Time: " + sw.Elapsed);
            Console.WriteLine("Error Count: " + errorCount);

            sw.Restart();
            Console.WriteLine("Generating Report ...");
            GenerateFileReport(fileNameList, pathOutput);
            sw.Stop();
            Console.WriteLine("Total Elapsed Time: " + sw.Elapsed);
        }

        private static void GenerateFileReport(List<string> fileNameList, string pathOutput)
        {
            var reportPath = Path.Combine(pathOutput, $"Report-{DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss", CultureInfo.InvariantCulture)}.txt");
            using (var sw = new StreamWriter(reportPath))
            {
                var count = fileNameList.Count();
                for (var i = 0; i < count; i++)
                {
                    var msg = $"{i + 1}/{count}\tProcessing {fileNameList[i]}\t ";
                    Console.Write(msg);
                    sw.Write(msg);
                    var path = Path.Combine(pathOutput, fileNameList[i]);
                    var lines = File.ReadLines(path);

                    var lineCount = lines.Count();
                    msg = $"Total lines: {lineCount}\t";
                    Console.Write(msg);
                    sw.Write(msg);

                    var uniqueLineCount = lines.Select(l => l.Split(',')[1]).Distinct().Count();
                    msg = $"Unique equipmentId count: {uniqueLineCount}";
                    Console.WriteLine(msg);
                    sw.WriteLine(msg);
                }
            }

        }

        private static void DrawTextProgressBar(int progress, int total)
        {
            //draw empty progress bar
            Console.CursorLeft = 0;
            Console.Write("["); //start
            Console.CursorLeft = 32;
            Console.Write("]"); //end
            Console.CursorLeft = 1;
            var oneChunk = 30.0f / total;

            //draw filled part
            var position = 1;
            for (var i = 0; i < oneChunk * progress; i++)
            {
                Console.BackgroundColor = ConsoleColor.Green;
                Console.CursorLeft = position++;
                Console.Write(" ");
            }

            //draw unfilled part
            for (var i = position; i <= 31; i++)
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.CursorLeft = position++;
                Console.Write(" ");
            }

            //draw totals
            Console.CursorLeft = 35;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write(progress + " of " + total + "    "); //blanks at the end remove any excess
        }
    }
}
