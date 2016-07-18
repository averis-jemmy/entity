package id.zenmorf.com.mapsapps.BusinessLogic;

import android.app.NotificationManager;
import android.content.Context;
import android.location.Location;
import android.provider.Settings;

import id.zenmorf.com.mapsapps.DataStorage.SettingsHelper;

/**
 * Created by hp on 2/3/2016.
 */
public class CacheManager {
    public static String BluetoothMACAddress = "FF:FF:00:00:68:F8";
    public static Context CacheContext = null;
    public static String UserID = "";
    public static String TokenID = "";
    public static String DeviceID = "";
    public static String CampaignID = "";
    public static String DeviceSerialNumber = "";
    public static String BluetoothType = "";
    public static String TempBluetoothType = "";

    public static boolean IsStopping = true;

    public static Location LastLocation;
    public static String LastDistance;

    public static NotificationManager NotificationManagerInstance;

    public static void InitDevice(){
        CampaignID = SettingsHelper.getCampaignID(CacheContext);
        //SettingsHelper.saveBluetoothAddress(CacheContext, BluetoothMACAddress);
        //SettingsHelper.saveBluetoothType(CacheContext, "LOW ENERGY");
        //CampaignID = "0";
        UserID = SettingsHelper.getLoginID(CacheContext);
        BluetoothType = SettingsHelper.getBluetoothType(CacheContext);
        DeviceID = SettingsHelper.getDeviceID(CacheContext);
        TokenID = SettingsHelper.getTokenID(CacheContext);
        if(DeviceID == "") {
            DeviceID = Settings.Secure.getString(CacheContext.getContentResolver(),
                    Settings.Secure.ANDROID_ID);
            SettingsHelper.saveDeviceID(CacheContext, DeviceID);
        }
        DeviceSerialNumber = SettingsHelper.getDeviceSerialNumber(CacheContext);
        BluetoothMACAddress = SettingsHelper.getBluetoothAddress(CacheContext);
    }
}
