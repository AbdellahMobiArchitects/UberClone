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

using Android.Content.PM;
using System.Threading.Tasks;

namespace UberClone.Activities
{
    [Activity(Label = "Current Location",ScreenOrientation = ScreenOrientation.Portrait)]
    public class ActivityYourLocation : FragmentActivity, ILocationListener ,IOnMapReadyCallback
    {
        
          GoogleMap mMap; //Null if google apk services isn't available...
          LocationManager locationmanager;
          string provider;
          Location location;
          CameraUpdate camera;
          Button button_requestuber,button_zoomin,button_zoomout;
          TextView tvinfo;

         bool requestactive = false;
         protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Layout_ActivityYourLocation);
            SetUpMapIfNeeded();
            button_requestuber = FindViewById<Button>(Resource.Id.button_requestuber);
            button_zoomin = FindViewById<Button>(Resource.Id.button_zoomin);
            button_zoomout = FindViewById<Button>(Resource.Id.button_zoomout);


            tvinfo = FindViewById<TextView>(Resource.Id.textviewinfo);

            button_requestuber.Click += button_requestuber_Click;
            button_zoomin.Click += Button_zoomin_Click;
            button_zoomout.Click += Button_zoomout_Click;
        }

        private async void Button_zoomout_Click(object sender, EventArgs e)
        {
            var latlng = new LatLng(location.Latitude, location.Longitude);
            camera = CameraUpdateFactory.NewLatLng(latlng);
            mMap.MoveCamera(camera);
            await Task.Delay(500);
            camera = CameraUpdateFactory.ZoomOut();
            mMap.AnimateCamera(camera);
            


        }

        private async void Button_zoomin_Click(object sender, EventArgs e)
        {
            var latlng = new LatLng(location.Latitude, location.Longitude);
            camera = CameraUpdateFactory.NewLatLng(latlng);
            mMap.MoveCamera(camera);
            await Task.Delay(500);
            camera = CameraUpdateFactory.ZoomIn();
            mMap.AnimateCamera(camera);
            
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
            UpdateLocation();
        }
        private void UpdateLocation()
        {
                mMap.Clear();
                location = locationmanager.GetLastKnownLocation(provider);
                LatLng latlng = new LatLng(location.Latitude, location.Longitude);
               
                MarkerOptions options = new MarkerOptions().SetPosition(latlng).SetTitle("MyLocation");
                mMap.AddMarker(options);
                camera = CameraUpdateFactory.NewLatLngZoom(latlng, 18);
                mMap.MoveCamera(camera);
                Android.Util.Log.Info("UberCloneApp", "Rider Current Location:\nLongitude: " + location.Longitude + "\nLatitude: " + location.Latitude);

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

       

        private void button_requestuber_Click(object sender, EventArgs e)
        {
            if (!requestactive)
            {
                tvinfo.Text = "Finding UberDriver...";
                button_requestuber.Text = "Cancel Uber";
                requestactive = true;
                /*creating classes in parser + create user with each requestuberclick*/
                Android.Util.Log.Info("UberCloneApp", "Uber Requested");
            }
            else
            {
                tvinfo.Text = "";
                button_requestuber.Text = "Request Uber";
                requestactive = false;
                Android.Util.Log.Info("UberCloneApp", "Uber Cancelled");
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