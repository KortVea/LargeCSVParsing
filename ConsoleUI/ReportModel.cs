using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleUI
{
    public class ReportModel
    {
        public string AssetId { get; set; }
        public string EquipmentId { get; set; }
        public DateTime Date { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public bool IsValid { get; set; }
        public ReportModel(){}

        //line format:
        //"Vehicle","DateTime_UTC","latitude","longitude","speed","heading","mileage"
        //"Democase - Anthony","2018-05-01 01:38:23.241000",-3378431,15113032,0,0,2438068
        public ReportModel(string line, SortedDictionary<string, string> refDic)
        {
            if (line == null || refDic == null) return;

            var components = line.Split(',');
            if (components.Length <= 3) return;

            var equipmentId = components[0].Trim('"');
            var date = components[1].Trim('"');
            float lat = 0, lng = 0;
            float.TryParse(components[2], out lat);
            lat /= 100000;
            float.TryParse(components[3], out lng);
            lng /= 100000;
            if (!refDic.ContainsKey(equipmentId)) return;

            AssetId = refDic[equipmentId];
            EquipmentId = equipmentId;
            Date = DateTime.Parse(date);
            Latitude = lat;
            Longitude = lng;

            IsValid = true;
        }

        public override string ToString()
        {
            return $@"""{AssetId}"",""{EquipmentId}"",""{Date:O}"",""{Latitude}"",""{Longitude}""";
        }
    }
}
