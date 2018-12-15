using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ESPAdmin.Models
{
    public class AdminViews
    {
        [Key]
        public string ID { get; set; }
        public string MenuID { get; set; }
        public string ParentMenuID { get; set; }
        public string ParentMenuName { get; set; }
        public string MenuName { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public bool Checked { get; set; }
    }
}