using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace Smart_Tailoring_WebAPI.Models
{
    public class Employee
    {
        public int EmpID { get; set; }
        public string EmployeeCode { get; set; }
        public string Name { get; set; }
        public string MobileNo { get; set; }
        public string Gender { get; set; }
        public DateTime DOB { get; set; }
        public Image Photo { get; set; }
        public string EmployeeType { get; set; }
        public string ActiveStatus { get; set; }
        public int LastChange { get; set; }
        public string Address { get; set; }
    }
}