using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Smart_Tailoring_WebAPI.Models
{
    public class ActivationDetails
    {
        public int ID { get; set; }
        public string ServerIP { get; set; }
        public string ActivationCode { get; set; }

        public string DeviceSerialNumber { get; set; }
    }
}