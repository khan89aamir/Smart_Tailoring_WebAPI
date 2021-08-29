using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Smart_Tailoring_WebAPI.Models
{
    public class clsImageList
    {
        public int GarmentID { get; set; }
        public int StyleID { get; set; }
        public int BodyPostureID { get; set; }
        public string ImageURL { get; set; }
    }
}