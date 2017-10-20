using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace UberClone.Web.Models
{
    public class Request
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Requestid { get; set; }
        public string Requester_username { get; set; }
        public string Driver_username { get; set; }
        public double Requester_lat { get; set; }
        public double Requester_long { get; set; }
    }
}