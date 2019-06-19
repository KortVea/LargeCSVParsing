using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ConsoleUI
{
    public class CSVFilesWriteDispatcher : IDisposable
    {
        private Dictionary<string, StreamWriter> _writerByMonthDic = new Dictionary<string, StreamWriter>();
        private readonly string _outputPath;
        public CSVFilesWriteDispatcher(string path)
        {
            _outputPath = path ?? throw new Exception("Output path can't be null");
        }
        public void WriteToCsvFilesAccordingToMonth(ReportModel item)
        {
            var fileName = $"{item.Date.Year}-{item.Date.Month}.csv";
            if (!_writerByMonthDic.ContainsKey(fileName))
            {
                var writer = new StreamWriter(Path.Combine(_outputPath, fileName));
                _writerByMonthDic.Add(fileName, writer);
            }
            WriteOut(fileName, item);
        }

        private void WriteOut(string fileName, ReportModel item)
        {
            var writer = _writerByMonthDic[fileName];
            writer.WriteLine(item.ToString());
        }

        public List<string> GetNamesOfGeneratedFile()
        {
            return _writerByMonthDic.Keys.ToList();
        }

        public void Dispose()
        {
            foreach (var item in _writerByMonthDic)
            {
                item.Value.Dispose();
            }
            _writerByMonthDic = null;
        }
    }
}
