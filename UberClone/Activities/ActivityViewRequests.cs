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
using Android.Support.V4.App;
using Android.Gms.Maps.Model;
using Android.Gms.Maps;

namespace UberClone.Activities
{
    [Activity(Label = "ViewRequests", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait,
        LaunchMode = Android.Content.PM.LaunchMode.SingleTop)]
    public class ActivityViewRequests : Activity, ILocationListener
    {
        GoogleMap mMap;
        ListView lvrequests;
        LocationManager locationmanager;
        string provider;
        Location location;

        List<string> list_requests = new List<string>();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                SetContentView(Resource.Layout.Layout_ViewRequests);
                lvrequests = FindViewById<ListView>(Resource.Id.listview_requests);

                locationmanager = (LocationManager)GetSystemService(Context.LocationService);
                provider = locationmanager.GetBestProvider(new Criteria(), false);
                locationmanager.RequestLocationUpdates(provider, 400, 1, this);
                location = locationmanager.GetLastKnownLocation(provider);

                FillListView();
            }
            catch (Exception e)
            {
                Toast.MakeText(this, e.InnerException.Message, ToastLength.Short);
            }
        }

        private async void FillListView()
        {
            location = locationmanager.GetLastKnownLocation(provider);
            var requests = await RestHelper.APIRequest<List<Request>>(AppUrls.api_url_GetRequestsWithoutDriver, HttpVerbs.GET, null, null);
            if (requests.Item1.Count > 0)
            {
                list_requests.Clear();
                foreach (var i in requests.Item1)
                {
                    if (string.IsNullOrEmpty(i.driver_usename))
                    {
                        var distanceinkm = GeoDistanceHelper.DistanceBetweenPlaces(location.Longitude, location.Latitude, i.requester_longitude, i.requester_latitude);
                        list_requests.Add(distanceinkm.ToString() + " Km");
                    }
                }
                var ListAdapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItem2, list_requests);
                lvrequests.Adapter = ListAdapter;
            }
            else
            {
                Toast.MakeText(this, "No Requests", ToastLength.Short);
            }

        }
        //private void RedirectUserExtra(Type activity)
        //{
        //    Intent i = new Intent(this, activity);
        //    i.PutExtra("username","nitro2010");
        //    i.PutExtra("latitude", 33.542660);
        //    i.PutExtra("longitude", -7.629246);
        //    this.StartActivity(i);
        //}
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