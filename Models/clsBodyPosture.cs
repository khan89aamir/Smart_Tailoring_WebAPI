﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Smart_Tailoring_WebAPI.Models
{
    public class clsBodyPosture
    {
        public int BodyPostureID { get; set; }
        public string BodyPostureType { get; set; }
        public List<clsImageList> lstImage { get; set; }
    }
}