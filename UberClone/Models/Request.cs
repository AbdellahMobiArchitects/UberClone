using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace UberClone.Models
{
    public class Request
    {
        public int Requestid { get; set; }
        public string Requester_username { get; set; }
        public string Driver_username { get; set; }
        public double Requester_lat { get; set; }
        public double Requester_long { get; set; }
    }
}