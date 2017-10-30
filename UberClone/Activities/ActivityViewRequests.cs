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
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Specialized;
using Newtonsoft.Json;
using UberClone.Models;
using UberClone.Helpers;
using Android.Locations;
using Android.Util;
using Android.Gms.Maps.Model;

namespace UberClone.Activities
{
    [Activity(Label = "ViewRequests", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait,
        LaunchMode = Android.Content.PM.LaunchMode.SingleTop)]
    public class ActivityViewRequests : Activity, ILocationListener
    {
        ListView lvrequests;
        LocationManager locationmanager;
        string provider;
        Location location;
        List<string> list_requests = new List<string>();
        Button button_request;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                SetContentView(Resource.Layout.Layout_ViewRequests);
                
                //setting up variables + events
                lvrequests = FindViewById<ListView>(Resource.Id.listview_requests);
                button_request = FindViewById<Button>(Resource.Id.button_request);

                button_request.Click += button_request_Click;

                //setting up location stuff
                locationmanager = (LocationManager)GetSystemService(Context.LocationService);
                provider = locationmanager.GetBestProvider(new Criteria(), false);
                locationmanager.RequestLocationUpdates(provider, 400, 1, this);


                #region call getrequests & populate listview
                //code
                //var requests = await RestRequestType.APIRequest<List<Request>>(AppUrls.api_url_requests, HttpVerbs.GET, null, null);
                //if (requests.Count>0)
                //{
                //    list_requests.Clear();
                //    foreach (var i in requests)
                //    {
                //        if (string.IsNullOrEmpty(i.driver_usename))
                //        {
                //            var distanceinkm = GeoDistanceHelper.DistanceBetweenPlaces(location.Longitude, location.Latitude, i.requester_longitude, i.requester_latitude);
                //            list_requests.Add(distanceinkm.ToString()+" Km");
                //        }
                //    }
                //    Android.Util.Log.Info("UberCloneApp.ListRequests", list_requests.ToString());
                //var ListAdapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItem1, list_requests);
                //  lvrequests.Adapter = ListAdapter;
                //}
                //else
                //{
                //    Android.App.AlertDialog.Builder dialog = new Android.App.AlertDialog.Builder(this);
                //    Android.App.AlertDialog alert = dialog.Create();
                //    alert.SetTitle("Information!");
                //    alert.SetMessage("No Requests");
                //    alert.SetIcon(Resource.Drawable.alert);
                //    alert.SetButton("OK", (c, ev) =>
                //    {

                //    });
                //    alert.Show();
                //}
                #endregion


            }
            catch (Exception e)
            {
                Android.Util.Log.Info("UberCloneApp.ActivityViewRequests.OnCreate", e.InnerException.Message);
            }
           
            

        }

        private void button_request_Click(object sender, EventArgs e)
        {
            RedirectUserExtra(typeof(ActivityViewRiderLocation));
        }
        private void RedirectUserExtra(Type activity)
        {
            Intent i = new Intent(this, activity);
            i.PutExtra("username","nitro2010");
            i.PutExtra("latitude", 33.542660);
            i.PutExtra("longitude", -7.629246);
            this.StartActivity(i);
        }
        private void UpdateLocation()
        {
            location = locationmanager.GetLastKnownLocation(provider);

        }
        public void OnLocationChanged(Location location)
        {
            UpdateLocation();
            Android.Util.Log.Info("UberCloneApp", "Location Changed");

        }
        public void OnProviderDisabled(string provider)
        {
            Android.Util.Log.Info("UberCloneApp", "Provider Disabled");
        }

        public void OnProviderEnabled(string provider)
        {
            Android.Util.Log.Info("UberCloneApp", "Provider Enabled");
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            Android.Util.Log.Info("UberCloneApp", "Status Changed");
        }
    }
}