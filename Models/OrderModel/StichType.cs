using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Smart_Tailoring_WebAPI.Models.OrderModel
{
    public class StitchType
    {
        public int StichTypeID { get; set; }
        public string StichTypeName { get; set; }
        public int LastChange { get; set; }
    }
}