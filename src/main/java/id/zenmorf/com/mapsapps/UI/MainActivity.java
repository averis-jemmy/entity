package id.zenmorf.com.mapsapps.UI;

import android.app.Activity;
import android.app.Application;
import android.app.NotificationManager;
import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothDevice;
import android.bluetooth.BluetoothManager;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.pm.PackageManager;
import android.content.res.Configuration;
import android.graphics.drawable.GradientDrawable;
import android.location.Location;
import android.location.LocationListener;
import android.location.LocationManager;
import android.os.Bundle;
import android.os.Handler;
import android.provider.Settings;
import android.support.design.widget.NavigationView;
import android.support.v4.app.ActivityCompat;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentManager;
import android.support.v4.content.ContextCompat;
import android.support.v4.widget.DrawerLayout;
import android.support.v7.app.ActionBarDrawerToggle;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.widget.Toolbar;
import android.util.DisplayMetrics;
import android.util.Log;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.view.inputmethod.InputMethodManager;
import android.widget.ImageView;
import android.widget.RelativeLayout;
import android.widget.TextView;
import android.widget.Toast;

import com.android.volley.AuthFailureError;
import com.android.volley.NetworkResponse;
import com.android.volley.Request;
import com.android.volley.Response;
import com.android.volley.VolleyError;
import com.android.volley.toolbox.JsonObjectRequest;

import org.json.JSONException;
import org.json.JSONObject;

import java.util.HashMap;
import java.util.List;
import java.util.Map;

import id.zenmorf.com.mapsapps.BusinessLogic.CacheManager;
import id.zenmorf.com.mapsapps.DataStorage.DBHandler;
import id.zenmorf.com.mapsapps.Dialog.LogoutDialog;
import id.zenmorf.com.mapsapps.Model.LocationInfo;
import id.zenmorf.com.mapsapps.BusinessLogic.MyApplication;
import id.zenmorf.com.mapsapps.R;
import id.zenmorf.com.mapsapps.DataStorage.SettingsHelper;
import id.zenmorf.com.mapsapps.BusinessLogic.VolleySingleton;

/**
 * Created by hp on 2/3/2016.
 */
public class MainActivity extends AppCompatActivity {
    private DrawerLayout mDrawer;
    private Toolbar toolbar;
    private NavigationView nvDrawer;
    private ActionBarDrawerToggle drawerToggle;

    private boolean isValid = false;
    private BluetoothAdapter mBluetoothAdapter;
    private int mBluetoothTrial = 0;
    private boolean mIsBluetooth = false;

    private LocationManager mLocationManager;

    private static final int REQUEST_LOGOUT = 0;
    private static final int REQUEST_BLUETOOTH = 1;

    private boolean isTracking = false;
    private boolean isStarting = true;

    private MyLocationListener mLocationListener;
    // The minimum distance to change Updates in meters
    private static final long MIN_DISTANCE_CHANGE_FOR_UPDATES = 0; // 0 meters
    // The minimum time between updates in milliseconds
    private static final long MIN_TIME_BW_UPDATES = 1000 * 5 * 1; // 5 seconds
    /* GPS Constant Permission */
    private static final int MY_PERMISSION_ACCESS_COARSE_LOCATION = 11;
    private static final int MY_PERMISSION_ACCESS_FINE_LOCATION = 12;

    private Handler mLeHandler;
    private static final long SCAN_PERIOD = 10000;

    private Handler mHandler = new Handler();
    private Runnable mRun = new Runnable() {
        @Override
        public void run() {
            try {
                try {
                    if(!mBluetoothAdapter.isEnabled()) {
                        mBluetoothAdapter.enable();
                    }
                    mIsBluetooth = false;
                    if(CacheManager.BluetoothType.equalsIgnoreCase("CLASSIC")) {
                        mBluetoothAdapter.startDiscovery();
                    }
                    if (CacheManager.BluetoothType.equalsIgnoreCase("LOW ENERGY")) {
                        scanLeDevice(true);
                    }
                }
                catch (Exception ex) {

                }
            }
            catch (Exception ex) {
                Log.e("Bluetooth", ex.getMessage());
            }
            mHandler.postDelayed(this, 10000);
        }
    };

    private void scanLeDevice(final boolean enable) {
        if(enable) {
            mLeHandler.postDelayed(new Runnable() {
                @Override
                public void run() {
                    mBluetoothAdapter.stopLeScan(mLeScanCallback);

                    if(!mIsBluetooth) {
                        if(mBluetoothTrial < 3)
                            mBluetoothTrial++;
                        else {
                            TextView tvOutOfRange = (TextView) findViewById(R.id.tv_out_of_range);
                            tvOutOfRange.setVisibility(View.VISIBLE);
                            DisplayMetrics metrics = CacheManager.CacheContext.getResources().getDisplayMetrics();
                            float dp = 5f;
                            float fpixels = metrics.density * dp;
                            int pixels = (int) (fpixels + 0.5f);

                            ImageView imgCircle = (ImageView)findViewById(R.id.img_circle);
                            GradientDrawable drawCircle = (GradientDrawable)imgCircle.getBackground();
                            drawCircle.setStroke(pixels, ContextCompat.getColor(CacheManager.CacheContext, R.color.iron));
                            StopTracking();
                        }
                    }
                }
            }, SCAN_PERIOD);
            mBluetoothAdapter.startLeScan(mLeScanCallback);
        } else {
            mBluetoothAdapter.stopLeScan(mLeScanCallback);
        }
    }

    private BluetoothAdapter.LeScanCallback mLeScanCallback = new BluetoothAdapter.LeScanCallback() {
        @Override
        public void onLeScan(final BluetoothDevice device, final int rssi, byte[] scanRecord) {
            runOnUiThread((new Runnable() {
                @Override
                public void run() {
                    if (device.getAddress().equalsIgnoreCase(CacheManager.BluetoothMACAddress)) {
                        mIsBluetooth = true;
                        mBluetoothTrial = 0;
                        DisplayMetrics metrics = CacheManager.CacheContext.getResources().getDisplayMetrics();
                        float dp = 5f;
                        float fpixels = metrics.density * dp;
                        int pixels = (int) (fpixels + 0.5f);

                        ImageView imgCircle = (ImageView)findViewById(R.id.img_circle);
                        GradientDrawable drawCircle = (GradientDrawable)imgCircle.getBackground();
                        drawCircle.setStroke(pixels, ContextCompat.getColor(CacheManager.CacheContext, R.color.light_orange));

                        TextView tvOutOfRange = (TextView) findViewById(R.id.tv_out_of_range);
                        tvOutOfRange.setVisibility(View.GONE);

                        try {
                            scanLeDevice(false);
                        } catch (Exception ex) {

                        }

                        if(!isTracking)
                            StartTracking();
                    }
                }
            }));
        }
    };

    // Create a BroadcastReceiver for ACTION_FOUND
    private final BroadcastReceiver mReceiver = new BroadcastReceiver() {
        @Override
        public void onReceive(Context context, Intent intent) {
            String action = intent.getAction();

            // When discovery finds a device
            if (BluetoothDevice.ACTION_FOUND.equals(action)) {
                // Get the BluetoothDevice object from the Intent
                int rssi = intent.getShortExtra(BluetoothDevice.EXTRA_RSSI,Short.MIN_VALUE);
                BluetoothDevice device = intent.getParcelableExtra(BluetoothDevice.EXTRA_DEVICE);
                // If it's already paired, skip it, because it's been listed already
                if (device.getAddress().equalsIgnoreCase(CacheManager.BluetoothMACAddress))
                {
                    try {
                        mBluetoothAdapter.cancelDiscovery();
                    } catch(Exception ex) {

                    }
                    mIsBluetooth = true;
                    mBluetoothTrial = 0;
                    DisplayMetrics metrics = CacheManager.CacheContext.getResources().getDisplayMetrics();
                    float dp = 5f;
                    float fpixels = metrics.density * dp;
                    int pixels = (int) (fpixels + 0.5f);


                    ImageView imgCircle = (ImageView)findViewById(R.id.img_circle);
                    GradientDrawable drawCircle = (GradientDrawable)imgCircle.getBackground();
                    drawCircle.setStroke(pixels, ContextCompat.getColor(CacheManager.CacheContext, R.color.light_orange));

                    TextView tvOutOfRange = (TextView) findViewById(R.id.tv_out_of_range);
                    tvOutOfRange.setVisibility(View.GONE);

                    if(!isTracking)
                        StartTracking();
                }
            }

            if (BluetoothAdapter.ACTION_DISCOVERY_FINISHED.equals(action)) {
                if(!mIsBluetooth) {
                    if(mBluetoothTrial < 3)
                        mBluetoothTrial++;
                    else {
                        TextView tvOutOfRange = (TextView) findViewById(R.id.tv_out_of_range);
                        tvOutOfRange.setVisibility(View.VISIBLE);
                        DisplayMetrics metrics = CacheManager.CacheContext.getResources().getDisplayMetrics();
                        float dp = 5f;
                        float fpixels = metrics.density * dp;
                        int pixels = (int) (fpixels + 0.5f);

                        ImageView imgCircle = (ImageView)findViewById(R.id.img_circle);
                        GradientDrawable drawCircle = (GradientDrawable)imgCircle.getBackground();
                        drawCircle.setStroke(pixels, ContextCompat.getColor(CacheManager.CacheContext, R.color.iron));

                        StopTracking();
                    }
                }
            }
        }
    };


    public class MyLocationListener implements LocationListener {
        public void onLocationChanged(Location location)
        {
            if (location != null) {
                double latitude = location.getLatitude();
                double longitude = location.getLongitude();
                double accuracy = location.getAccuracy();

                if(accuracy < 50 && accuracy > 0) {
                    CacheManager.LastLocation = location;

                    if(isStarting) {
                        try {
                            isStarting = false;
                            if(isInternetAvailable()) {
                                pushDBTrackingData();

                                pushWebServiceData(-1, "start", CacheManager.TokenID, CacheManager.LastLocation.getLatitude(), CacheManager.LastLocation.getLongitude(), CacheManager.LastLocation.getAccuracy(), (int) System.currentTimeMillis());
                            }
                            else {
                                saveIntoDB("start", CacheManager.TokenID, latitude, longitude, accuracy, (int) System.currentTimeMillis());
                            }
                        } catch (Exception ex) {
                            isStarting = true;
                        }
                    }
                    else {
                        try {
                            if(isInternetAvailable()) {
                                pushDBTrackingData();

                                pushWebServiceData(-1, "location", CacheManager.TokenID, latitude, longitude, accuracy, (int) System.currentTimeMillis());
                            }
                            else {
                                saveIntoDB("location", CacheManager.TokenID, latitude, longitude, accuracy, (int)System.currentTimeMillis());
                            }
                        } catch (Exception ex) {

                        }
                    }
                }
            }
        }

        public void onProviderDisabled(String provider)
        {
            Toast.makeText(getApplicationContext(), "Gps Disabled", Toast.LENGTH_SHORT).show();
        }
        public void onProviderEnabled(String provider)
        {
            Toast.makeText( getApplicationContext(), "Gps Enabled", Toast.LENGTH_SHORT).show();
        }
        public void onStatusChanged(String provider, int status, Bundle extras)
        {

        }
    }

    public void StartTracking() {
        try {
            CacheManager.IsStopping = false;

            if (ContextCompat.checkSelfPermission(this, android.Manifest.permission.ACCESS_COARSE_LOCATION) != PackageManager.PERMISSION_GRANTED) {
                ActivityCompat.requestPermissions(this, new String[]{android.Manifest.permission.ACCESS_COARSE_LOCATION},
                        MY_PERMISSION_ACCESS_COARSE_LOCATION);
            }
            if (ContextCompat.checkSelfPermission(this, android.Manifest.permission.ACCESS_FINE_LOCATION) != PackageManager.PERMISSION_GRANTED) {
                ActivityCompat.requestPermissions(this, new String[]{android.Manifest.permission.ACCESS_FINE_LOCATION},
                        MY_PERMISSION_ACCESS_FINE_LOCATION);
            }

            if(mLocationManager.isProviderEnabled(LocationManager.GPS_PROVIDER)) {
                mLocationManager.requestLocationUpdates(LocationManager.GPS_PROVIDER, MIN_TIME_BW_UPDATES, MIN_DISTANCE_CHANGE_FOR_UPDATES, mLocationListener);
            }
            if(mLocationManager.isProviderEnabled(LocationManager.NETWORK_PROVIDER)) {
                mLocationManager.requestLocationUpdates(LocationManager.NETWORK_PROVIDER, MIN_TIME_BW_UPDATES, MIN_DISTANCE_CHANGE_FOR_UPDATES, mLocationListener);
            }

            isTracking = true;
            isStarting = true;

            CacheManager.LastDistance = "0";
            TextView tvDistance = (TextView) findViewById(R.id.tv_distance);
            tvDistance.setText("0");
        }
        catch (Exception ex) {
            Log.e("Location", ex.getMessage());
        }
    }

    public void StopTracking() {
        try {
            if(isTracking) {
                if (ContextCompat.checkSelfPermission(this, android.Manifest.permission.ACCESS_COARSE_LOCATION) != PackageManager.PERMISSION_GRANTED) {
                    ActivityCompat.requestPermissions(this, new String[]{android.Manifest.permission.ACCESS_COARSE_LOCATION},
                            MY_PERMISSION_ACCESS_COARSE_LOCATION);
                }
                if (ContextCompat.checkSelfPermission(this, android.Manifest.permission.ACCESS_FINE_LOCATION) != PackageManager.PERMISSION_GRANTED) {
                    ActivityCompat.requestPermissions(this, new String[]{android.Manifest.permission.ACCESS_FINE_LOCATION},
                            MY_PERMISSION_ACCESS_FINE_LOCATION);
                }

                try {
                    mLocationManager.removeUpdates(mLocationListener);
                } catch (Exception ex) {

                }

                try {
                    if(isInternetAvailable()) {
                        pushDBTrackingData();

                        pushWebServiceData(-1, "stop", CacheManager.TokenID, CacheManager.LastLocation.getLatitude(), CacheManager.LastLocation.getLongitude(), CacheManager.LastLocation.getAccuracy(), (int) System.currentTimeMillis());
                    }
                    else {
                        saveIntoDB("stop", CacheManager.TokenID, CacheManager.LastLocation.getLatitude(), CacheManager.LastLocation.getLongitude(), CacheManager.LastLocation.getAccuracy(), (int) System.currentTimeMillis());
                    }
                } catch (Exception ex) {

                }
                isTracking = false;
                CacheManager.IsStopping = true;

                CacheManager.LastDistance = "0";
                TextView tvDistance = (TextView) findViewById(R.id.tv_distance);
                tvDistance.setText("0");
            }
        }
        catch (Exception ex) {
            Log.e("Location", ex.getMessage());
        }
    }

    private void pushWebServiceData(final int id, final String action, final String tokenID, final double latitude, final double longitude, final double accuracy, final int currentTimeMillis) {
        Map<String, String> params = new HashMap<>();
        params.put("lat", String.valueOf(latitude));
        params.put("long", String.valueOf(longitude));
        params.put("timestamp", String.valueOf(currentTimeMillis));

        try {
            Log.e("Request", (new JSONObject(params)).toString());
        } catch (Exception e) {
            e.printStackTrace();
        }

        String url = "http://mobweb.cloudapp.net:81/api/v1/trip/" + action;
        JsonObjectRequest postRequest = new JsonObjectRequest(Request.Method.POST, url, new JSONObject(params),
                new Response.Listener<JSONObject>() {
                    @Override
                    public void onResponse(JSONObject response) {
                        try {
                            JSONObject jsonResponse = response.getJSONObject("result");
                            String status = jsonResponse.getString("status");

                            Log.e("Response Update", response.toString());

                            if(action.equals("start")) {
                                if(!status.equals("TRIP_START_SUCCESS")) {
                                    if(id < 0)
                                        saveIntoDB(action, tokenID, latitude, longitude, accuracy, currentTimeMillis);
                                }
                                else {
                                    if(id >= 0)
                                        deleteDBData(id);
                                }
                            }
                            if(action.equals("location")) {
                                if(!status.equals("LOCATION_UPDATE_SUCCESS")) {
                                    if(id < 0)
                                        saveIntoDB(action, tokenID, latitude, longitude, accuracy, currentTimeMillis);
                                }
                                else {
                                    if(id >= 0)
                                        deleteDBData(id);
                                }
                            }
                            if(action.equals("stop")) {
                                if(!status.equals("TRIP_STOP_SUCCESS")) {
                                    if(id < 0)
                                        saveIntoDB(action, tokenID, latitude, longitude, accuracy, currentTimeMillis);
                                }
                                else {
                                    if(id >= 0)
                                        deleteDBData(id);
                                }
                            }

                            try {
                                JSONObject jsonData = jsonResponse.getJSONObject("data");
                                String distance = jsonData.getString("distance");
                                int dist;
                                try {
                                    dist = Integer.valueOf(distance) / 1000;
                                    distance = String.valueOf(dist);
                                } catch (Exception ex) {
                                    ex.printStackTrace();
                                }

                                CacheManager.LastDistance = distance;

                                TextView tvDistance = (TextView) findViewById(R.id.tv_distance);
                                tvDistance.setText(distance);
                            } catch (Exception ex) {
                                ex.printStackTrace();
                            }
                        } catch (JSONException e) {
                            e.printStackTrace();
                            if(id < 0)
                                saveIntoDB(action, tokenID, latitude, longitude, accuracy, currentTimeMillis);
                        }
                    }
                },
                new Response.ErrorListener() {
                    @Override
                    public void onErrorResponse(VolleyError error) {
                        NetworkResponse networkResponse = error.networkResponse;
                        if (networkResponse != null && (networkResponse.statusCode == 401 || networkResponse.statusCode == 403)) {
                            // HTTP Status Code: 401 Unauthorized Or 403 Forbidden
                            onLogoutPressed();
                        } else {
                            if (id < 0)
                                saveIntoDB(action, tokenID, latitude, longitude, accuracy, currentTimeMillis);
                        }
                    }
                }) {
            @Override
            public Map<String, String> getHeaders() throws AuthFailureError {
                HashMap<String, String> headers = new HashMap<String, String>();
                headers.put("auth", tokenID);
                return headers;
            }
        };

        //Volley.newRequestQueue(this).add(postRequest);
        VolleySingleton.getInstance(this).addToRequestQueue(postRequest);
    }

    private void saveIntoDB(String action, String tokenID, double latitude, double longitude, double accuracy, int currentTimeMillis) {
        DBHandler dbHandler = new DBHandler(CacheManager.CacheContext, null, null, 1);
        LocationInfo locationInfo =
                new LocationInfo(action, tokenID, latitude, longitude, accuracy, currentTimeMillis);
        dbHandler.addLocation(locationInfo);
    }

    private void deleteDBData(int id) {
        DBHandler dbHandler = new DBHandler(CacheManager.CacheContext, null, null, 1);
        dbHandler.deleteLocation(id);
    }

    private void clearDBData() {
        DBHandler dbHandler = new DBHandler(CacheManager.CacheContext, null, null, 1);
        List<LocationInfo> locations = dbHandler.findLocation();
        for (LocationInfo info: locations) {
            dbHandler.deleteLocation(info.getID());
        }
    }

    private void pushDBTrackingData() {
        DBHandler dbHandler = new DBHandler(CacheManager.CacheContext, null, null, 1);
        List<LocationInfo> locations = dbHandler.findLocation();
        for (LocationInfo info: locations) {
            pushWebServiceData(info.getID(), info.getAction(), CacheManager.TokenID, info.getLatitude(), info.getLongitude(), info.getAccuracy(), info.getCurrentTimeMillis());
        }
    }

    public boolean isInternetAvailable() {
        try {
            Runtime runtime = Runtime.getRuntime();
            try {
                Process ipProcess = runtime.exec("/system/bin/ping -c 1 8.8.8.8");
                int     exitValue = ipProcess.waitFor();
                return (exitValue == 0);
            } catch (Exception e) {
                return false;
            }
        } catch (Exception ex) {
            return false;
        }
    }

    public void selectDrawerItem(MenuItem menuItem) {
        if(menuItem.isChecked())
            menuItem.setChecked(false);

        mDrawer.closeDrawers();

        RelativeLayout layoutTracker = (RelativeLayout)findViewById(R.id.layout_tracker);
        layoutTracker.setVisibility(View.GONE);

        Fragment fragment;
        Class fragmentClass = null;
        fragmentClass = HomeFragment.class;

        switch(menuItem.getItemId()) {
            case R.id.nav_home:
                fragmentClass = HomeFragment.class;
                layoutTracker.setVisibility(View.VISIBLE);
                break;
            case R.id.nav_account:
                fragmentClass = ProfileFragment.class;
                break;
            case R.id.nav_messages:
                break;
            case R.id.nav_campaign:
                break;
            case R.id.nav_logout:
                fragmentClass = HomeFragment.class;
                layoutTracker.setVisibility(View.VISIBLE);
                LogoutDialog cdd = new LogoutDialog(MainActivity.this);
                cdd.show();
                break;
            case R.id.nav_about:
                fragmentClass = AboutFragment.class;
                break;
            case R.id.nav_exit:
                StopTracking();
                try {
                    MyApplication myApp = (MyApplication)this.getApplication();
                    if (myApp.wasInBackground)
                    {
                        //Do specific came-here-from-background code
                        myApp.StopNotification();
                    }

                    myApp.stopActivityTransitionTimer();
                } catch (Exception ex) {

                }
                finish();
                break;
            default:
                break;
        }

        try {
            if(fragmentClass != null) {
                fragment = (Fragment) fragmentClass.newInstance();

                FragmentManager fragmentManager = getSupportFragmentManager();
                fragmentManager.beginTransaction().replace(R.id.flContent, fragment).commit();
            }
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    private ActionBarDrawerToggle setupDrawerToggle() {
        return new ActionBarDrawerToggle(this, mDrawer, toolbar, R.string.drawer_open, R.string.drawer_close) {
            public void onDrawerClosed(View view) {
                invalidateOptionsMenu();
            }

            public void onDrawerOpened(View view) {
                InputMethodManager inputMethodManager = (InputMethodManager) MainActivity.this.getSystemService(Activity.INPUT_METHOD_SERVICE);
                inputMethodManager.hideSoftInputFromWindow(MainActivity.this.getCurrentFocus().getWindowToken(), 0);
                nvDrawer.bringToFront();
                nvDrawer.requestLayout();
                invalidateOptionsMenu();
            }
        };
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        CacheManager.IsStopping = true;

        CacheManager.CacheContext = getApplicationContext();
        CacheManager.InitDevice();

        if(CacheManager.UserID.isEmpty()) {
            Intent intent = new Intent(this, LoginActivity.class);
            startActivity(intent);
            finish();
        }
        else {
            // Set a Toolbar to replace the ActionBar.
            toolbar = (Toolbar) findViewById(R.id.toolbar);
            setSupportActionBar(toolbar);

            // Find our drawer view
            mDrawer = (DrawerLayout) findViewById(R.id.drawer_layout);

            // Find our drawer view
            nvDrawer = (NavigationView) findViewById(R.id.nvView);
            // Setup drawer view
            nvDrawer.setNavigationItemSelectedListener(
                    new NavigationView.OnNavigationItemSelectedListener() {
                        @Override
                        public boolean onNavigationItemSelected(MenuItem menuItem) {
                            selectDrawerItem(menuItem);
                            return true;
                        }
                    });
            nvDrawer.setCheckedItem(R.id.nav_home);

            drawerToggle = setupDrawerToggle();

            // Tie DrawerLayout events to the ActionBarToggle
            mDrawer.addDrawerListener(drawerToggle);

            mLocationListener = new MyLocationListener();mLocationManager = (LocationManager) getSystemService(LOCATION_SERVICE);
            mLocationListener = new MyLocationListener();

            String ns = Context.NOTIFICATION_SERVICE;
            CacheManager.NotificationManagerInstance = (NotificationManager) getSystemService(ns);

            if (!mLocationManager.isProviderEnabled(LocationManager.GPS_PROVIDER)) {
                Toast.makeText(CacheManager.CacheContext, "Please enable your GPS", Toast.LENGTH_SHORT).show();
                startActivity(new Intent(Settings.ACTION_LOCATION_SOURCE_SETTINGS));
                finish();
            } else {
                if (CacheManager.CampaignID.isEmpty()) {
                    if (!isTracking)
                        StartTracking();
                } else {
                    if (CacheManager.BluetoothMACAddress.isEmpty()) {
                        Intent i = new Intent(MainActivity.this, BluetoothActivity.class);
                        startActivity(i);
                        finish();
                    } else {
                        mLeHandler = new Handler();

                        final BluetoothManager bluetoothManager = (BluetoothManager) getSystemService(Context.BLUETOOTH_SERVICE);
                        mBluetoothAdapter = bluetoothManager.getAdapter();

                        if (CacheManager.BluetoothType.equalsIgnoreCase("CLASSIC")) {
                            mBluetoothAdapter = BluetoothAdapter.getDefaultAdapter();
                            // Register for broadcasts when a device is discovered
                            IntentFilter filter = new IntentFilter(BluetoothDevice.ACTION_FOUND);
                            this.registerReceiver(mReceiver, filter);

                            filter = new IntentFilter(BluetoothAdapter.ACTION_DISCOVERY_FINISHED);
                            this.registerReceiver(mReceiver, filter);
                        }

                        mHandler.postDelayed(mRun, 0);

                        isValid = true;
                    }
                }
            }
        }
    }

    @Override
    public void onBackPressed() {
        if(!CacheManager.IsStopping) {
            return;
        }

        super.onBackPressed();
    }

    @Override
    public void onResume() {
        try {
            MyApplication myApp = (MyApplication)this.getApplication();
            if (myApp.wasInBackground)
            {
                //Do specific came-here-from-background code
                myApp.StopNotification();
            }

            myApp.stopActivityTransitionTimer();
        } catch (Exception ex) {
        }

        super.onResume();
    }

    @Override
    public void onPause() {
        try {
            if(!CacheManager.IsStopping) {
                ((MyApplication)this.getApplication()).startActivityTransitionTimer();
            }
        } catch (Exception ex) {
        }

        super.onPause();
    }

    @Override
    protected void onDestroy() {
        try {
            mHandler.removeCallbacks(mRun);
            mBluetoothAdapter.disable();
            if(CacheManager.BluetoothType.equalsIgnoreCase("LOW ENERGY")) {
                scanLeDevice(false);
            }
            if(CacheManager.BluetoothType.equalsIgnoreCase("CLASSIC")) {
                this.unregisterReceiver(mReceiver);
            }
        }
        catch (Exception ex) {
            Log.e("Location", ex.getMessage());
        }
            /*
        try {
            VolleySingleton.getInstance(this).getRequestQueue().cancelAll(new RequestQueue.RequestFilter() {
                @Override
                public boolean apply(Request<?> request) {
                    // do I have to cancel this?
                    return true; // -> always yes
                }
            });
        } catch (Exception ex) {
            Log.e("Location", ex.getMessage());
        }
            */

        super.onDestroy();
    }

    public void onBluetoothAccessFailed() {
        Toast.makeText(this, "Wrong password!", Toast.LENGTH_SHORT).show();
    }

    public void onBluetoothAccessPressed() {
        StopTracking();
        try {
            MyApplication myApp = (MyApplication)this.getApplication();
            if (myApp.wasInBackground)
            {
                //Do specific came-here-from-background code
                myApp.StopNotification();
            }

            myApp.stopActivityTransitionTimer();
        } catch (Exception ex) {

        }
        Intent i = new Intent(MainActivity.this, BluetoothActivity.class);
        startActivity(i);
        finish();
    }

    public void onLogoutFailed() {
        nvDrawer.setCheckedItem(R.id.nav_home);
    }

    public void onLogoutPressed() {
        StopTracking();
        try {
            MyApplication myApp = (MyApplication)this.getApplication();
            if (myApp.wasInBackground)
            {
                //Do specific came-here-from-background code
                myApp.StopNotification();
            }

            myApp.stopActivityTransitionTimer();
        } catch (Exception ex) {

        }
        SettingsHelper.saveLoginID(CacheManager.CacheContext, "");
        Intent i = new Intent(MainActivity.this, MainActivity.class);
        startActivity(i);
        finish();
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        if (requestCode == REQUEST_LOGOUT) {
            if (resultCode == RESULT_OK) {
                StopTracking();
                try {
                    MyApplication myApp = (MyApplication)this.getApplication();
                    if (myApp.wasInBackground)
                    {
                        //Do specific came-here-from-background code
                        myApp.StopNotification();
                    }

                    myApp.stopActivityTransitionTimer();
                } catch (Exception ex) {

                }
                SettingsHelper.saveLoginID(CacheManager.CacheContext, "");
                Intent i = new Intent(MainActivity.this, MainActivity.class);
                startActivity(i);
                finish();
            }
        }
        if (requestCode == REQUEST_BLUETOOTH) {
            if (resultCode == RESULT_OK) {
                StopTracking();
                try {
                    MyApplication myApp = (MyApplication)this.getApplication();
                    if (myApp.wasInBackground)
                    {
                        //Do specific came-here-from-background code
                        myApp.StopNotification();
                    }

                    myApp.stopActivityTransitionTimer();
                } catch (Exception ex) {

                }
                Intent i = new Intent(MainActivity.this, BluetoothActivity.class);
                startActivity(i);
                finish();
            }
        }
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        // Inflate the menu; this adds items to the action bar if it is present.
        //getMenuInflater().inflate(R.menu.menu_main, menu);

        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        // The action bar home/up action should open or close the drawer.
        if (drawerToggle.onOptionsItemSelected(item)) {
            return true;
        }

        return super.onOptionsItemSelected(item);
    }

    // `onPostCreate` called when activity start-up is complete after `onStart()`
    // NOTE! Make sure to override the method with only a single `Bundle` argument
    @Override
    protected void onPostCreate(Bundle savedInstanceState) {
        super.onPostCreate(savedInstanceState);
        // Sync the toggle state after onRestoreInstanceState has occurred.
        drawerToggle.syncState();
    }

    @Override
    public void onConfigurationChanged(Configuration newConfig) {
        super.onConfigurationChanged(newConfig);
        // Pass any configuration change to the drawer toggles
        drawerToggle.onConfigurationChanged(newConfig);
    }

    @Override
    public void onSaveInstanceState(Bundle outState) {
        super.onSaveInstanceState(outState);
    }

    @Override
    protected void onRestoreInstanceState(Bundle savedInstanceState) {
        super.onRestoreInstanceState(savedInstanceState);
    }
}