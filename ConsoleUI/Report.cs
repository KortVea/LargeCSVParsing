﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ConsoleUI
{
    public class PerFileInfoModel
    {
        public string FileName { get; set; }
        public int TotalLineCount { get; set; }
        public int DistinctLineCount { get; set; }
    }
}