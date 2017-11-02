using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Plugin.Connectivity;
using Android.Content.PM;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using UberClone.Models;
using UberClone.Helpers;
using System.Net.Http.Headers;
using Android.Support.V4.App;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;

namespace UberClone.Activities
{
    [Activity(Label = "@string/app_name",
        MainLauncher = true,
        ScreenOrientation = ScreenOrientation.Portrait,
        LaunchMode = LaunchMode.SingleTop,
        Theme = "@style/Theme.MyCustomTheme",
        Icon = "@drawable/icon")]
    public class MainActivity : AppCompatActivity
    {
        Switch switch_usertype;
        Button button_getstarted;
        string RiderOrDriver;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            //Set your main view here
            SetContentView(Resource.Layout.Layout_MainActivity);
            switch_usertype = FindViewById<Switch>(Resource.Id.switch_usertype);
            button_getstarted = FindViewById<Button>(Resource.Id.button_getstarted);

            button_getstarted.Click += Button_getstarted_Click;
        }

        private async void Button_getstarted_Click(object sender, EventArgs e)
        {
            //disable input
            button_getstarted.Enabled = false;

            //client is rider
            if (!switch_usertype.Checked)
            {
                RiderOrDriver = switch_usertype.TextOff;
               
                if (!string.IsNullOrEmpty(Settings.User_ID))
                {
                    //GetUser();
                    var getresult = await GetUser(Convert.ToInt32(Settings.User_ID));
                    if (getresult.Item2)
                    {
                        //Re-Assign();
                        Settings.User_ID = getresult.Item1.user_id.ToString();
                        Settings.Username = getresult.Item1.username;
                        Settings.Usertype = RiderOrDriver;
                        Settings.User_Longitude = getresult.Item1.user_longitude.ToString();
                        Settings.User_Latitude = getresult.Item1.user_latitude.ToString();
                        //Redirect();
                        RedirectUser(typeof(ActivityYourLocation));

                    }
                    if (!getresult.Item2)
                    {
                        var saveresult = await SaveUser();
                        if (saveresult.Item2)
                        {
                            Settings.User_ID = saveresult.Item1.user_id.ToString();
                            Settings.Username = saveresult.Item1.username;
                            Settings.Usertype = RiderOrDriver;
                            Settings.User_Longitude = saveresult.Item1.user_longitude.ToString();
                            Settings.User_Latitude = saveresult.Item1.user_latitude.ToString();
                            RedirectUser(typeof(ActivityYourLocation));
                        }
                        if (!saveresult.Item2)
                        {
                            Toast.MakeText(this, "Error adding "+RiderOrDriver, ToastLength.Short);
                        }
                    }
                }
                if (string.IsNullOrEmpty(Settings.User_ID))
                {
                    var saveresult = await SaveUser();
                    if (saveresult.Item2)
                    {
                        Settings.User_ID = saveresult.Item1.user_id.ToString();
                        Settings.Username = saveresult.Item1.username;
                        Settings.Usertype = RiderOrDriver;
                        Settings.User_Longitude = saveresult.Item1.user_longitude.ToString();
                        Settings.User_Latitude = saveresult.Item1.user_latitude.ToString();
                        RedirectUser(typeof(ActivityYourLocation));
                    }
                    if (!saveresult.Item2)
                    {
                        Toast.MakeText(this, "Error adding " + RiderOrDriver, ToastLength.Short);
                    }
                }

            }

            //client is driver
            if (switch_usertype.Checked)
            {
                RiderOrDriver = switch_usertype.TextOn;
                if (string.IsNullOrEmpty(Settings.User_ID))
                {
                    var saveresult = await SaveUser();
                    if (saveresult.Item2)
                    {
                        Settings.User_ID = saveresult.Item1.user_id.ToString();
                        Settings.Username = saveresult.Item1.username;
                        Settings.Usertype = RiderOrDriver;
                        Settings.User_Longitude = saveresult.Item1.user_longitude.ToString();
                        Settings.User_Latitude = saveresult.Item1.user_latitude.ToString();
                        RedirectUser(typeof(ActivityViewRequests));
                    }
                    if (!saveresult.Item2)
                    {
                        Toast.MakeText(this, "Error adding " + RiderOrDriver, ToastLength.Short);
                    }
                }
                if (!string.IsNullOrEmpty(Settings.User_ID))
                {
                    //GetUser();
                    var getresult = await GetUser(Convert.ToInt32(Settings.User_ID));
                    if (getresult.Item2)
                    {
                        //Re-Assign();
                        Settings.User_ID = getresult.Item1.user_id.ToString();
                        Settings.Username = getresult.Item1.username;
                        Settings.Usertype = RiderOrDriver;
                        Settings.User_Longitude = getresult.Item1.user_longitude.ToString();
                        Settings.User_Latitude = getresult.Item1.user_latitude.ToString();
                        //Redirect();
                        RedirectUser(typeof(ActivityViewRequests));

                    }
                    if (!getresult.Item2)
                    {
                        var saveresult = await SaveUser();
                        if (saveresult.Item2)
                        {
                            Settings.User_ID = saveresult.Item1.user_id.ToString();
                            Settings.Username = saveresult.Item1.username;
                            Settings.Usertype = RiderOrDriver;
                            Settings.User_Longitude = saveresult.Item1.user_longitude.ToString();
                            Settings.User_Latitude = saveresult.Item1.user_latitude.ToString();
                            RedirectUser(typeof(ActivityViewRequests));
                        }
                        if (!saveresult.Item2)
                        {
                            Toast.MakeText(this, "Error adding " + RiderOrDriver, ToastLength.Short);
                        }
                    }
                }
            }

            //enable input
            button_getstarted.Enabled = true;
        }

        private void RedirectUser(Type activity)
        {
            Intent i = new Intent(this, activity);
            i.PutExtra("user_type", RiderOrDriver);
            this.StartActivity(i);
        }

        #region Onbackpressed
        public override void OnBackPressed()
        {
            //bad idea deleting user from here; he can just kill process without doing this work
            //TODO: implement a good userdeletion maybe after the request has been fulfilled
            Android.App.AlertDialog.Builder dialog = new Android.App.AlertDialog.Builder(this);
            Android.App.AlertDialog alert = dialog.Create();
            alert.SetTitle("Info");
            alert.SetMessage("Exit App?");
            alert.SetIcon(Resource.Drawable.alert);
            alert.SetButton("OK", async (c, ev) =>
             {
                 //base.OnBackPressed();
                 if (await DeleteUser())
                 {
                     int pid = Android.OS.Process.MyPid();
                     if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                     {
                         FinishAndRemoveTask();
                         
                         Android.OS.Process.KillProcess(pid);
                     }
                     else
                     {
                         Finish();
                         Android.OS.Process.KillProcess(pid);
                     }
                 }
             });
            alert.SetButton2("CANCEL", (c, ev) => {/* here cancel action */ });
            alert.Show();
        }
        #endregion

        #region GetExistentUser

        public async Task<Tuple<User, bool, string>> GetUser(int id)
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                //internet available, getuser with that id
                var result = await RestHelper.APIRequest<User>(AppUrls.api_url_users+id.ToString()+"/", HttpVerbs.GET, null, null, null);

                return new Tuple<User, bool, string>(result.Item1, result.Item2, result.Item3);
            }
            else
            {
                //internet not available, user tries again later
                return new Tuple<User, bool, string>(default(User), false, "No Internet Connection!");
            }
        }

        #endregion

        #region SaveUserToDB
        private async Task<Tuple<User, bool, string>> SaveUser()
        {
            //check internet first
            if (CrossConnectivity.Current.IsConnected)
            {
                //internet available, save2db & set up localc params
                var postparams = new FormUrlEncodedContent(new[]
               {
                     new KeyValuePair<string, string>("usertype", RiderOrDriver)
                 });
                var result = await RestHelper.APIRequest<User>(AppUrls.api_url_users, HttpVerbs.POST, null, postparams, null);

                return new Tuple<User, bool, string>(result.Item1, result.Item2, result.Item3);
            }
            else
            {
                //internet not available, user tries again later
                return new Tuple<User, bool, string>(default(User), false, "No Internet Connection!");
            }
        }
        #endregion

        #region DeleteUserFromDB

        private async Task<bool> DeleteUser()
        {
            if (!string.IsNullOrEmpty(Settings.User_ID))
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    //attempting user deletion from db
                    string url = AppUrls.api_url_users + Settings.User_ID;
                    var httpClient = new HttpClient();
                    var response = await httpClient.DeleteAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        //successful attempt, cleaning local variables as well
                        //Settings.ClearUserLocalVars();
                        Toast.MakeText(this, "Cya Next Time!", ToastLength.Short).Show();
                        return true;
                    }
                    else
                    {
                        //failed attempt, app stays open for now
                        Toast.MakeText(this, "Error deleting user from db", ToastLength.Long).Show();
                        return false;
                    }
                }
                else
                {
                    Toast.MakeText(this, "No Internet", ToastLength.Long).Show();
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        #endregion
    }
}