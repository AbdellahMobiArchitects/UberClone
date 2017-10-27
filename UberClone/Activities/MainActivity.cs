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

       

        protected override void OnDestroy()
        {
            base.OnDestroy();

        }
        protected override void OnResume()
        {
            base.OnResume();
           
        }

        

        private async void Button_getstarted_Click(object sender, EventArgs e)
        {

            if (switch_usertype.Checked)
            {
                RiderOrDriver = switch_usertype.TextOn;
                var saveresult = await SaveUser();
                if (saveresult.Item1)
                {
                    RedirectUser(typeof(ActivityViewRequests));
                }
                else
                {
                    Android.App.AlertDialog.Builder dialog = new Android.App.AlertDialog.Builder(this);
                    Android.App.AlertDialog alert = dialog.Create();
                    alert.SetTitle("Information!");
                    alert.SetMessage("Couldn't Connect You With Our Database");
                    alert.SetIcon(Resource.Drawable.alert);
                    alert.SetButton("OK", (c, ev) =>
                    {

                    });
                    alert.Show();
                }
            }
            if (!switch_usertype.Checked)
            {
                RiderOrDriver = switch_usertype.TextOff;
                var saveresult = await SaveUser();
                if (saveresult.Item1)
                {
                    RedirectUser(typeof(ActivityYourLocation));
                }
                else
                {
                    Android.App.AlertDialog.Builder dialog = new Android.App.AlertDialog.Builder(this);
                    Android.App.AlertDialog alert = dialog.Create();
                    alert.SetTitle("Information!");
                    alert.SetMessage(saveresult.Item2);
                    alert.SetIcon(Resource.Drawable.alert);
                    alert.SetButton("OK", (c, ev) =>
                    {

                    });
                    alert.Show();
                }
            }

        }

        private async Task<Tuple<bool,string>> SaveUser()
        {
            //check internet first
            if (CrossConnectivity.Current.IsConnected)
            {
                //internet available, setting up locals & save 'em to db
                Settings.Usertype = RiderOrDriver;
                var requestparameters = new FormUrlEncodedContent(new[]
               {
                     new KeyValuePair<string, string>("usertype", Settings.Usertype),
                     new KeyValuePair<string, string>("user_longitude", Settings.User_long),
                     new KeyValuePair<string, string>("user_latitude", Settings.User_Lat)
                 });
               var result = await RestHelper.APIRequest<User>(AppUrls.api_url_users,HttpVerbs.POST,null,requestparameters);
                if ( result.Item1 !=null & result.Item2)
                {
                    Settings.User_id = result.Item1.user_id.ToString();
                    Settings.Username = result.Item1.username;
                    return new Tuple<bool, string>(result.Item2,result.Item3);
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

        private void RedirectUser(Type activity)
        {
            Intent i = new Intent(this, activity);
            i.PutExtra("user_type", RiderOrDriver);
            this.StartActivity(i);
        }

        #region SaveUserToDB
        //private async void SaveUserToDb(string usertype, Type activity)
        //{
        //    try
        //    {
        //        if (CrossConnectivity.Current.IsConnected)
        //        {
        //            string url = AppUrls.api_url_users;
        //            User user1 = new User
        //            {
        //                usertype = usertype,
        //            };
        //            var myContent = JsonConvert.SerializeObject(user1);
        //            var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
        //            var byteContent = new ByteArrayContent(buffer);
        //            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        //            var httpClient = new HttpClient();


        //            var response = await httpClient.PostAsync(url, byteContent);
        //            if (response.IsSuccessStatusCode)
        //            {
        //                Toast.MakeText(this, response.StatusCode.ToString() + response.ReasonPhrase.ToString(), ToastLength.Long).Show();
        //                Settings.User_id = user1.user_id.ToString();
        //                Settings.Username = user1.username;
        //                Settings.Usertype = user1.usertype;
        //                Settings.User_Lat = user1.user_latitude.ToString();
        //                Settings.User_long = user1.user_longitude.ToString();
        //                RedirectUser(activity);
        //            }
        //            else
        //            {
        //                Toast.MakeText(this, response.StatusCode.ToString() + response.ReasonPhrase.ToString(), ToastLength.Long).Show();
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {

        //        Toast.MakeText(this, e.Message, ToastLength.Long).Show();

        //    }
        //}
        #endregion

        #region Onkeybackpressed
        public override void OnBackPressed()
        {
            //bad idea deleting user from here; he can just kill process without doing this work
            //TODO: implement a good userdeletion maybe after the request has been fulfilled
            Android.App.AlertDialog.Builder dialog = new Android.App.AlertDialog.Builder(this);
            Android.App.AlertDialog alert = dialog.Create();
            alert.SetTitle("Info");
            alert.SetMessage("Exit App?");
            alert.SetIcon(Resource.Drawable.alert);
            alert.SetButton("OK",async (c, ev) =>
            {
                //base.OnBackPressed();
                if (await DeleteUser())
                {
                    if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                    { FinishAndRemoveTask(); }
                    else { Finish(); }
                }
            });
            alert.SetButton2("CANCEL", (c, ev) => {/* here cancel action */ });
            alert.Show();
        }

        private async Task<bool> DeleteUser()
        {
            if (!string.IsNullOrEmpty(Settings.User_id))
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    //attempting user deletion from db
                    string url = AppUrls.api_url_users + Settings.User_id;
                    var httpClient = new HttpClient();
                    var response = await httpClient.DeleteAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        //successful attempt, cleaning local variables as well
                        Settings.ClearAll();
                        Toast.MakeText(this, "Cya Next Time!", ToastLength.Short).Show();
                        return true;
                    }
                    else
                    {
                        //failed attempt, app stays open for now
                        Android.App.AlertDialog.Builder dialog = new Android.App.AlertDialog.Builder(this);
                        Android.App.AlertDialog alert = dialog.Create();
                        alert.SetTitle("Information!");
                        alert.SetMessage("Couldn't Clean User From Database!");
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
                    Toast.MakeText(this, "No Connection, Couldn't Clean User From Database!", ToastLength.Long).Show();
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

