using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Smart_Tailoring_WebAPI.Models.OrderModel
{
    public class GarmentList
    {
        public int GarmentID { get; set; }

        public string ImageURL { get; set; }

        public string Name { get; set; }
    }
}