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
using Plugin.Connectivity;
using UberClone.Helpers;
using UberClone.Models;
using System.Net.Http;
using System.Collections.Specialized;
using System.Globalization;

namespace UberClone.Activities
{
    [Activity(Label = "Current Location", ScreenOrientation = ScreenOrientation.Portrait)]
    public class ActivityYourLocation : FragmentActivity, ILocationListener, IOnMapReadyCallback, GoogleMap.IOnMapLoadedCallback
    {

        GoogleMap mMap; //Null if google apk services isn't available...
        LocationManager locationmanager;
        string provider;
        public Location location;
        CameraUpdate camera;
        string thisrequestdriverusername = null;
        Button button_requestuber, button_zoomin, button_zoomout;
        TextView tvinfo;

        LatLng mydriverlocation = new LatLng(0, 0); 

        bool requestactive = false;

        bool isRefreshLocationCalled = false;
        

        List<Marker> markers = new List<Marker>();

        LatLngBounds.Builder builder;

        Handler handler = new Handler();
        Action act;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Layout_ActivityYourLocation);
            button_requestuber = FindViewById<Button>(Resource.Id.button_requestuber);
            button_zoomin = FindViewById<Button>(Resource.Id.button_zoomin);
            button_zoomout = FindViewById<Button>(Resource.Id.button_zoomout);
            tvinfo = FindViewById<TextView>(Resource.Id.textviewinfo);

            button_requestuber.Click += button_requestuber_Click;
            button_zoomin.Click += Button_zoomin_Click;
            button_zoomout.Click += Button_zoomout_Click;

            //handler 5sec

            SetUpMapIfNeeded();
            act = new Action(RefreshLocation);
        }
        private async void Button_zoomout_Click(object sender, EventArgs e)
        {
            LatLng latlng = new LatLng(location.Latitude, location.Longitude);
            camera = CameraUpdateFactory.NewLatLng(latlng);
            mMap.MoveCamera(camera);
            await Task.Delay(500);
            camera = CameraUpdateFactory.ZoomOut();
            mMap.AnimateCamera(camera);
        }

        private async void Button_zoomin_Click(object sender, EventArgs e)
        {
            LatLng latlng = new LatLng(location.Latitude, location.Longitude);
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
                SupportMapFragment frag = (SupportFragmentManager.FindFragmentById(Resource.Id.fragment_googlemap) as SupportMapFragment);
                frag.GetMapAsync(this);

            }
            Android.Util.Log.Info("Lift", "SetupMapIfNeeded");
        }
        public void OnMapReady(GoogleMap googleMap)
        {
            this.mMap = googleMap;
            locationmanager = (LocationManager)GetSystemService(Context.LocationService);
            provider = locationmanager.GetBestProvider(new Criteria(), false);
            locationmanager.RequestLocationUpdates(provider, 400, 1, this);
            UpdateLocation();
            Android.Util.Log.Info("Lift", "OnMapReady");
            mMap.SetOnMapLoadedCallback(this);
            
        }
        private void UpdateLocation()
        {
            location = locationmanager.GetLastKnownLocation(provider);
            Android.Util.Log.Info("Lift", "UpdateLocation");
        }
        public void OnMapLoaded()
        {
            if (isRefreshLocationCalled == false)
            {
                RefreshLocation();
                Android.Util.Log.Info("Lift", "OnMapLoadedRefreshLocation");
            }
            Android.Util.Log.Info("Lift", "OnMapLoadedCallback");
        }

        public void OnLocationChanged(Location location)
        {
            UpdateLocation();
            Android.Util.Log.Info("Lift", "OnLocationChanged");

        }

        public async void RefreshLocation()
        {
            mMap.Clear();
            if (requestactive == false)
            {
                // api getuserrequest
                Tuple<Request, bool, string> userrequest = await GetThisUserRequest();
                Android.Util.Log.Info("Lift", "requestactive false API GetUserRequest " + userrequest.Item2
                    );
                if (userrequest.Item1!=null)
                {
                    Android.Util.Log.Info("Lift", "API RequestFound");
                    requestactive = true;
                    tvinfo.Text = "Finding UberDriver...";
                    button_requestuber.Text = "Cancel Uber";
                    if (!String.IsNullOrEmpty(userrequest.Item1.driver_usename))
                    {
                        thisrequestdriverusername = userrequest.Item1.driver_usename;
                        tvinfo.Text = "Your Driver Is Cumming";
                        button_requestuber.Visibility = ViewStates.Invisible;
                    }
                }
                if (userrequest.Item1 == null)
                {
                    Android.Util.Log.Info("Lift", "API RequestEmpty ");
                    thisrequestdriverusername = "";
                    tvinfo.Text = "";
                    button_requestuber.Visibility = ViewStates.Visible;
                }
            }

            if (String.IsNullOrEmpty(thisrequestdriverusername))
            {
                if (location != null)
                {
                    LatLng latlng = new LatLng(location.Latitude, location.Longitude);
                    MarkerOptions options = new MarkerOptions()
                                            .SetPosition(latlng)
                                            .SetTitle("MyLocation");
                    mMap.AddMarker(options);
                    camera = CameraUpdateFactory.NewLatLngZoom(latlng, 18);
                    mMap.MoveCamera(camera);
                }
                else
                {
                    Toast.MakeText(this, "Locating...", ToastLength.Short);
                }
            }
            if (requestactive == true)
            {
               
                // api getuserrequest
                Tuple<Request, bool, string> userrequest = await GetThisUserRequest();
                Android.Util.Log.Info("Lift", "requestactive true API RequestActive " + userrequest.Item2);
                if (!String.IsNullOrEmpty(userrequest.Item1.driver_usename))
                {
                    thisrequestdriverusername = userrequest.Item1.driver_usename;
                }
                    if (!String.IsNullOrEmpty(thisrequestdriverusername))
                {
                    //api get user where username == driver username
                    Tuple<User, bool, string> result_getdl = await GetRequestDriverLocation();

                    Android.Util.Log.Info("Lift","API RequestDriverLocation " +result_getdl.Item2.ToString());
                    if (result_getdl.Item2)
                    {
                        mydriverlocation.Longitude = Convert.ToDouble(result_getdl.Item1.user_longitude);
                        mydriverlocation.Latitude = Convert.ToDouble(result_getdl.Item1.user_latitude);
                    }


                    if (mydriverlocation.Longitude != 0 && mydriverlocation.Latitude != 0)
                    {
                        markers.Clear();
                        builder = new LatLngBounds.Builder();
                       
                        double distanceinkm = GeoDistanceHelper.DistanceBetweenPlaces(location.Longitude, location.Latitude, mydriverlocation.Longitude, mydriverlocation.Latitude);
                        tvinfo.Text = "Your Driver is " + distanceinkm + " Km Away";

                        markers.Add(mMap.AddMarker(new MarkerOptions()
                           .SetPosition(new LatLng(location.Latitude, location.Longitude))
                           .SetTitle("MyLocation")));

                        markers.Add(mMap.AddMarker(new MarkerOptions()
                           .SetIcon(BitmapDescriptorFactory
                           .DefaultMarker(BitmapDescriptorFactory.HueYellow))
                           .SetPosition(new LatLng(mydriverlocation.Latitude, mydriverlocation.Longitude))
                           .SetTitle("RiderLocation")));

                        if (markers.Count > 0)
                        {
                            foreach (Marker m in markers)
                            {
                                builder.Include(m.Position);
                            }

                        }
                        LatLngBounds bounds = builder.Build();
                        CameraUpdate cu = CameraUpdateFactory.NewLatLngBounds(bounds,100);
                        mMap.MoveCamera(cu);
                        
                    }
                }
               // api updateuserlocation in request
                Tuple<bool, string> result_update = await UpdateUserRequestLocationInDB();
                Android.Util.Log.Info("Lift", "API UpdateUserRequestLocationInDB " + result_update.Item1.ToString());

            }
           
                handler.PostDelayed(new Java.Lang.Runnable(act), 5000);
                Android.Util.Log.Info("Lift", "Handler");
            
            
            isRefreshLocationCalled = true;
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

        

      

        #region Request Uber Button Click
        private async void button_requestuber_Click(object sender, EventArgs e)
        {
            if (!requestactive)
            {
                //api save user
                /*create request save to db*/
                Tuple<bool,string> saveresult = await SaveUsersRequest();
                requestactive = true;
                tvinfo.Text = "Finding UberDriver...";
                button_requestuber.Text = "Cancel Uber";
                Android.Util.Log.Info("Lift", "API AddRequest " + saveresult.Item1.ToString());

            }
            else
            {
                // api deleteuser
                /*remove request from db*/
                Tuple<bool, string> saveresult = await DeleteUserRequest();
                requestactive = false;
                tvinfo.Text = "";
                button_requestuber.Text = "Request Uber";
                Android.Util.Log.Info("Lift", "API DeleteRequest " + saveresult.ToString());
            }
        }
        #endregion

        #region SaveRequestToDB
        private async Task<Tuple<bool, string>> SaveUsersRequest()
        {
            //check internet first
            if (CrossConnectivity.Current.IsConnected)
            {
                //internet available, setting up locals & save 'em to db

                FormUrlEncodedContent requestparameters = new FormUrlEncodedContent(new[]
               {
                    new KeyValuePair<string, string>("requester_username",Settings.Username),
                     new KeyValuePair<string, string>("requester_longitude", location.Longitude.ToString(CultureInfo.InvariantCulture)),
                     new KeyValuePair<string, string>("requester_latitude", location.Latitude.ToString(CultureInfo.InvariantCulture))

                 });
                Tuple<Request,bool, string> result = await RestHelper.APIRequest<Request>(AppUrls.api_url_requests, HttpVerbs.POST, null, requestparameters);
                if (result.Item1 != null & result.Item2)
                {
                    Settings.Request_ID = result.Item1.request_id.ToString();
                    Settings.Requester_Username = result.Item1.requester_username;
                    Settings.Requester_Longitude = result.Item1.requester_longitude.ToString(CultureInfo.InvariantCulture);
                    Settings.Requester_Latitude = result.Item1.requester_latitude.ToString(CultureInfo.InvariantCulture);
                    Settings.Driver_Username = result.Item1.driver_usename;
                    return new Tuple<bool, string>(result.Item2, result.Item3);
                }
                else
                {
                    return new Tuple<bool, string>(result.Item2, result.Item3);
                }
            }
            else
            {
                //internet not available, user tries again later
                return new Tuple<bool, string>(false, "No Internet Connection!");
            }
        }
        #endregion

        #region GetThisUserRequest

        public async Task<Tuple<Request, bool, string>> GetThisUserRequest()
        {
            //check internet first
            if (CrossConnectivity.Current.IsConnected)
            {
                //internet available, get this user's request

                NameValueCollection paramss = new NameValueCollection
                {
                    { "username", Settings.Username }
                };

                Tuple<Request, bool, string> result = await RestHelper.APIRequest<Request>(AppUrls.api_url_GetThisUserRequest,HttpVerbs.GET, paramss);

                return new Tuple<Request, bool, string>(result.Item1, result.Item2, result.Item3);
            }
            else
            {
                //internet not available, user tries again later
                return new Tuple<Request, bool, string>(default(Request), false, "No Internet Connection!");
            }
        }

        #endregion

        #region UpdateUserLocationInDB

        private async Task<Tuple<bool, string>> UpdateUserRequestLocationInDB()
        {
            //check internet first
            if (CrossConnectivity.Current.IsConnected)
            {

                //internet available, setting up locals & save 'em to db
                string reqlong = location.Longitude.ToString(CultureInfo.InvariantCulture);
                string reqlat = location.Latitude.ToString(CultureInfo.InvariantCulture);
                FormUrlEncodedContent paramss = new FormUrlEncodedContent(new[]
               {
                     new KeyValuePair<string, string>("request_id",Settings.Request_ID),
                     new KeyValuePair<string, string>("requester_username",Settings.Username),
                     new KeyValuePair<string, string>("requester_longitude",reqlong),
                     new KeyValuePair<string, string>("requester_latitude", reqlat),
                     new KeyValuePair<string, string>("driver_usename", thisrequestdriverusername)
                 });
                Tuple<Request, bool, string> result = await RestHelper.APIRequest<Request>(AppUrls.api_url_requests + Settings.Request_ID, HttpVerbs.PUT, null, null, paramss);

                    return new Tuple<bool, string>(result.Item2, result.Item3);
            }
            else
            {
                //internet not available, user tries again later
                return new Tuple<bool, string>(false, "No Internet Connection, Cannot Sync Your Location With Our Database");
            }
        }

        #endregion

        #region DeleteRequestFromDB&ClearLocals
        private async Task<bool> DeleteUserRequest()
        {
            if (!string.IsNullOrEmpty(Settings.Request_ID))
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    //attempting user deletion from db
                    string url = AppUrls.api_url_requests + Settings.Request_ID;
                    HttpClient httpClient = new HttpClient();
                    HttpResponseMessage response = await httpClient.DeleteAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        //successful attempt, cleaning local variables as well
                        Settings.ClearRequestLocalVars();
                        Toast.MakeText(this, "RequestDeleted", ToastLength.Short).Show();
                        return true;
                    }
                    else
                    {
                        //failed attempt, app stays open for now
                        Android.App.AlertDialog.Builder dialog = new Android.App.AlertDialog.Builder(this);
                        Android.App.AlertDialog alert = dialog.Create();
                        alert.SetTitle("Information!");
                        alert.SetMessage("Error: Couldn't Clean Request From Database!");
                        alert.SetIcon(Resource.Drawable.alert);
                        alert.SetButton("OK", (c, ev) =>
                        {

                        });
                        alert.Show();
                        return false;
                    }
                }
                else
                {
                    Toast.MakeText(this, "No Connection: Couldn't Clean Request From Database!", ToastLength.Long).Show();
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region GetDriverLocation

        private async Task<Tuple<User, bool, string>> GetRequestDriverLocation()
        {
            //check internet first
            if (CrossConnectivity.Current.IsConnected)
            {
                //internet available, setting up locals & getting this user's very own request

                NameValueCollection paramss = new NameValueCollection
                {
                    { "username", thisrequestdriverusername }
                };

                Tuple<User, bool, string> result = await RestHelper.APIRequest<User>(AppUrls.api_url_GetThisUser, HttpVerbs.GET, paramss, null,null);

                    return new Tuple<User, bool, string>(result.Item1, result.Item2, result.Item3);
                
            }
            else
            {
                //internet not available, user tries again later
                return new Tuple<User, bool, string>(default(User), false, "No Internet Connection!");
            }
        }

        #endregion

       
    }
}