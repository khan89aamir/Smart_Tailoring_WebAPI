using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Smart_Tailoring_WebAPI.Models
{
    public class Response
    {   
        public int Value { get; set; }
        public object Value2 { get; set; }
        public string Message { get; set; }
        public bool Result { get; set; }
    }
}