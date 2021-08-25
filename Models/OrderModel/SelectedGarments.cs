using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Smart_Tailoring_WebAPI.Models.OrderModel
{
    public class SelectedGarments
    {
        public int GarmentID { get; set; }
        public string GarmentCode { get; set; }
        public string Name { get; set; }
        public string Photo { get; set; }
        public int QTY { get; set; }

        public bool IsTrail { get; set; }
        public DateTime TrailDate { get; set; }

        public DateTime DeliveryDate { get; set; }
        public double Rate { get; set; }
        public double TrimAmount { get; set; }
        public string OrderType { get; set; }
        public int OrderTypeID { get; set; }

        public List<clsGarmentRate> lstGarmentRate { get; set; }
    }
}