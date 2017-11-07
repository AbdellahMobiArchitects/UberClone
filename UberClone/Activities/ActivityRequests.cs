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
using Newtonsoft.Json;
using System.Net.Http;
using System.Globalization;
using Java.Util;
using Android.Graphics;
using System.Threading.Tasks;

namespace UberClone.Activities
{
    [Activity(Label = "ActivityRequests",ScreenOrientation =Android.Content.PM.ScreenOrientation.Portrait)]
    public class ActivityRequests : FragmentActivity ,Android.Locations.ILocationListener,Android.Gms.Maps.IOnMapReadyCallback, GoogleMap.IOnMarkerClickListener
    {
        GoogleMap mMap;
        LocationManager locationmanager;
        string provider;
        Location location;

        List<Marker> markers = new List<Marker>();
        List<Request> List_Request = new List<Request>();
        Dictionary<Marker, Request> myMarkers = new Dictionary<Marker, Request>();

        Android.Gms.Maps.Model.Polyline myPolyline;

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
                markers.Clear();
                myMarkers.Clear();
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
                        
                        if (i.requester_latitude != 0 & i.requester_longitude != 0)
                        {
                            double distanceinkm = GeoDistanceHelper.DistanceBetweenPlaces(location.Longitude, location.Latitude, i.requester_longitude, i.requester_latitude);

                            LatLng riderlatlng = new LatLng(i.requester_latitude, i.requester_longitude);

                            Marker marker = mMap.AddMarker(new MarkerOptions()
                                .SetIcon(BitmapDescriptorFactory
                                .DefaultMarker(BitmapDescriptorFactory.HueOrange))
                                .SetPosition(riderlatlng)
                                .SetTitle(i.requester_username));
                            
                            Request Req = new Request
                            {
                                request_id = i.request_id,
                                requester_username = i.requester_username,
                                driver_usename = i.driver_usename,
                                requester_longitude = i.requester_longitude,
                                requester_latitude = i.requester_latitude
                            };

                            markers.Add(marker);
                            List_Request.Add(Req);

                             myMarkers.Add(marker,Req);
                        }
                    }
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
        public bool OnMarkerClick(Marker marker)
        {
            if (myMarkers.TryGetValue(marker, out Request cltrequest))
            {
                LatLng location_client = new LatLng(cltrequest.requester_latitude, cltrequest.requester_longitude);
                LatLng location_driver = new LatLng(location.Latitude, location.Longitude);

                var result =  GetDirectionAsync(location_client, location_driver);

                return true;
            }
            else
            {
                return false;
            }
        }
        private async Direction GetDirectionAsync(LatLng a,LatLng b)
        {
            using (HttpClient http_clt = new HttpClient())
            {
                string a_latitude = Convert.ToString(a.Latitude);
                string a_logitude = Convert.ToString(a.Longitude);
                string b_latitude = Convert.ToString(b.Latitude);
                string b_longitude = Convert.ToString(b.Longitude);
                HttpResponseMessage result = await http_clt
                    .GetAsync("https://maps.googleapis.com/maps/api/directions/json"
                    +"?origin="
                    +a_latitude
                    +","
                    +a_logitude
                    +"&destination="
                    +b_latitude
                    +","
                    +b_longitude
                    +"&key=AIzaSyAZRBPXKfwvmTTD0nFkfsweU3OhLAQhGC8");
                if (result.IsSuccessStatusCode)
                {
                    var content result.Content.ReadAsStringAsync()
                    return true;
                }
                if (result.IsSuccessStatusCode)
                {
                    return false;
                }
                else
                {
                    return false;
                }
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
            mMap.SetOnMarkerClickListener(this);
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