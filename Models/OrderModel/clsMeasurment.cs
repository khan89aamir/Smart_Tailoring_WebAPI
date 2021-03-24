using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Smart_Tailoring_WebAPI.Models
{
    public class clsMeasurment
    {
        public int MeasurmentID { get; set; }
        public string MeasurmentName { get; set; }
        public bool IsMendatory { get; set; }

        public decimal value { get; set; }
    }
}