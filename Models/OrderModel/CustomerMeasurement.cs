using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Smart_Tailoring_WebAPI.Models.OrderModel
{
    public class CustomerMeasurement
    {
        public int SalesOrderID { get; set; }
        public int MasterGarmentID { get; set; }
        public int GarmentID { get; set; }
        public int MeasurementID { get; set; }
        public decimal MeasurementValue { get; set; }
        public int CreatedBy { get; set; }
    }
}