package id.zenmorf.com.mapsapps.UI;

import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothDevice;
import android.bluetooth.BluetoothManager;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.pm.PackageManager;
import android.os.Bundle;
import android.os.Handler;
import android.support.v7.app.AppCompatActivity;
import android.util.Log;
import android.view.View;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.LinearLayout;
import android.widget.ListView;
import android.widget.Toast;

import id.zenmorf.com.mapsapps.BusinessLogic.CacheManager;
import id.zenmorf.com.mapsapps.R;
import id.zenmorf.com.mapsapps.DataStorage.SettingsHelper;

/**
 * Created by hp on 9/3/2016.
 */
public class BluetoothActivity extends AppCompatActivity implements View.OnClickListener {

    LinearLayout _layoutBluetoothType;
    LinearLayout _layoutBluetoothSearch;

    private BluetoothAdapter mBluetoothAdapter;
    private ArrayAdapter<String> mArrayAdapter;
    private Context mContext;
    private Handler mHandler;
    private Button btnSearch;
    private Button btnBle;
    private Button btnClassic;
    private ListView lstNewDevices;

    private static final long SCAN_PERIOD = 15000;

    // Create a BroadcastReceiver for ACTION_FOUND
    private final BroadcastReceiver mReceiver = new BroadcastReceiver() {
        @Override
        public void onReceive(Context context, Intent intent) {
            String action = intent.getAction();

            if (BluetoothDevice.ACTION_FOUND.equals(action)) {
                BluetoothDevice device = intent.getParcelableExtra(BluetoothDevice.EXTRA_DEVICE);
                int rssi = intent.getShortExtra(BluetoothDevice.EXTRA_RSSI, Short.MIN_VALUE);
                String bluetooth = "Name : " + device.getName() + "\nRSSI : " + String.valueOf(rssi) + "\n" + device.getAddress();

                for(int i=0 ; i < mArrayAdapter.getCount() ; i++){
                    String obj = mArrayAdapter.getItem(i);
                    if(obj.contains(device.getName()) && obj.contains(device.getAddress())) {
                        mArrayAdapter.remove(obj);
                    }
                }

                mArrayAdapter.add(bluetooth);
                mArrayAdapter.notifyDataSetChanged();
            }
            if (BluetoothAdapter.ACTION_DISCOVERY_FINISHED.equals(action)) {
                Toast.makeText(context, "Discovery has finished", Toast.LENGTH_LONG).show();
                findViewById(R.id.btn_search_device).setEnabled(true);
            }
        }
    };

    @Override
    public void onClick(View v) {
        switch(v.getId()) {
            case R.id.btn_ble:
                if (!getPackageManager().hasSystemFeature(PackageManager.FEATURE_BLUETOOTH_LE)) {
                    Toast.makeText(CacheManager.CacheContext, "BLE is not supported", Toast.LENGTH_SHORT).show();
                    finish();
                } else {
                    final BluetoothManager bluetoothManager = (BluetoothManager) getSystemService(Context.BLUETOOTH_SERVICE);
                    mBluetoothAdapter = bluetoothManager.getAdapter();

                    if (mBluetoothAdapter == null) {
                        Toast.makeText(CacheManager.CacheContext, "Bluetooth is not supported", Toast.LENGTH_SHORT).show();
                        finish();
                    } else {
                        CacheManager.TempBluetoothType = "LOW ENERGY";
                        _layoutBluetoothType.setVisibility(View.GONE);
                        _layoutBluetoothSearch.setVisibility(View.VISIBLE);
                    }
                }
                break;
            case R.id.btn_classic:
                final BluetoothManager bluetoothManager = (BluetoothManager) getSystemService(Context.BLUETOOTH_SERVICE);
                mBluetoothAdapter = bluetoothManager.getAdapter();

                if (mBluetoothAdapter == null) {
                    Toast.makeText(CacheManager.CacheContext, "Bluetooth is not supported", Toast.LENGTH_SHORT).show();
                    finish();
                } else {
                    CacheManager.TempBluetoothType = "CLASSIC";
                    _layoutBluetoothType.setVisibility(View.GONE);
                    _layoutBluetoothSearch.setVisibility(View.VISIBLE);

                    // Register for broadcasts when a device is discovered
                    IntentFilter filter = new IntentFilter(BluetoothDevice.ACTION_FOUND);
                    this.registerReceiver(mReceiver, filter);
                    filter = new IntentFilter(BluetoothAdapter.ACTION_DISCOVERY_FINISHED);
                    this.registerReceiver(mReceiver, filter);
                }
                break;
            case R.id.btn_search_device:
                try {
                    if (!mBluetoothAdapter.isEnabled()) {
                        mBluetoothAdapter.enable();
                        Thread.sleep(2000);
                    }
                    if (CacheManager.TempBluetoothType.equalsIgnoreCase("LOW ENERGY")) {
                        scanLeDevice(true);
                    }
                } catch (Exception ex) {
                }
                if (CacheManager.TempBluetoothType.equalsIgnoreCase("CLASSIC")) {
                    mBluetoothAdapter.startDiscovery();
                }
                findViewById(R.id.btn_search_device).setEnabled(false);
                break;
            default:
                break;
        }
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_bluetooth);

        _layoutBluetoothType = (LinearLayout)findViewById(R.id.layout_bluetooth_type);
        _layoutBluetoothSearch = (LinearLayout)findViewById(R.id.layout_bluetooth_search);
        _layoutBluetoothType.setVisibility(View.VISIBLE);

        mHandler = new Handler();

        mContext = getApplicationContext();
        mArrayAdapter = new ArrayAdapter<String>(this, android.R.layout.simple_list_item_1, 0);

        btnSearch = (Button)findViewById(R.id.btn_search_device);

        lstNewDevices = (ListView) findViewById(R.id.list_new_devices);
        lstNewDevices.setAdapter(mArrayAdapter);

        btnSearch.setOnClickListener(this);

        lstNewDevices.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
                if (CacheManager.TempBluetoothType.equalsIgnoreCase("CLASSIC")) {
                    mBluetoothAdapter.cancelDiscovery();
                }
                if (CacheManager.TempBluetoothType.equalsIgnoreCase("LOW ENERGY")) {
                    scanLeDevice(false);
                }

                String itemText = parent.getAdapter().getItem(position).toString();
                String macAddress = itemText.substring(itemText.lastIndexOf("\n") + 1);

                try {
                    SettingsHelper.saveBluetoothType(mContext, CacheManager.TempBluetoothType);
                    CacheManager.BluetoothType = CacheManager.TempBluetoothType;
                    SettingsHelper.saveBluetoothAddress(mContext, macAddress);
                    CacheManager.BluetoothMACAddress = macAddress;
                    Intent i = new Intent(BluetoothActivity.this, MainActivity.class);
                    startActivity(i);
                    finish();
                } catch (Exception ex) {
                    Log.e("bluetooth", ex.getMessage());
                }
            }
        });

        Button btnBle = (Button)findViewById(R.id.btn_ble);
        btnBle.setOnClickListener(this);

        Button btnClassic = (Button)findViewById(R.id.btn_classic);
        btnClassic.setOnClickListener(this);
    }

    @Override
    protected void onDestroy()
    {
        try {
            if(CacheManager.TempBluetoothType.equalsIgnoreCase("LOW ENERGY")) {
                scanLeDevice(false);
            }
            if(CacheManager.TempBluetoothType.equalsIgnoreCase("CLASSIC")) {
                this.unregisterReceiver(mReceiver);
            }
        } catch (Exception ex) {

        }
        super.onDestroy();
    }

    private void scanLeDevice(final boolean enable) {
        if(enable) {
            mHandler.postDelayed(new Runnable() {
                @Override
                public void run() {
                    mBluetoothAdapter.stopLeScan(mLeScanCallback);
                    Toast.makeText(mContext, "Discovery has finished", Toast.LENGTH_LONG).show();
                    findViewById(R.id.btn_search_device).setEnabled(true);
                }
            }, SCAN_PERIOD);
            mBluetoothAdapter.startLeScan(mLeScanCallback);
        } else {
            mBluetoothAdapter.stopLeScan(mLeScanCallback);
            findViewById(R.id.btn_search_device).setEnabled(true);
        }
    }

    private BluetoothAdapter.LeScanCallback mLeScanCallback = new BluetoothAdapter.LeScanCallback() {
        @Override
        public void onLeScan(final BluetoothDevice device, final int rssi, byte[] scanRecord) {
            runOnUiThread((new Runnable() {
                @Override
                public void run() {
                    String bluetooth = "Name : " + device.getName() + "\nRSSI : " + String.valueOf(rssi) + "\n" + device.getAddress();

                    for(int i=0 ; i < mArrayAdapter.getCount() ; i++){
                        String obj = mArrayAdapter.getItem(i);
                        if(obj.contains(device.getName()) && obj.contains(device.getAddress())) {
                            mArrayAdapter.remove(obj);
                        }
                    }

                    mArrayAdapter.add(bluetooth);
                    mArrayAdapter.notifyDataSetChanged();
                }
            }));
        }
    };
}
