using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ESPAdmin.Models
{
    public class Audits
    {
        public string ID { get; set; }
        public string TABLE_NAME { get; set; }
        public string COLUMN_NAME { get; set; }
        public string OLD_VALUE { get; set; }
        public string NEW_VALUE { get; set; }
        public string RECORD_ID { get; set; }
        public string OLD_VALUE2 { get; set; }
        public string NEW_VALUE2 { get; set; }
        public string RECORD_ID2 { get; set; }
        public string RECORD_COLUMN { get; set; }
        public string CHANGED_BY { get; set; }
        public string DATE_CHANGED { get; set; }
        public string APPROVED_BY { get; set; }
        public string DATE_APPROVED { get; set; }
        public string STATUS { get; set; }
        public string CHANGE_TYPE { get; set; }
        public string COLUMN_TYPE { get; set; }
        public string RECORD_COLUMN_TYPE { get; set; }
        public string NOTE { get; set; }
    }
}