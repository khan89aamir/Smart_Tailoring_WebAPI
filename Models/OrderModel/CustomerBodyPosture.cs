using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Smart_Tailoring_WebAPI.Models.OrderModel
{
    public class CustomerBodyPosture
    {
        public int SalesOrderID { get; set; }
        public int MasterGarmentID { get; set; }
        public int GarmentID { get; set; }
        public int BodyPostureID { get; set; }
        public int BodyPostureMappingID { get; set; }
        public int CreatedBy { get; set; }
    }
}