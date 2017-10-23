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
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.Support.V7.App;
using Android.Support.V4.App;
using Newtonsoft.Json;


namespace UberClone.Activities
{

    #region AlternativeGoogleMapsCode
    //[Activity(Label = "ActivityYourLocation")]
    //public class ActivityYourLocation : Activity, IOnMapReadyCallback
    //{
    //    private GoogleMap GMap;
    //    protected override void OnCreate(Bundle savedInstanceState)
    //    {
    //        base.OnCreate(savedInstanceState);

    //        try
    //        {
    //            // Create your application here
    //            SetContentView(Resource.Layout.Layout_ActivityYourLocation);
    //            SetUpMap();
    //        }
    //        catch (Exception e)
    //        {
    //            Toast.MakeText(this, "Something's Wrong: " + e.InnerException.Message, ToastLength.Long).Show();
    //        }
    //    }

    //    private void SetUpMap()
    //    {
    //        if (GMap == null)
    //        {
    //            FragmentManager.FindFragmentById<MapFragment>(Resource.Id.fragment_googlemaps).GetMapAsync(this);
    //        }
    //    }

    //    public void OnMapReady(GoogleMap googleMap)
    //    {
    //        try
    //        {
    //            this.GMap = googleMap;
    //            LatLng latlng = new LatLng(Convert.ToDouble(33.54288), Convert.ToDouble(-7.64029));
    //            CameraUpdate camera = CameraUpdateFactory.NewLatLngZoom(latlng, 15);
    //            GMap.MoveCamera(camera);
    //            MarkerOptions options = new MarkerOptions().SetPosition(latlng).SetTitle("My Location");
    //            GMap.AddMarker(options);
    //        }
    //        catch (Exception e)
    //        {
    //            Toast.MakeText(this, "Something's Wrong In OnMapReady: " + e.InnerException.Message, ToastLength.Long).Show();
    //        }
    //    }
    //}
    #endregion

    [Activity(Label = "ActivityYourLocation")]
    public class ActivityYourLocation : FragmentActivity, ILocationListener ,IOnMapReadyCallback
    {
        
         private GoogleMap mMap; //Null if google apk services isn't available...
         public LocationManager locationmanager;
         public  string provider;
         public Location location;

         Button requestuber;
         TextView tvinfo;

         bool requestactive = false;
         protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Layout_ActivityYourLocation);
            SetUpMapIfNeeded();
            requestuber = FindViewById<Button>(Resource.Id.button_requestuber);
            requestuber.Click += Requestuber_Click;
            tvinfo = FindViewById<TextView>(Resource.Id.textviewinfo);
        }
        private void SetUpMapIfNeeded()
        {
            // Do a null check to confirm that we have not already instantiated the map.
            if (mMap == null)
            {
                var frag = (SupportFragmentManager.FindFragmentById(Resource.Id.fragment_googlemap) as SupportMapFragment);
                frag.GetMapAsync(this);

            }
        }
        public void OnMapReady(GoogleMap googleMap)
        {
            this.mMap = googleMap;
            locationmanager = (LocationManager)GetSystemService(Context.LocationService);
            provider = locationmanager.GetBestProvider(new Criteria(), false);
            locationmanager.RequestLocationUpdates(provider, 400, 1, this);
            location = locationmanager.GetLastKnownLocation(provider);
            Toast.MakeText(this, "Map Ready", ToastLength.Short).Show();

            UpdateLocation();
        }
        private void UpdateLocation()
        {
                mMap.Clear();
                var newloc= locationmanager.GetLastKnownLocation(provider);
                LatLng latlng = new LatLng(newloc.Latitude, newloc.Longitude);
                MarkerOptions options = new MarkerOptions().SetPosition(latlng).SetTitle("MyLocation");
                mMap.AddMarker(options);
                CameraUpdate camera = CameraUpdateFactory.NewLatLngZoom(latlng, 5);
                mMap.MoveCamera(camera);
        }
       

        public void OnLocationChanged(Location location)
        {
            Toast.MakeText(this, "Location Changed", ToastLength.Long).Show();
            UpdateLocation();
            
        }

        public void OnProviderDisabled(string provider)
        {
            Toast.MakeText(this, "Provider Disabled", ToastLength.Long).Show();
        }

        public void OnProviderEnabled(string provider)
        {
            Toast.MakeText(this, "Provider Enabled", ToastLength.Long).Show();
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            Toast.MakeText(this, "Status Changed", ToastLength.Long).Show();
        }

       

        private void Requestuber_Click(object sender, EventArgs e)
        {
            if (!requestactive)
            {
                Android.Util.Log.Info("My App", "Uber Requested");
                tvinfo.Text = "Finding UberDriver...";
                requestuber.Text = "Cancel Uber";
                requestactive = true;
                /*creating classes in parser + create user with each requestuberclick*/
            }
            else
            {
                Android.Util.Log.Info("My App", "Uber Cancelled");
                tvinfo.Text = "";
                requestuber.Text = "Request Uber";
                requestactive = false;
            }


        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        protected override void OnResume()
        {
            base.OnResume();
        }
    }
}