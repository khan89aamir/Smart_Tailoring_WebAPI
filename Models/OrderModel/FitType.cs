using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Smart_Tailoring_WebAPI.Models.OrderModel
{
    public class FitType
    {
        public int FitTypeID { get; set; }
        public string FitTypeName { get; set; }
        public int LastChange { get; set; }
    }
}