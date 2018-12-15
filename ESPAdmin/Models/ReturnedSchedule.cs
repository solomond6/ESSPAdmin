using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ESPAdmin.Models
{
    public class ReturnedSchedule
    {
        public string RequestID { get; set; }
        public string HistoryID { get; set; }
        public string EmployerCode { get; set; }
        public string ReturnDate { get; set; }
        public string Classification { get; set; }
        public string Amount_Returned { get; set; }
        public string Amount_on_Schedule { get; set; }
        public string Narration { get; set; }

        public string Comment { get; set; }
        public string Assignee { get; set; }
        public string Assignor { get; set; }
        public string AssignDate { get; set; }
        public string AssignStatus { get; set; }
    }
}