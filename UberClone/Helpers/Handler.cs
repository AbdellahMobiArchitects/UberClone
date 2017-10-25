//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using Android.App;
//using Android.Content;
//using Android.OS;
//using Android.Runtime;
//using Android.Views;
//using Android.Widget;

//namespace UberClone.Helpers
//{
//    class Handler
//    {
//        Handler handler = new Handler();
//        Action act;
//        string name = "toto";
//        int i = 0;

//        protected override void OnCreate(Bundle savedInstanceState)
//        {
//            base.OnCreate(savedInstanceState);

//            // Set our view from the "main" layout resource
//            SetContentView(Resource.Layout.Main);
//            TotoHandler();



//        }

//        private void TotoHandler()
//        {
//            act = new Action(Hello);
//            handler.PostDelayed(new Runnable(act), 0);
//            void Hello()
//            {
//                if (i == 4)
//                {
//                    name = "foobar";
//                }
//                Toast.MakeText(this, name, ToastLength.Short).Show();
//                i++;
//                handler.PostDelayed(act, 5000);

//            }
//        }
//    }
//}