﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Smart_Tailoring_WebAPI
{
    public class clsMeasurment
    {  
        public int MeasurmentID { get; set; }
        public string MeasurmentName { get; set; }
        public bool IsMandatory { get; set; }

        public string value { get; set; }
    }
}