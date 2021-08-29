using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Smart_Tailoring_WebAPI.Models.OrderModel
{
    public class clsStyle
    {
        public int StyleID { get; set; }
        public string StyleName { get; set; }
        public bool IsMandatory { get; set; }
        public List<clsImageList> lstImage { get; set; }
    }
}