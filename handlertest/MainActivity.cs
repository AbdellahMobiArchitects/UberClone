using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Java.Lang;
using System.Threading;
using System.Threading.Tasks;

namespace handlertest
{
    [Activity(Label = "@string/app_name", MainLauncher = true, LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Icon = "@drawable/icon")]
    public class MainActivity : AppCompatActivity
    {
        static Context thisActivity = null;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            //Set your main view here
            SetContentView(Resource.Layout.main);
            thisActivity = this;

        }

        public Runnable Hellorunner = new Runnable(() =>
          {
              Handler handler = new Handler();
              handler.PostDelayed(hello, 5000);
          });

        private static void hello()
        {
            Toast.MakeText(thisActivity, "message", ToastLength.Long).Show();
        }
    }

}


