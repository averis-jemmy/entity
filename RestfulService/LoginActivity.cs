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
using MyAverisClient;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.ComponentModel;
using MyAverisEntity;
using MyAverisCommon;
using Newtonsoft.Json;
using Android.Views.InputMethods;
using Android.Support.V7.App;

namespace MyAveris.Droid
{
    [Activity(Label = "MYAveris", Theme = "@style/MyTheme.Base", WindowSoftInputMode = SoftInput.StateHidden, Icon = "@drawable/icon")]
    public class LoginActivity : AppCompatActivity
    {
        ProgressDialog _processProgress;
        string strResult;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            //Window.RequestFeature(WindowFeatures.NoTitle);
            Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Login);

            // Initialize toolbar
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.AppBar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetTitle(Resource.String.Empty);
            SupportActionBar.SetDisplayHomeAsUpEnabled(false);
            SupportActionBar.SetDisplayShowHomeEnabled(false);
            toolbar.FindViewById<TextView>(Resource.Id.lblClose).Visibility = ViewStates.Visible;
            toolbar.FindViewById<TextView>(Resource.Id.lblClose).Click += Close_OnClick;

            //Android.App.ActionBar mActionBar = this.ActionBar;
            //mActionBar.SetDisplayShowHomeEnabled(false);
            //mActionBar.SetDisplayShowTitleEnabled(false);
            //LayoutInflater mInflater = LayoutInflater.From(this);
            //View mCustomView = mInflater.Inflate(Resource.Layout.CustomLoginBar, null);
            //mActionBar.DisplayOptions = ActionBarDisplayOptions.ShowCustom;
            //mActionBar.CustomView = mCustomView;
            //mActionBar.SetDisplayShowCustomEnabled(true);
            //FindViewById<TextView>(Resource.Id.lblClose).Click += Close_OnClick;

            Button btnSubmit = (Button)FindViewById(Resource.Id.btnSubmit);
            btnSubmit.Click += SubmitButton_OnClick;

            TextView tvResend = (TextView)FindViewById(Resource.Id.tvResend);
            Button btnVerify = (Button)FindViewById(Resource.Id.btnVerify);
            btnVerify.Click += VerifyButton_OnClick;
            tvResend.Click += ResendText_OnClick;

            ServicePointManager.ServerCertificateValidationCallback =
                delegate(Object obj, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
                {
                    return (true);
                };

            _processProgress = new ProgressDialog(this);
            _processProgress.Indeterminate = true;
            _processProgress.SetProgressStyle(ProgressDialogStyle.Spinner);
            _processProgress.SetMessage("Loading...");
            _processProgress.SetCancelable(false);
            //_processProgress.SetButton("Cancel", (senderAlert, args) =>
            //{
                //CancelEvent();
            //});
        }

        void Close_OnClick(object sender, EventArgs e)
        {
            Finish();
        }

        void SubmitButton_OnClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(FindViewById<EditText>(Resource.Id.etPhoneNumber).Text))
            {
                Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this);
                alert.SetTitle("Error");
                alert.SetMessage("Please input your mobile number");
                alert.SetPositiveButton("OK", (senderAlert, args) =>
                {
                });

                RunOnUiThread(() =>
                {
                    alert.Show();
                });
                return;
            }

            if (FindViewById<EditText>(Resource.Id.etPhoneNumber).Text[0] == '0')
            {
                Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this);
                alert.SetTitle("Error");
                alert.SetMessage("Please input your country code (ex: 60 for Malaysia)");
                alert.SetPositiveButton("OK", (senderAlert, args) =>
                {
                });

                RunOnUiThread(() =>
                {
                    alert.Show();
                });
                return;
            }

            _processProgress.Show();

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        void ResendText_OnClick(object sender, EventArgs e)
        {
            _processProgress.Show();

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            RestClient client = new RestClient(CacheManager.URL, HttpVerb.POST, ContentTypeString.JSON, FindViewById<EditText>(Resource.Id.etPhoneNumber).Text);
            strResult = client.ProcessRequest("SignIn", null);
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _processProgress.Dismiss();
            if (!string.IsNullOrEmpty(strResult))
            {
                _processProgress.Dismiss();

                Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this);
                alert.SetTitle("Error");
                alert.SetMessage(strResult);
                alert.SetPositiveButton("OK", (senderAlert, args) =>
                {
                });

                RunOnUiThread(() =>
                {
                    alert.Show();
                });
                return;
            }

            try
            {
                InputMethodManager inputManager = (InputMethodManager)this.GetSystemService(Context.InputMethodService);
                var currentFocus = this.CurrentFocus;
                if (currentFocus != null)
                {
                    inputManager.HideSoftInputFromWindow(currentFocus.WindowToken, HideSoftInputFlags.None);
                }
            }
            catch { }

            CacheManager.PhoneNumber = FindViewById<EditText>(Resource.Id.etPhoneNumber).Text;
            FindViewById<LinearLayout>(Resource.Id.layoutPhone).Visibility = ViewStates.Gone;
            FindViewById<LinearLayout>(Resource.Id.layoutVerify).Visibility = ViewStates.Visible;
        }

        void VerifyButton_OnClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(FindViewById<EditText>(Resource.Id.etVerificationCode).Text)
                || FindViewById<EditText>(Resource.Id.etVerificationCode).Text.Length < 6)
            {
                Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this);
                alert.SetTitle("Error");
                alert.SetMessage("Please input your verification code");
                alert.SetPositiveButton("OK", (senderAlert, args) =>
                {
                });

                RunOnUiThread(() =>
                {
                    alert.Show();
                });
                return;
            }

            _processProgress.Show();

            BackgroundWorker verifyWorker = new BackgroundWorker();
            verifyWorker.DoWork += verifyWorker_DoWork;
            verifyWorker.RunWorkerCompleted += verifyWorker_RunWorkerCompleted;
            verifyWorker.RunWorkerAsync();
        }

        void verifyWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Verification model = new Verification();
            model.VerificationCode = FindViewById<EditText>(Resource.Id.etVerificationCode).Text;
            model.PhoneNumber = CacheManager.PhoneNumber;
            model.DeviceToken = "";
            model.DeviceType = InitialData.DeviceType.Android;

            string requestData = JsonConvert.SerializeObject(model);

            RestClient client = new RestClient(CacheManager.URL, HttpVerb.POST, ContentTypeString.JSON, requestData);
            strResult = client.ProcessRequest("Verify", null);
        }

        void verifyWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _processProgress.Dismiss();
            try
            {
                UserInfo info = JsonConvert.DeserializeObject<UserInfo>(strResult);
                if (info != null)
                {
                    User user = Database.UpdateUser(new User()
                    {
                        UserID = info.UserID,
                        Name = info.UserName,
                        EmailAddress = info.Email,
                        PhoneNumber = info.PhoneNumber,
                        TokenID = info.TokenID.GetValueOrDefault(),
                        IsRecruiter = info.IsRecruiter,
                        DeviceType = InitialData.DeviceType.Android,
                        DeviceToken = ""
                    });

                    Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this);
                    alert.SetTitle("Welcome");
                    alert.SetMessage("Hi, " + user.Name);
                    alert.SetPositiveButton("OK", (senderAlert, args) =>
                    {
                        SetResult(Result.Ok);
                        Finish();
                    });

                    RunOnUiThread(() =>
                    {
                        alert.Show();
                    });
                }
                else
                {
                    Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this);
                    alert.SetTitle("Error");
                    alert.SetMessage(strResult);
                    alert.SetPositiveButton("OK", (senderAlert, args) =>
                    {
                    });

                    RunOnUiThread(() =>
                    {
                        alert.Show();
                    });

                    FindViewById<EditText>(Resource.Id.etVerificationCode).Text = "";
                    return;
                }
            }
            catch
            {
                Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this);
                alert.SetTitle("Error");
                alert.SetMessage(strResult);
                alert.SetPositiveButton("OK", (senderAlert, args) =>
                {
                });

                RunOnUiThread(() =>
                {
                    alert.Show();
                });

                FindViewById<EditText>(Resource.Id.etVerificationCode).Text = "";
                return;
            }
        }
    }
}