using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Smart_Tailoring_WebAPI.Models.OrderModel
{
    public class clsGarmentRate
    {
        public int GarmentRateID { get; set; }
        public int GarmentID { get; set; }
        public string GarmentCode { get; set; }
        public string GarmentName { get; set; }
        public string GarmentCodeName { get; set; }
        public string GarmentType { get; set; }
        public double Rate { get; set; }
        public string OrderType { get; set; }
        public int LastChange { get; set; }
    }
}