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

using UberClone.Models;

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
            locationmanager = (LocationManager)GetSystemService(Context.LocationService);
            provider = locationmanager.GetBestProvider(new Criteria(), false);
            locationmanager.RequestLocationUpdates(provider, 400, 1, this);
            location = locationmanager.GetLastKnownLocation(provider);
            if (location != null & mMap!=null)
            {
                mMap.Clear();
                mMap.UiSettings.ZoomControlsEnabled = true;
                LatLng latlng = new LatLng(location.Latitude, location.Longitude);
                CameraUpdate camera = CameraUpdateFactory.NewLatLngZoom(latlng, 15);
                mMap.MoveCamera(camera);
                MarkerOptions options = new MarkerOptions().SetPosition(latlng).SetTitle("MyLocation");
                mMap.AddMarker(options);
            }


            requestuber = FindViewById<Button>(Resource.Id.button_requestuber);
            requestuber.Click += Requestuber_Click;
            tvinfo = FindViewById<TextView>(Resource.Id.textviewinfo);
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
                //ParseObject request = new ParseObject("Requests");

                //request.put("requesterUsername", ParseUser.getCurrentUser().getUsername());

                //ParseACL parseACL = new ParseACL();
                //parseACL.setPublicWriteAccess(true);
                //parseACL.setPublicReadAccess(true);
                //request.setACL(parseACL);
                //    request.saveInBackground(new SaveCallback() {
                //        @Override
                //        public void done(ParseException e)
                //    {

                //        if (e == null)
                //        {

                //            infoTextView.setText("Finding Uber driver...");
                //            requestUberButton.setText("Cancel Uber");
                //            requestActive = true;

                //        }

                //    }
                //});
            }
            else
            {
                Android.Util.Log.Info("My App", "Uber Cancelled");
                tvinfo.Text = "";
                requestuber.Text = "Request Uber";
                requestactive = false;
            }


        }

        protected override void OnResume()
        {
            base.OnResume();
            SetUpMapIfNeeded();
            locationmanager.RequestLocationUpdates(provider, 400, 1, this);
        }


        private void SetUpMapIfNeeded()
        {
            // Do a null check to confirm that we have not already instantiated the map.
            if (mMap == null)
            {
                (SupportFragmentManager.FindFragmentById(Resource.Id.fragment_googlemap) as SupportMapFragment).GetMapAsync(this);
   
            }
        }

        public void OnLocationChanged(Location location)
        {
            if (location != null & mMap != null)
            {
                mMap.Clear();
                //mMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(location.Latitude, location.Longitude), 10));
                //mMap.AddMarker(new MarkerOptions().SetPosition(new LatLng(location.Latitude, location.Longitude)).SetTitle("My Location"));
                mMap.UiSettings.ZoomControlsEnabled = true;
                LatLng latlng = new LatLng(location.Latitude, location.Longitude);
                CameraUpdate camera = CameraUpdateFactory.NewLatLngZoom(latlng, 15);
                mMap.MoveCamera(camera);
                MarkerOptions options = new MarkerOptions().SetPosition(latlng).SetTitle("MyLocation");
                mMap.AddMarker(options);
            }
        }

        public void OnProviderDisabled(string provider)
        {
            Toast.MakeText(this, "ProviderDisabled", ToastLength.Long).Show();
        }

        public void OnProviderEnabled(string provider)
        {
            Toast.MakeText(this, "ProviderEnabled", ToastLength.Long).Show();
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            Toast.MakeText(this, "StatusChanged", ToastLength.Long).Show();
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            this.mMap = googleMap;

        }

        protected override void OnPause()
        {
            base.OnPause();
            locationmanager.RemoveUpdates(this);
        }
    }
}