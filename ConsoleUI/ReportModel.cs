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

        //line format1:
        //"Vehicle","DateTime_UTC","latitude","longitude","speed","heading","mileage"
        //"Democase - Anthony","2018-05-01 01:38:23.241000",-3378431,15113032,0,0,2438068
        public ReportModel(string line, SortedDictionary<string, string> refDic)
        {
            if (line == null || refDic == null) return;

            var components = line.Split(',');
            if (components.Length <= 3) return;

            var equipmentId = components[0].Trim('"');
            var date = components[1].Trim('"');
            float.TryParse(components[2], out var lat);
            lat /= 100000;
            float.TryParse(components[3], out var lng);
            lng /= 100000;
            if (!refDic.ContainsKey(equipmentId)) return;

            AssetId = refDic[equipmentId];
            EquipmentId = equipmentId;
            Date = DateTime.Parse(date);
            Latitude = lat;
            Longitude = lng;

            IsValid = true;
        }

        // line format2:
        // "Ref(AssetId)","Vehicle(EquipmentId)","DateTime_UTC","Latitude","Longitude"
        // "BM113,13139026,ENSH","BM113","2014-12-31 23:00:34.029000",-2185689,14842776
        public ReportModel(string line)
        {
            if (line == null) return;
            var components = line.Split(',');
            if (components.Length < 7) return;

            var equipmentId = components[0].Trim('"');
            var date = components[4].Trim('"');
            float.TryParse(components[5], out var lat);
            lat /= 100000;
            float.TryParse(components[6], out var lng);
            lng /= 100000;

            AssetId = components[1];
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
