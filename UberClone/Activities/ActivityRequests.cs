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
using UberClone.Helpers;
using System.Net.Http;
using System.Globalization;

namespace UberClone.Activities
{
    [Activity(Label = "ActivityRequests",ScreenOrientation =Android.Content.PM.ScreenOrientation.Portrait)]
    public class ActivityRequests : FragmentActivity ,Android.Locations.ILocationListener,Android.Gms.Maps.IOnMapReadyCallback
    {
        GoogleMap mMap;
        LocationManager locationmanager;
        string provider;
        Location location;

        List<Marker> markers = new List<Marker>();
        List<Request> List_Request = new List<Request>();

        LatLngBounds.Builder builder;

        Handler handler = new Handler();
        Action act;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Layout_ActivityRequests);


            SetupMap();

            act = new Action(UpdateMap);
        }

        private void UpdateMap()
        {
            location = locationmanager.GetLastKnownLocation(provider);
            if (location != null)
            {
                RefreshMapMarkers();
                SetMyLocation();
            }
            handler.PostDelayed(new Java.Lang.Runnable(act), 5000);
        }

        private async void RefreshMapMarkers()
        {
            mMap.Clear();
            markers.Clear();
            builder = new LatLngBounds.Builder();

            


            Tuple<List<Request>, bool, string> requests = await RestHelper.APIRequest<List<Request>>(AppUrls.api_url_GetRequestsWithoutDriver, HttpVerbs.GET, null, null);
            if (requests.Item1.Count>0)
            {
                List_Request.Clear();
                foreach (Request i in requests.Item1)
                {
                    if (string.IsNullOrEmpty(i.driver_usename))
                    {
                        LatLng driverlatlng = new LatLng(location.Latitude, location.Longitude);
                        markers.Add(mMap.AddMarker(new MarkerOptions()
                            .SetIcon(BitmapDescriptorFactory
                            .DefaultMarker(BitmapDescriptorFactory.HueRed))
                            .SetPosition(driverlatlng)
                            .SetTitle("MyLocation")));

                        double distanceinkm = GeoDistanceHelper.DistanceBetweenPlaces(location.Longitude, location.Latitude, i.requester_longitude, i.requester_latitude);

                        //  List_Request.Add(distanceinkm.ToString() + " Km");
                        List_Request.Add(new Request
                        {
                            request_id = i.request_id,
                            requester_username = i.requester_username,
                            driver_usename = i.driver_usename,
                            requester_longitude = i.requester_longitude,
                            requester_latitude = i.requester_latitude
                        });
                        
                        if (i.requester_latitude != 0 & i.requester_longitude != 0)
                        {
                            LatLng riderlatlng = new LatLng(i.requester_latitude, i.requester_longitude);
                            markers.Add(mMap.AddMarker(new MarkerOptions()
                                .SetIcon(BitmapDescriptorFactory
                                .DefaultMarker(BitmapDescriptorFactory.HueOrange))
                                .SetPosition(riderlatlng)
                                .SetTitle(i.requester_username)));
                        }

                        if (markers.Count > 0)
                        {
                            foreach (Marker m in markers)
                            {
                                builder.Include(m.Position);
                            }

                        }
                        LatLngBounds bounds = builder.Build();
                        CameraUpdate cu = CameraUpdateFactory.NewLatLngBounds(bounds, 100);
                        mMap.MoveCamera(cu);
                    }
                }
            }
            else
            {
                LatLng driverlatlng = new LatLng(location.Latitude, location.Longitude);
                markers.Add(mMap.AddMarker(new MarkerOptions()
                    .SetIcon(BitmapDescriptorFactory
                    .DefaultMarker(BitmapDescriptorFactory.HueRed))
                    .SetPosition(driverlatlng)
                    .SetTitle("MyLocation")));
                CameraUpdate camera = CameraUpdateFactory.NewLatLngZoom(driverlatlng, 18);
                mMap.MoveCamera(camera);

                Toast.MakeText(this, "No Requests Found", ToastLength.Short).Show();
            }

        }
        private async void SetMyLocation()
        {
            string myLong = location.Longitude.ToString(CultureInfo.InvariantCulture);
            string myLat = location.Latitude.ToString(CultureInfo.InvariantCulture);

            FormUrlEncodedContent paramss = new FormUrlEncodedContent(new[] {
                 new KeyValuePair<string, string>("user_id",Settings.User_ID),
                 new KeyValuePair<string, string>("username",Settings.Username),
                 new KeyValuePair<string, string>("usertype",Settings.Usertype),
                 new KeyValuePair<string, string>("user_longitude",myLong),
                 new KeyValuePair<string, string>("user_latitude",myLat)});

            Tuple<User, bool, string> result = await RestHelper.APIRequest<User>(AppUrls.api_url_users + Settings.User_ID, HttpVerbs.POST, null, paramss);
            if (!result.Item2)
            {
                Toast.MakeText(this, "Update Location Error", ToastLength.Short).Show();
            }
            if (result.Item2)
            {
                Toast.MakeText(this, "Location Updated", ToastLength.Short).Show();
            }

        }

        public void OnMapReady(GoogleMap googleMap)
        {
            this.mMap = googleMap;
            locationmanager = (LocationManager)GetSystemService(Context.LocationService);
            provider = locationmanager.GetBestProvider(new Criteria(), false);
            locationmanager.RequestLocationUpdates(provider, 100, 1, this);
            UpdateMap();

        }
        public void OnLocationChanged(Location location)
        {
            location = locationmanager.GetLastKnownLocation(provider);
        }

        public void OnProviderDisabled(string provider)
        {
            Toast.MakeText(this, "OnProviderDisabled", ToastLength.Short).Show();
        }

        public void OnProviderEnabled(string provider)
        {
            Toast.MakeText(this, "OnProviderEnabled", ToastLength.Short).Show();
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            Toast.MakeText(this, "OnStatusChanged", ToastLength.Short).Show();
        }
        private void SetupMap()
        {
            if (mMap == null)
            {
                (SupportFragmentManager.FindFragmentById(Resource.Id.fragment_ActivityRequests) as SupportMapFragment).GetMapAsync(this);
            }
        }

 
    }
}