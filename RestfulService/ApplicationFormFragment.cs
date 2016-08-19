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
using Android.Provider;
using Java.IO;
using Android.Content.PM;
using Android.Graphics;
using Android.Media;
using System.IO;

namespace MyAveris.Droid
{
    [Activity(Label = "ApplicationFormFragment")]
    public class ApplicationFormFragment : Fragment
    {
        Java.IO.File imageFile, imageDir;
        ImageView imgProf;
        ProgressDialog _processProgress;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _processProgress = new ProgressDialog(this.Activity);
            _processProgress.Indeterminate = true;
            _processProgress.SetProgressStyle(ProgressDialogStyle.Spinner);
            _processProgress.SetMessage("Loading...");
            _processProgress.SetCancelable(false);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.ApplicationFormFragment, container, false);

            view.FindViewById<ImageView>(Resource.Id.imgPersonalInfo).Click += PersonalInfo_OnClick;
            view.FindViewById<LinearLayout>(Resource.Id.layPersonalInfo).Click += PersonalInfo_OnClick;
            view.FindViewById<ImageView>(Resource.Id.imgProf).Click += ProfilePicture_OnClick;

            imgProf = view.FindViewById<ImageView>(Resource.Id.imgProf);

            if(CacheManager.JobInfo.Photo != null)
            {
                try
                {
                    Bitmap bmp = BitmapFactory.DecodeByteArray(CacheManager.JobInfo.Photo, 0, CacheManager.JobInfo.Photo.Length);
                    imgProf.SetImageBitmap(bmp);
                    GC.Collect();
                }
                catch { }
            }

            return view;
        }

        void ProfilePicture_OnClick(object sender, EventArgs e)
        {
            SelectImage();
        }

        void PersonalInfo_OnClick(object sender, EventArgs e)
        {
            var intent = new Intent(this.Activity, typeof(PersonalInfoActivity));
            StartActivity(intent);
        }

        public override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            try
            {
                if (resultCode == Result.Ok)
                {
                    _processProgress.Show();
                    Android.Net.Uri contentUri = null;
                    int height = Resources.DisplayMetrics.HeightPixels; 
                    int width = Resources.DisplayMetrics.WidthPixels;
                    if (requestCode == 1)
                    {
                        contentUri = Android.Net.Uri.FromFile(imageFile);

                        Bitmap bmp = NGetBitmap(contentUri);
                        ExifInterface ei = new ExifInterface(contentUri.Path);
                        string orientation = ei.GetAttribute(ExifInterface.TagOrientation);

                        switch (orientation)
                        {
                            case "6":
                                bmp = RotateImage(bmp, 90);
                                break;
                            case "3":
                                bmp = RotateImage(bmp, 180);
                                break;
                            case "8":
                                bmp = RotateImage(bmp, 270);
                                break;
                            case "1":
                            default:
                                break;
                        }

                        JobApplication app = Database.GetJobApplication();
                        byte[] bitmapData = null;
                        using (var stream = new MemoryStream())
                        {
                            bmp.Compress(Bitmap.CompressFormat.Png, 0, stream);
                            bitmapData = stream.ToArray();
                        }
                        CacheManager.JobInfo.Photo = bitmapData;
                        app.Photo = bitmapData;
                        Database.UpdateJobApplication(app);

                        imgProf.SetImageBitmap(bmp);
                        GC.Collect();
                    }
                    if (requestCode == 2)
                    {
                        contentUri = data.Data;

                        Bitmap bmp = NGetBitmap(contentUri);

                        string orientation = "0";
                        Android.Database.ICursor cursor = this.Activity.ContentResolver.Query(contentUri,
                            new String[] { MediaStore.Images.ImageColumns.Orientation }, null, null, null);
                        if (cursor.Count == 1)
                        {
                            cursor.MoveToFirst();
                            orientation = cursor.GetInt(0).ToString();
                        }

                        switch (orientation)
                        {
                            case "90":
                                bmp = RotateImage(bmp, 90);
                                break;
                            case "180":
                                bmp = RotateImage(bmp, 180);
                                break;
                            case "270":
                                bmp = RotateImage(bmp, 270);
                                break;
                            case "0":
                            default:
                                break;
                        }

                        JobApplication app = Database.GetJobApplication();
                        byte[] bitmapData = null;
                        using (var stream = new MemoryStream())
                        {
                            bmp.Compress(Bitmap.CompressFormat.Png, 0, stream);
                            bitmapData = stream.ToArray();
                        }
                        CacheManager.JobInfo.Photo = bitmapData;
                        app.Photo = bitmapData;
                        Database.UpdateJobApplication(app);

                        imgProf.SetImageBitmap(bmp);
                        GC.Collect();
                    }
                }
            }
            catch { }
            _processProgress.Dismiss();
        }

        public static Bitmap RotateImage(Bitmap source, float angle)
        {
            Matrix matrix = new Matrix();
            matrix.PostRotate(angle);
            return Bitmap.CreateBitmap(source, 0, 0, source.Width, source.Height, matrix, true);
        }

        private Bitmap NGetBitmap(Android.Net.Uri uriImage)
        {
            Android.Graphics.Bitmap mBitmap = null;
            mBitmap = Android.Provider.MediaStore.Images.Media.GetBitmap(this.Activity.ContentResolver, uriImage);
            return mBitmap;
        }

        private void SelectImage()
        {
            string[] items = { "Take Photo", "Choose from Library", "Cancel" };
            Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this.Activity);
            alert.SetTitle("Add Photo!");
            alert.SetItems(items, (senderAlert, args) =>
            {
                //bool result = Utility.checkPermission(MainActivity.this);
                if (items[args.Which] == "Take Photo")
                {
                    if (IsThereAnAppToTakePictures())
                    {
                        CreateDirectoryForPictures();
                        Intent intent = new Intent(MediaStore.ActionImageCapture);
                        imageFile = new Java.IO.File(imageDir, String.Format("myPhoto_{0}.jpg", Guid.NewGuid()));
                        intent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(imageFile));
                        StartActivityForResult(intent, 1);
                    }
                }
                else if (items[args.Which] == "Choose from Library")
                {
                    Intent intent = new Intent();
                    intent.SetType("image/*");
                    intent.SetAction(Intent.ActionGetContent);
                    StartActivityForResult(Intent.CreateChooser(intent, "Select File"), 2);
                }
            });

            this.Activity.RunOnUiThread(() =>
            {
                alert.Show();
            });
        }

        private bool IsThereAnAppToTakePictures()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            PackageManager pm = this.Activity.PackageManager;
            IList<ResolveInfo> availableActivities = pm.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
            return availableActivities != null && availableActivities.Count > 0;
        }

        private void CreateDirectoryForPictures()
        {
            imageDir = new Java.IO.File(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures),
                "MyAverisImage");
            if (!imageDir.Exists())
            {
                imageDir.Mkdirs();
            }
        }

        /* Checks if external storage is available for read and write */
        public bool IsExternalStorageWritable()
        {
            if (Android.OS.Environment.ExternalStorageState == Android.OS.Environment.MediaMounted)
            {
                return true;
            }
            return false;
        }
    }
}