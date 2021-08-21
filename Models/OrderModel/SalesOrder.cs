using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Smart_Tailoring_WebAPI.Models.OrderModel
{
    public class SalesOrder
    {
        public int CustomerID { get; set; }
        public string OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime TrailDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal OrderAmount { get; set; }
        public decimal CGST { get; set; }
        public decimal SGST { get; set; }
        public int OrderQTY { get; set; }
        public string OrderMode { get; set; }
        public int CreatedBy { get; set; }
    }
}