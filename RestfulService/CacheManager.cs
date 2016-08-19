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
using MyAverisEntity;
using Android.Graphics;

namespace MyAveris.Droid
{
    public static class CacheManager
    {
        public static JobApplicationInfo JobInfo;
        public static int JobApplicationID;
        public static string URL = "https://203.223.140.195/MyAverisServiceHttps/AverisMobile.svc/";
        public static Context mContext;
        public static string PhoneNumber;
        public static Guid UserID;
        public static Guid TokenID;
        public static Guid JobID;
        public static bool IsRecruiter;
        public static List<ShortJobApplicationInfo> JobApplicationInfos = new List<ShortJobApplicationInfo>();
        public static Bitmap ImageProf;

        public static void Init(Context context)
        {
            mContext = context;
        }
    }
}