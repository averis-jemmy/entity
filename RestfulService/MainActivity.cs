using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.ComponentModel;
using MyAverisClient;
using System.Collections.Generic;
using MyAverisEntity;
using Newtonsoft.Json;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using Android.Views.InputMethods;
using MyAverisCommon;

namespace MyAveris.Droid
{
    [Activity(Label = "MYAveris", Theme = "@style/MyTheme.Base", WindowSoftInputMode = SoftInput.StateHidden, Icon = "@drawable/icon")]
    public class MainActivity : AppCompatActivity
    {
        ProgressDialog _processProgress;
        string strResult, strLookupResult, strSubmitResult;

        Android.Support.V7.Widget.Toolbar toolbar;
        DrawerLayout drawerLayout;
        NavigationView navigationView;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            ServicePointManager.ServerCertificateValidationCallback =
                delegate(Object obj, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
                {
                    return (true);
                };

            var user = Database.GetUser();

            if (user == null)
            {
                var intent = new Intent(this, typeof(DiscoverAverisActivity));
                StartActivity(intent);
                Finish();
            }
            else
            {
                CacheManager.TokenID = user.TokenID;
                CacheManager.UserID = user.UserID;
                CacheManager.IsRecruiter = user.IsRecruiter;

                SetContentView(Resource.Layout.Main);
                drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawerLayout);

                JobApplication jobApp = Database.GetJobApplication();
                if (jobApp != null)
                {
                    CacheManager.JobApplicationID = jobApp.ID;

                    CacheManager.JobInfo = new JobApplicationInfo()
                    {
                        Photo = jobApp.Photo,
                        Title = jobApp.Title,
                        FirstName = jobApp.FirstName,
                        LastName = jobApp.LastName,
                        PositionApplied = jobApp.PositionApplied,
                        KnownAs = jobApp.KnownAs,
                        ChineseCharacter = jobApp.ChineseCharacter,
                        Gender = jobApp.Gender,
                        DateOfBirth = jobApp.DateOfBirth,
                        CountryOfBirth = jobApp.CountryOfBirth,
                        Nationality = jobApp.Nationality,
                        Race = jobApp.Race,
                        MaritalStatus = jobApp.MaritalStatus,
                        Religion = jobApp.Religion,
                        IdentityNo = jobApp.IdentityNo,
                        DateOfIssue = jobApp.DateOfIssue,
                        DateOfExpiry = jobApp.DateOfExpiry,
                        CountryOfIssue = jobApp.CountryOfIssue,
                        EmailAddress = jobApp.EmailAddress,
                        ApplicationStatus = jobApp.ApplicationStatus,
                        IsLocked = jobApp.IsLocked
                    };
                }

                // Initialize toolbar
                toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.AppBar);
                SetSupportActionBar(toolbar);
                SupportActionBar.SetTitle(Resource.String.MyAveris);
                //SupportActionBar.SetIcon(Resource.Drawable.Icon);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SupportActionBar.SetDisplayShowHomeEnabled(true);
                toolbar.FindViewById<TextView>(Resource.Id.lblSubmit).Click += Submit_OnClick;
                toolbar.FindViewById<TextView>(Resource.Id.lblRefresh).Click += Refresh_OnClick;

                // Attach item selected handler to navigation view
                navigationView = FindViewById<NavigationView>(Resource.Id.navView);
                navigationView.NavigationItemSelected += NavigationView_NavigationItemSelected;
                navigationView.Menu.Clear();
                if (!user.IsRecruiter)
                {
                    navigationView.InflateMenu(Resource.Menu.menu_applicant);
                    if (true)
                    {
                        navigationView.Menu.RemoveItem(Resource.Id.navPreBoarding);
                    }
                }
                else
                {
                    navigationView.InflateMenu(Resource.Menu.menu_recruiter);
                }

                // Create ActionBarDrawerToggle button and add it to the toolbar
                var drawerToggle = new ActionBarDrawerToggle(this, drawerLayout, toolbar, Resource.String.open_drawer, Resource.String.close_drawer);
                drawerLayout.AddDrawerListener(drawerToggle);
                drawerToggle.SyncState();

                //Load default screen
                var ft = FragmentManager.BeginTransaction();
                ft.AddToBackStack(null);
                ft.Add(Resource.Id.HomeFrameLayout, new HomeFragment());
                ft.Commit();

                FindViewById<Button>(Resource.Id.nav_logout).Click += LogoutButton_OnClick;

                _processProgress = new ProgressDialog(this);
                _processProgress.Indeterminate = true;
                _processProgress.SetProgressStyle(ProgressDialogStyle.Spinner);
                _processProgress.SetMessage("Loading...");
                _processProgress.SetCancelable(false);
                _processProgress.Show();

                BackgroundWorker lookupWorker = new BackgroundWorker();
                lookupWorker.DoWork += lookupWorker_DoWork;
                lookupWorker.RunWorkerCompleted += lookupWorker_RunWorkerCompleted;
                lookupWorker.RunWorkerAsync();
            }
        }

        void Submit_OnClick(object sender, EventArgs e)
        {
            if (!CacheManager.JobInfo.IsLocked.GetValueOrDefault())
            {
                _processProgress.Show();

                BackgroundWorker jobAppWorker = new BackgroundWorker();
                jobAppWorker.DoWork += jobAppWorker_DoWork;
                jobAppWorker.RunWorkerCompleted += jobAppWorker_RunWorkerCompleted;
                jobAppWorker.RunWorkerAsync();
            }
            else
            {
                Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this);
                alert.SetTitle("Error");
                alert.SetMessage("You have submitted this application. Please wait until further notice...");
                alert.SetPositiveButton("OK", (senderAlert, args) =>
                {
                });

                RunOnUiThread(() =>
                {
                    alert.Show();
                });
            }
        }

        void Refresh_OnClick(object sender, EventArgs e)
        {
            _processProgress.Show();

            BackgroundWorker refreshWorker = new BackgroundWorker();
            refreshWorker.DoWork += refreshWorker_DoWork;
            refreshWorker.RunWorkerCompleted += refreshWorker_RunWorkerCompleted;
            refreshWorker.RunWorkerAsync();
        }

        protected override void OnResume()
        {
            base.OnResume();

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
        }

        void NavigationView_NavigationItemSelected(object sender, NavigationView.NavigationItemSelectedEventArgs e)
        {
            if (!e.MenuItem.IsChecked)
            {
                var ft = FragmentManager.BeginTransaction();
                toolbar.FindViewById<TextView>(Resource.Id.lblSubmit).Visibility = ViewStates.Gone;
                toolbar.FindViewById<TextView>(Resource.Id.lblRefresh).Visibility = ViewStates.Gone;
                SupportActionBar.SetTitle(Resource.String.MyAveris);
                switch (e.MenuItem.ItemId)
                {
                    case (Resource.Id.navHome):
                        ft.Replace(Resource.Id.HomeFrameLayout, new HomeFragment());
                        break;
                    case (Resource.Id.navApplicationForm):
                        SupportActionBar.SetTitle(Resource.String.ApplicationForm);
                        toolbar.FindViewById<TextView>(Resource.Id.lblSubmit).Visibility = ViewStates.Visible;
                        ft.Replace(Resource.Id.HomeFrameLayout, new ApplicationFormFragment());
                        break;
                    case (Resource.Id.navApplicationList):
                        SupportActionBar.SetTitle(Resource.String.ApplicationList);
                        toolbar.FindViewById<TextView>(Resource.Id.lblRefresh).Visibility = ViewStates.Visible;
                        ft.Replace(Resource.Id.HomeFrameLayout, new ApplicationListFragment());
                        break;
                }
                ft.Commit();
            }
            drawerLayout.CloseDrawers();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    drawerLayout.OpenDrawer(Android.Support.V4.View.GravityCompat.Start);
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        void LogoutButton_OnClick(object sender, EventArgs e)
        {
            Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this);
            alert.SetTitle("Logout");
            alert.SetMessage("Do you really want to logout?");
            alert.SetPositiveButton("Yes", (senderAlert, args) =>
            {
                Database.ClearData();
                CacheManager.JobInfo = null;
                var intent = new Intent(this, typeof(MainActivity));
                StartActivity(intent);
                FinishAffinity();
            });
            alert.SetNegativeButton("No", (senderAlert, args) =>
            {
            });

            RunOnUiThread(() =>
            {
                alert.Show();
            });
        }

        void jobAppWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<KeyValuePair<string, string>> headers = new List<KeyValuePair<string, string>>();
            headers.Add(new KeyValuePair<string, string>("UserID", CacheManager.UserID.ToString()));
            headers.Add(new KeyValuePair<string, string>("TokenID", CacheManager.TokenID.ToString()));

            string requestData = JsonConvert.SerializeObject(CacheManager.JobInfo,
                                              new JsonSerializerSettings() { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat });
            RestClient client = new RestClient(CacheManager.URL, HttpVerb.POST, ContentTypeString.JSON, requestData);
            strSubmitResult = client.ProcessRequest("SubmitJobApplication", headers);
        }

        void jobAppWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                _processProgress.Dismiss();
                if (!string.IsNullOrEmpty(strSubmitResult))
                {
                    if (strSubmitResult.ToUpper().Contains("IT IS FORBIDDEN"))
                    {
                        Database.ClearData();
                        CacheManager.JobInfo = null;
                        Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this);
                        alert.SetTitle("Error");
                        alert.SetMessage("Your session has expired. Please login again.");
                        alert.SetPositiveButton("OK", (senderAlert, args) =>
                        {
                            var intent = new Intent(this, typeof(MainActivity));
                            StartActivity(intent);
                            FinishAffinity();
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
                        alert.SetMessage("Submission Failed - " + strSubmitResult);
                        alert.SetPositiveButton("OK", (senderAlert, args) =>
                        {
                        });

                        RunOnUiThread(() =>
                        {
                            alert.Show();
                        });
                    }
                }
                else
                {
                    Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this);
                    alert.SetTitle("Info");
                    alert.SetMessage("Your job application has been submitted successfully");
                    alert.SetPositiveButton("OK", (senderAlert, args) =>
                    {
                    });

                    RunOnUiThread(() =>
                    {
                        alert.Show();
                    });

                    CacheManager.JobInfo.ApplicationStatus = InitialData.ApplicationStatus.Submitted;
                    CacheManager.JobInfo.IsLocked = true;

                    JobApplication model = new JobApplication()
                    {
                        ID = CacheManager.JobApplicationID,
                        Title = CacheManager.JobInfo.Title,
                        FirstName = CacheManager.JobInfo.FirstName,
                        LastName = CacheManager.JobInfo.LastName,
                        PositionApplied = CacheManager.JobInfo.PositionApplied,
                        KnownAs = CacheManager.JobInfo.KnownAs,
                        ChineseCharacter = CacheManager.JobInfo.ChineseCharacter,
                        Gender = CacheManager.JobInfo.Gender,
                        DateOfBirth = CacheManager.JobInfo.DateOfBirth,
                        CountryOfBirth = CacheManager.JobInfo.CountryOfBirth,
                        Nationality = CacheManager.JobInfo.Nationality,
                        Race = CacheManager.JobInfo.Race,
                        MaritalStatus = CacheManager.JobInfo.MaritalStatus,
                        Religion = CacheManager.JobInfo.Religion,
                        IdentityNo = CacheManager.JobInfo.IdentityNo,
                        DateOfIssue = CacheManager.JobInfo.DateOfIssue,
                        DateOfExpiry = CacheManager.JobInfo.DateOfExpiry,
                        CountryOfIssue = CacheManager.JobInfo.CountryOfIssue,
                        EmailAddress = CacheManager.JobInfo.EmailAddress,
                        ApplicationStatus = CacheManager.JobInfo.ApplicationStatus,
                        IsLocked = CacheManager.JobInfo.IsLocked.GetValueOrDefault()
                    };

                    Database.UpdateJobApplication(model);
                }
            }
            catch { }
        }

        void lookupWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            RestClient client = new RestClient(CacheManager.URL, HttpVerb.POST, ContentTypeString.JSON, string.Empty);
            strLookupResult = client.ProcessRequest("GetDeclarationDetails", null);
        }

        void lookupWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(strLookupResult))
                {
                    List<DeclarationDetailInfo> info = JsonConvert.DeserializeObject<List<DeclarationDetailInfo>>(strLookupResult);
                    var details = (from item in info
                                   select new DeclarationDetail()
                                   {
                                       ID = item.ID,
                                       No = item.No.GetValueOrDefault(),
                                       Declaration = item.Declaration
                                   }).ToList();

                    if (details != null && details.Count > 0)
                        Database.UpdateDeclarationDetails(details);
                }
            }
            catch { }

            if (!CacheManager.IsRecruiter)
            {
                if (CacheManager.JobInfo == null)
                {
                    BackgroundWorker worker = new BackgroundWorker();
                    worker.DoWork += worker_DoWork;
                    worker.RunWorkerCompleted += worker_RunWorkerCompleted;
                    worker.RunWorkerAsync();
                }
                else
                {
                    BackgroundWorker shortWorker = new BackgroundWorker();
                    shortWorker.DoWork += shortWorker_DoWork;
                    shortWorker.RunWorkerCompleted += shortWorker_RunWorkerCompleted;
                    shortWorker.RunWorkerAsync();
                }
            }
            else
            {
                BackgroundWorker refreshWorker = new BackgroundWorker();
                refreshWorker.DoWork += refreshWorker_DoWork;
                refreshWorker.RunWorkerCompleted += refreshWorker_RunWorkerCompleted;
                refreshWorker.RunWorkerAsync();
            }
        }

        void refreshWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<KeyValuePair<string, string>> headers = new List<KeyValuePair<string, string>>();
            headers.Add(new KeyValuePair<string, string>("UserID", CacheManager.UserID.ToString()));
            headers.Add(new KeyValuePair<string, string>("TokenID", CacheManager.TokenID.ToString()));

            RestClient client = new RestClient(CacheManager.URL, HttpVerb.POST, ContentTypeString.JSON, string.Empty);
            strResult = client.ProcessRequest("GetAllJobApplications", headers);
        }

        void refreshWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                _processProgress.Dismiss();
                if (!string.IsNullOrEmpty(strResult))
                {
                    if (strResult.ToUpper().Contains("IT IS FORBIDDEN"))
                    {
                        Database.ClearData();
                        CacheManager.JobInfo = null;
                        Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this);
                        alert.SetTitle("Error");
                        alert.SetMessage("Your session has expired. Please login again.");
                        alert.SetPositiveButton("OK", (senderAlert, args) =>
                        {
                            var intent = new Intent(this, typeof(MainActivity));
                            StartActivity(intent);
                            FinishAffinity();
                        });

                        RunOnUiThread(() =>
                        {
                            alert.Show();
                        });
                    }
                    else if (strResult.Contains("ErrorMessage"))
                    {
                        if (CacheManager.JobInfo == null)
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
                        }
                    }
                    else
                    {
                        List<ShortJobApplicationInfo> infos = JsonConvert.DeserializeObject<List<ShortJobApplicationInfo>>(strResult);
                        if (infos != null && infos.Count > 0)
                            CacheManager.JobApplicationInfos = infos;
                        else
                        {
                            Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this);
                            alert.SetTitle("Error");
                            alert.SetMessage("No more pending job applications");
                            alert.SetPositiveButton("OK", (senderAlert, args) =>
                            {
                            });

                            RunOnUiThread(() =>
                            {
                                alert.Show();
                            });
                        }
                    }
                }
            }
            catch { }
        }

        void shortWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<KeyValuePair<string, string>> headers = new List<KeyValuePair<string, string>>();
            headers.Add(new KeyValuePair<string, string>("UserID", CacheManager.UserID.ToString()));
            headers.Add(new KeyValuePair<string, string>("TokenID", CacheManager.TokenID.ToString()));

            RestClient client = new RestClient(CacheManager.URL, HttpVerb.POST, ContentTypeString.JSON, string.Empty);
            strResult = client.ProcessRequest("GetJobApplicationStatus", headers);
        }

        void shortWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                _processProgress.Dismiss();
                if (!string.IsNullOrEmpty(strResult))
                {
                    if (strResult.ToUpper().Contains("IT IS FORBIDDEN"))
                    {
                        Database.ClearData();
                        CacheManager.JobInfo = null;
                        Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this);
                        alert.SetTitle("Error");
                        alert.SetMessage("Your session has expired. Please login again.");
                        alert.SetPositiveButton("OK", (senderAlert, args) =>
                        {
                            var intent = new Intent(this, typeof(MainActivity));
                            StartActivity(intent);
                            FinishAffinity();
                        });

                        RunOnUiThread(() =>
                        {
                            alert.Show();
                        });
                    }
                    else if (strResult.Contains("ErrorMessage"))
                    {
                        if (CacheManager.JobInfo == null)
                        {
                            Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this);
                            alert.SetTitle("Error");
                            alert.SetMessage("Please enable your internet for a while");
                            alert.SetPositiveButton("OK", (senderAlert, args) =>
                            {
                                var intent = new Intent(this, typeof(MainActivity));
                                StartActivity(intent);
                                FinishAffinity();
                            });

                            RunOnUiThread(() =>
                            {
                                alert.Show();
                            });
                        }
                    }
                    else
                    {
                        ShortJobApplicationInfo info = JsonConvert.DeserializeObject<ShortJobApplicationInfo>(strResult);
                        CacheManager.JobInfo.ApplicationStatus = info.ApplicationStatus;
                        CacheManager.JobInfo.IsLocked = info.IsLocked;

                        JobApplication model = new JobApplication()
                        {
                            ID = CacheManager.JobApplicationID,
                            Title = CacheManager.JobInfo.Title,
                            FirstName = CacheManager.JobInfo.FirstName,
                            LastName = CacheManager.JobInfo.LastName,
                            PositionApplied = CacheManager.JobInfo.PositionApplied,
                            KnownAs = CacheManager.JobInfo.KnownAs,
                            ChineseCharacter = CacheManager.JobInfo.ChineseCharacter,
                            Gender = CacheManager.JobInfo.Gender,
                            DateOfBirth = CacheManager.JobInfo.DateOfBirth,
                            CountryOfBirth = CacheManager.JobInfo.CountryOfBirth,
                            Nationality = CacheManager.JobInfo.Nationality,
                            Race = CacheManager.JobInfo.Race,
                            MaritalStatus = CacheManager.JobInfo.MaritalStatus,
                            Religion = CacheManager.JobInfo.Religion,
                            IdentityNo = CacheManager.JobInfo.IdentityNo,
                            DateOfIssue = CacheManager.JobInfo.DateOfIssue,
                            DateOfExpiry = CacheManager.JobInfo.DateOfExpiry,
                            CountryOfIssue = CacheManager.JobInfo.CountryOfIssue,
                            EmailAddress = CacheManager.JobInfo.EmailAddress,
                            ApplicationStatus = info.ApplicationStatus,
                            IsLocked = info.IsLocked.GetValueOrDefault()
                        };

                        Database.UpdateJobApplication(model);
                    }
                }
            }
            catch { }
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<KeyValuePair<string, string>> headers = new List<KeyValuePair<string, string>>();
            headers.Add(new KeyValuePair<string, string>("UserID", CacheManager.UserID.ToString()));
            headers.Add(new KeyValuePair<string, string>("TokenID", CacheManager.TokenID.ToString()));

            RestClient client = new RestClient(CacheManager.URL, HttpVerb.POST, ContentTypeString.JSON, string.Empty);
            strResult = client.ProcessRequest("GetLatestJobApplication", headers);
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                _processProgress.Dismiss();
                if (!string.IsNullOrEmpty(strResult))
                {
                    if (strResult.ToUpper().Contains("IT IS FORBIDDEN"))
                    {
                        Database.ClearData();
                        CacheManager.JobInfo = null;
                        Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this);
                        alert.SetTitle("Error");
                        alert.SetMessage("Your session has expired. Please login again.");
                        alert.SetPositiveButton("OK", (senderAlert, args) =>
                        {
                            var intent = new Intent(this, typeof(MainActivity));
                            StartActivity(intent);
                            FinishAffinity();
                        });

                        RunOnUiThread(() =>
                        {
                            alert.Show();
                        });
                    }
                    else if (strResult.Contains("ErrorMessage"))
                    {
                        if (CacheManager.JobInfo == null)
                        {
                            Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this);
                            alert.SetTitle("Error");
                            alert.SetMessage("Please enable your internet for a while");
                            alert.SetPositiveButton("OK", (senderAlert, args) =>
                            {
                                var intent = new Intent(this, typeof(MainActivity));
                                StartActivity(intent);
                                FinishAffinity();
                            });

                            RunOnUiThread(() =>
                            {
                                alert.Show();
                            });
                        }
                    }
                    else
                    {
                        JobApplicationInfo info = null;
                        try
                        {
                            info = JsonConvert.DeserializeObject<JobApplicationInfo>(strResult);
                        }
                        catch { }

                        if (CacheManager.JobInfo == null)
                        {
                            if (info == null)
                            {
                                JobApplication model = new JobApplication()
                                {
                                    ApplicationStatus = InitialData.ApplicationStatus.New,
                                    IsLocked = false
                                };

                                JobApplication final = Database.InsertJobApplication(model);

                                CacheManager.JobApplicationID = final.ID;

                                CacheManager.JobInfo = new JobApplicationInfo()
                                {
                                    ApplicationStatus = InitialData.ApplicationStatus.New,
                                    IsLocked = false
                                };
                            }
                            else
                            {
                                CacheManager.JobInfo = info;
                                JobApplication model = new JobApplication()
                                {
                                    Title = info.Title,
                                    FirstName = info.FirstName,
                                    LastName = info.LastName,
                                    PositionApplied = info.PositionApplied,
                                    KnownAs = info.KnownAs,
                                    ChineseCharacter = info.ChineseCharacter,
                                    Gender = info.Gender,
                                    DateOfBirth = info.DateOfBirth,
                                    CountryOfBirth = info.CountryOfBirth,
                                    Nationality = info.Nationality,
                                    Race = info.Race,
                                    MaritalStatus = info.MaritalStatus,
                                    Religion = info.Religion,
                                    IdentityNo = info.IdentityNo,
                                    DateOfIssue = info.DateOfIssue,
                                    DateOfExpiry = info.DateOfExpiry,
                                    CountryOfIssue = info.CountryOfIssue,
                                    EmailAddress = info.EmailAddress,
                                    ApplicationStatus = info.ApplicationStatus,
                                    IsLocked = info.IsLocked.GetValueOrDefault()
                                };

                                JobApplication final = Database.InsertJobApplication(model);

                                CacheManager.JobApplicationID = final.ID;
                            }
                        }
                    }
                }
            }
            catch { }
        }
    }
}