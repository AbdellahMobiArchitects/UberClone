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
                "Request ID: " + request_id + "\n" +
                "Requester Username: " + requester_username + "\n" +
                "Requester Longitude: " + requester_longitude + "\n" +
                "Requester Latitude: " + requester_latitude + "\n" +
                "Driver Name: " + driver_usename
                );
        }
    }
}