using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Smart_Tailoring_WebAPI.Models.OrderModel
{
    public class CustomerStyle
    {
        public int SalesOrderID { get; set; }
        public int MasterGarmentID { get; set; }
        public int GarmentID { get; set; }
        public int StyleID { get; set; }
        public int QTY { get; set; }
        public int StyleImageID { get; set; }
        public int CreatedBy { get; set; }
    }
}