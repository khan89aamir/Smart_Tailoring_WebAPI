using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Smart_Tailoring_WebAPI.Models.OrderModel
{
    public class CustomerStichFitType
    {
        public int SalesOrderID { get; set; }
        public int MasterGarmentID { get; set; }
        public int GarmentID { get; set; }
        public int StichTypeID { get; set; }
        public int FitTypeID { get; set; }
        public string OrderDate { get; set; }
    }
}