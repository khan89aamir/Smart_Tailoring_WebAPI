﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Smart_Tailoring_WebAPI.Models
{
    public class Customer
    {
        public int MB_CustomerID { get; set; }
        public int CustomerID { get; set; }
        public string Name { get; set; }
        public string MobileNo { get; set; }

        public string EmailID { get; set; }

        public string Address { get; set; }

        public string LastChange { get; set; }
    }
}