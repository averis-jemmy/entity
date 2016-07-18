package id.zenmorf.com.mapsapps.UI;

import android.app.ActivityManager;
import android.app.Notification;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothDevice;
import android.bluetooth.BluetoothManager;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.pm.PackageManager;
import android.graphics.Color;
import android.location.Location;
import android.location.LocationListener;
import android.location.LocationManager;
import android.os.Build;
import android.os.Handler;
import android.support.v4.app.ActivityCompat;
import android.support.v4.app.FragmentActivity;
import android.os.Bundle;
import android.support.v4.content.ContextCompat;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

import com.google.android.gms.maps.CameraUpdateFactory;
import com.google.android.gms.maps.GoogleMap;
import com.google.android.gms.maps.OnMapReadyCallback;
import com.google.android.gms.maps.SupportMapFragment;
import com.google.android.gms.maps.model.LatLng;
import com.google.android.gms.maps.model.Marker;
import com.google.android.gms.maps.model.MarkerOptions;
import com.google.android.gms.maps.model.PolylineOptions;

import java.text.DecimalFormat;
import java.util.ArrayList;
import java.util.List;

import id.zenmorf.com.mapsapps.BusinessLogic.CacheManager;
import id.zenmorf.com.mapsapps.R;

public class MapsActivity extends FragmentActivity implements OnMapReadyCallback {

    private boolean mIsBluetooth = false;
    private GoogleMap mMap;
    Marker mMarker;
    ArrayList<Location> mLocations = new ArrayList<Location>();
    private LocationManager mLocationManager;
    private MyLocationListener mLocationListener;
    private BluetoothAdapter mBluetoothAdapter;
    NotificationManager mNotificationManager;
    private double totDistance = 0, latA = 0, longA = 0, latB = 0, longB = 0;
    // The minimum distance to change Updates in meters
    private static final long MIN_DISTANCE_CHANGE_FOR_UPDATES = 0; // 0 meters
    // The minimum time between updates in milliseconds
    private static final long MIN_TIME_BW_UPDATES = 1000 * 3 * 1; // 5 seconds
    /* GPS Constant Permission */
    private static final int MY_PERMISSION_ACCESS_COARSE_LOCATION = 11;
    private static final int MY_PERMISSION_ACCESS_FINE_LOCATION = 12;
    private int mBluetoothTrial = 0;

    private boolean isStopping = false;
    private static final int NOTIFICATION_ID = 0;

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

                    TextView tvDistance = (TextView)findViewById(R.id.tv_distance);
                    tvDistance.setText("Active");
                    TextView tvBluetooth = (TextView)findViewById(R.id.tv_bluetooth);
                    if(rssi > -65)
                        tvBluetooth.setText("Good");
                    else if(rssi > -75)
                        tvBluetooth.setText("Moderate");
                    else if (rssi <= -75)
                        tvBluetooth.setText("Weak");
                }
            }

            if (BluetoothAdapter.ACTION_DISCOVERY_FINISHED.equals(action)) {
                if(!mIsBluetooth) {
                    if(mBluetoothTrial < 3)
                        mBluetoothTrial++;
                    else {
                        TextView tvDistance = (TextView) findViewById(R.id.tv_distance);
                        tvDistance.setText("Not Active");
                        TextView tvBluetooth = (TextView) findViewById(R.id.tv_bluetooth);
                        tvBluetooth.setText("Not Found");
                    }
                }
            }
        }
    };


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
                    //mBluetoothAdapter.disable();
                    //Thread.sleep(5000);
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

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_maps);
        SupportMapFragment mapFragment = (SupportMapFragment) getSupportFragmentManager()
                .findFragmentById(R.id.map);
        mapFragment.getMapAsync(this);

        String ns = Context.NOTIFICATION_SERVICE;
        mNotificationManager = (NotificationManager)getSystemService(ns);

        if(CacheManager.BluetoothType.equalsIgnoreCase("LOW ENERGY")) {
            if (!getPackageManager().hasSystemFeature(PackageManager.FEATURE_BLUETOOTH_LE)) {
                Toast.makeText(this, "BLE is not supported", Toast.LENGTH_SHORT).show();
                finish();
            }

            final BluetoothManager bluetoothManager = (BluetoothManager) getSystemService(Context.BLUETOOTH_SERVICE);
            mBluetoothAdapter = bluetoothManager.getAdapter();

            if (mBluetoothAdapter == null) {
                Toast.makeText(this, "Bluetooth is not supported", Toast.LENGTH_SHORT).show();
                finish();
            }
        }

        mLeHandler = new Handler();
        mLocationManager = (LocationManager)getSystemService(LOCATION_SERVICE);
        mLocationListener = new MyLocationListener();

        mHandler.postDelayed(mRun, 0);

        if(CacheManager.BluetoothType.equalsIgnoreCase("CLASSIC")) {
            mBluetoothAdapter = BluetoothAdapter.getDefaultAdapter();
            // Register for broadcasts when a device is discovered
            IntentFilter filter = new IntentFilter(BluetoothDevice.ACTION_FOUND);
            this.registerReceiver(mReceiver, filter);

            filter = new IntentFilter(BluetoothAdapter.ACTION_DISCOVERY_FINISHED);
            this.registerReceiver(mReceiver, filter);
        }

        try {
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
        }
        catch (Exception ex) {
            Log.e("Location", ex.getMessage());
        }

        Button btnStop = (Button)findViewById(R.id.btn_stop);
        btnStop.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                onStopClicked();
            }
        });
    }

    public void onStopClicked(){
        mHandler.removeCallbacks(mRun);
        try {
            mBluetoothAdapter.disable();
            if(CacheManager.BluetoothType.equalsIgnoreCase("LOW ENERGY")) {
                scanLeDevice(false);
            }
            if(CacheManager.BluetoothType.equalsIgnoreCase("CLASSIC")) {
                this.unregisterReceiver(mReceiver);
            }
            //Thread.sleep(5000);
        }
        catch (Exception ex) {
            Log.e("Location", ex.getMessage());
        }

        try {
            if (ContextCompat.checkSelfPermission(this, android.Manifest.permission.ACCESS_COARSE_LOCATION) != PackageManager.PERMISSION_GRANTED) {
                ActivityCompat.requestPermissions(this, new String[]{android.Manifest.permission.ACCESS_COARSE_LOCATION},
                        MY_PERMISSION_ACCESS_COARSE_LOCATION);
            }
            if (ContextCompat.checkSelfPermission(this, android.Manifest.permission.ACCESS_FINE_LOCATION) != PackageManager.PERMISSION_GRANTED) {
                ActivityCompat.requestPermissions(this, new String[]{android.Manifest.permission.ACCESS_FINE_LOCATION},
                        MY_PERMISSION_ACCESS_FINE_LOCATION);
            }
            mLocationManager.removeUpdates(mLocationListener);
        }
        catch (Exception ex) {
            Log.e("Location", ex.getMessage());
        }

        isStopping = true;

        try {
            mNotificationManager.cancelAll();
        } catch (Exception ex) {

        }

        finish();
    }

    @Override
    public void onMapReady(GoogleMap googleMap) {
        mMap = googleMap;

        // Add a marker in Sydney, Australia, and move the camera.
        //LatLng sydney = new LatLng(-34, 151);
        //mMap.addMarker(new MarkerOptions().position(sydney).title("Marker in Sydney"));
        //mMap.moveCamera(CameraUpdateFactory.newLatLng(sydney));
    }

    public class MyLocationListener implements LocationListener {
        public void onLocationChanged(Location location)
        {
            if (location != null) {
                // Getting latitude of the current location
                double latitude = location.getLatitude();
                // Getting longitude of the current location
                double longitude = location.getLongitude();
                // Getting accuracy of the current location
                double accuracy = location.getAccuracy();

                TextView tvLatitude = (TextView)findViewById(R.id.tv_latitude);
                tvLatitude.setText(String.valueOf(latitude));
                TextView tvLongitude = (TextView)findViewById(R.id.tv_longitude);
                tvLongitude.setText(String.valueOf(longitude));
                TextView tvAccuracy = (TextView)findViewById(R.id.tv_accuracy);
                tvAccuracy.setText(new DecimalFormat("##.##").format(accuracy) + " M");

                if(accuracy < 25 && accuracy > 0) {
                    // Creating a LatLng object for the current location
                    LatLng latLng = new LatLng(latitude, longitude);
                    // Showing the current location in Google Map
                    mMap.moveCamera(CameraUpdateFactory.newLatLng(latLng));
                    // Zoom in the Google Map
                    mMap.animateCamera(CameraUpdateFactory.zoomTo(15));

                    mLocations.add(location);

                    if (latA == 0 && longA == 0) {
                        mMarker = mMap.addMarker(new MarkerOptions().position(latLng).title("New Marker").draggable(false));
                        latA = location.getLatitude();
                        longA = location.getLongitude();
                    } else {
                        latB = location.getLatitude();
                        longB = location.getLongitude();

                        mMarker.setPosition(latLng);
                        drawPrimaryLinePath(mLocations);

                        totDistance += getDistance(latA, longA, latB, longB);

                        //TextView tvDistance = (TextView)findViewById(R.id.tv_distance);
                        //tvDistance.setText(new DecimalFormat("##.##").format(totDistance) + " M");

                        latA = latB;
                        longA = longB;
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

    private double getDistance(double latitudeA, double longitudeA, double latitudeB, double longitudeB)
    {
        double distance;
        Location locationA = new Location("LocationA");
        locationA.setLatitude(latitudeA);
        locationA.setLongitude(longitudeA);
        Location locationB = new Location("LocationB");
        locationB.setLatitude(latitudeB);
        locationB.setLongitude(longitudeB);
        distance = locationA.distanceTo(locationB);

        return distance;
    }

    private void drawPrimaryLinePath( ArrayList<Location> listLocsToDraw )
    {
        if ( mMap == null )
        {
            return;
        }

        if ( listLocsToDraw.size() < 2 )
        {
            return;
        }

        PolylineOptions options = new PolylineOptions();

        options.color( Color.parseColor("#CC0000FF") );
        options.width(5);
        options.visible(true);

        for ( Location locRecorded : listLocsToDraw )
        {
            options.add( new LatLng( locRecorded.getLatitude(),
                    locRecorded.getLongitude() ) );
        }

        mMap.addPolyline(options);
    }

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
                            TextView tvDistance = (TextView) findViewById(R.id.tv_distance);
                            tvDistance.setText("Not Active");
                            TextView tvBluetooth = (TextView) findViewById(R.id.tv_bluetooth);
                            tvBluetooth.setText("Not Found");
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

                        TextView tvDistance = (TextView) findViewById(R.id.tv_distance);
                        tvDistance.setText("Active");
                        TextView tvBluetooth = (TextView) findViewById(R.id.tv_bluetooth);
                        if (rssi > -65)
                            tvBluetooth.setText("Good");
                        else if (rssi > -75)
                            tvBluetooth.setText("Moderate");
                        else if (rssi <= -75)
                            tvBluetooth.setText("Weak");

                        try {
                            scanLeDevice(false);
                        } catch (Exception ex) {

                        }
                    }
                }
            }));
        }
    };

    private Intent getPreviousIntent() {
        Intent newIntent = null;
        final ActivityManager activityManager = (ActivityManager) getSystemService(Context.ACTIVITY_SERVICE);
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP) {
            final List<ActivityManager.AppTask> recentTaskInfos = activityManager.getAppTasks();
            if (!recentTaskInfos.isEmpty()) {
                for (ActivityManager.AppTask appTaskTaskInfo : recentTaskInfos) {
                    if (appTaskTaskInfo.getTaskInfo().baseIntent.getComponent().getPackageName().equals("id.zenmorf.com.mapsapps")) {
                        newIntent = appTaskTaskInfo.getTaskInfo().baseIntent;
                        newIntent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
                    }
                }
            }
        } else {
            final List<ActivityManager.RecentTaskInfo> recentTaskInfos = activityManager.getRecentTasks(1024, 0);
            if (!recentTaskInfos.isEmpty()) {
                for (ActivityManager.RecentTaskInfo recentTaskInfo : recentTaskInfos) {
                    if (recentTaskInfo.baseIntent.getComponent().getPackageName().equals("id.zenmorf.com.mapsapps")) {
                        newIntent = recentTaskInfo.baseIntent;
                        newIntent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
                    }
                }
            }
        }
        if (newIntent == null) newIntent = new Intent();
        return newIntent;
    }

    @Override
     public void onBackPressed() {
        return;
    }

    @Override
    public void onResume() {
        try {
            mNotificationManager.cancelAll();
        } catch (Exception ex) {

        }

        super.onResume();
    }

    @Override
    public void onPause() {
        try {
            if(!isStopping) {
                Intent nIntent = getPreviousIntent();
                PendingIntent pi = PendingIntent.getActivity(this, 0, nIntent, 0);

                Notification notification = new Notification.Builder(CacheManager.CacheContext)
                        .setContentTitle("NEW NOTIFICATION")
                        .setContentText("HELLO!!! YOU REALLY SHOULD FIX THIS")
                        .setContentIntent(pi)
                        .setSmallIcon(R.drawable.icon)
                        .build();

                notification.flags = Notification.FLAG_NO_CLEAR;

                mNotificationManager.notify(NOTIFICATION_ID, notification);
            }
        }
        catch (Exception ex) {

        }

        super.onPause();
    }

    @Override
    protected void onDestroy()
    {
        onStopClicked();
        super.onDestroy();
    }
}