using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ConsoleApp1
{
    public class CSVFilesWriteDispatcher : IDisposable
    {
        private Dictionary<string, StreamWriter> writerByMonthDic = new Dictionary<string, StreamWriter>();
        private string outputPath;

        public CSVFilesWriteDispatcher(string path)
        {
            outputPath = path ?? throw new Exception("Output path can't be null");
        }
        public void WriteToCSVFilesAccordingToMonth(ReportModel item)
        {
            var fileName = $"{item.Date.Year}-{item.Date.Month}.csv";
            if (writerByMonthDic.ContainsKey(fileName))
            {
                WriteOut(fileName, item);
            }
            else
            {
                var writer = new StreamWriter(Path.Combine(outputPath, fileName));
                writerByMonthDic.Add(fileName, writer);
                WriteOut(fileName, item);
            }
        }

        private void WriteOut(string fileName, ReportModel item)
        {
            var writer = writerByMonthDic[fileName];
            writer.WriteLine(item.ToString());
        }

        public void Dispose()
        {
            foreach (var item in writerByMonthDic)
            {
                item.Value.Dispose();
            }
            writerByMonthDic = null;
        }
    }
}
