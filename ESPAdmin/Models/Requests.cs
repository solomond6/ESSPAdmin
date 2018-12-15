using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ESPAdmin.Models
{
    public class Requests
    {
        public string RequestID { get; set; }
        public string EmployerName { get; set; }
        public string Category { get; set; }
        public string Creator { get; set; }
        public string Comment { get; set; }
        public string lastmodifieddate { get; set; }
        public string LastModifiedByRoleID { get; set; }
        public string CreatorRoles { get; set; }
        public string LastModifierRole { get; set; }
        public string CurrentStatus { get; set; }
        public string CurrentAssignedToName { get; set; }
        public string Datecreated { get; set; }
        public string GroupOwner { get; set; }
        public string LastModifiedBy { get; set; }
        public string CurrentAssignedToID { get; set; }
    }
}