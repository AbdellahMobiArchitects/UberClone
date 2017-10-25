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
        public int request_id { get; set; }
        public string requester_username { get; set; }
        public double requester_longitude { get; set; }
        public double requester_latitude { get; set; }
        public string driver_usename { get; set; }

        public override string ToString()
        {
            return (
                "Request ID" + request_id +
                "Requester Username" + requester_username +
                "Requester Longitude" + requester_longitude +
                "Requester Latitude" + requester_latitude +
                "Driver Name" + driver_usename
                );
        }
    }
}