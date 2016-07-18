package id.zenmorf.com.mapsapps.DataStorage;

import android.content.Context;
import android.content.SharedPreferences;

/**
 * Created by hp on 3/3/2016.
 */
public class SettingsHelper {

    private static final String PREFS_NAME = "DeviceData";
    private static final String bluetoothAddressKey = "BLUETOOTH_ADDRESS";
    private static final String bluetoothTypeKey = "BLUETOOTH_TYPE";
    private static final String deviceID = "DEVICE_ID";
    private static final String deviceSerialNumber = "DEVICE_SERIAL_NUMBER";
    private static final String loginID = "USER_ID";
    private static final String campaignID = "CAMPAIGN_ID";
    private static final String tokenID = "TOKEN_ID";

    public static String getDeviceID(Context context) {
        SharedPreferences settings = context.getSharedPreferences(PREFS_NAME, 0);
        return settings.getString(deviceID, "");
    }

    public static String getDeviceSerialNumber(Context context) {
        SharedPreferences settings = context.getSharedPreferences(PREFS_NAME, 0);
        return settings.getString(deviceSerialNumber, "");
    }

    public static String getBluetoothAddress(Context context) {
        SharedPreferences settings = context.getSharedPreferences(PREFS_NAME, 0);
        return settings.getString(bluetoothAddressKey, "");
    }

    public static String getBluetoothType(Context context) {
        SharedPreferences settings = context.getSharedPreferences(PREFS_NAME, 0);
        return settings.getString(bluetoothTypeKey, "");
    }

    public static String getLoginID(Context context) {
        SharedPreferences settings = context.getSharedPreferences(PREFS_NAME, 0);
        return settings.getString(loginID, "");
    }

    public static String getCampaignID(Context context) {
        SharedPreferences settings = context.getSharedPreferences(PREFS_NAME, 0);
        return settings.getString(campaignID, "");
    }

    public static String getTokenID(Context context) {
        SharedPreferences settings = context.getSharedPreferences(PREFS_NAME, 0);
        return settings.getString(tokenID, "");
    }

    public static void saveDeviceID(Context context, String id) {
        SharedPreferences settings = context.getSharedPreferences(PREFS_NAME, 0);
        SharedPreferences.Editor editor = settings.edit();
        editor.putString(deviceID, id);
        editor.commit();
    }

    public static void saveDeviceSerialNumber(Context context, String serialNumber) {
        SharedPreferences settings = context.getSharedPreferences(PREFS_NAME, 0);
        SharedPreferences.Editor editor = settings.edit();
        editor.putString(deviceSerialNumber, serialNumber);
        editor.commit();
    }

    public static void saveBluetoothAddress(Context context, String address) {
        SharedPreferences settings = context.getSharedPreferences(PREFS_NAME, 0);
        SharedPreferences.Editor editor = settings.edit();
        editor.putString(bluetoothAddressKey, address);
        editor.commit();
    }

    public static void saveBluetoothType(Context context, String type) {
        SharedPreferences settings = context.getSharedPreferences(PREFS_NAME, 0);
        SharedPreferences.Editor editor = settings.edit();
        editor.putString(bluetoothTypeKey, type);
        editor.commit();
    }

    public static void saveLoginID(Context context, String userID) {
        SharedPreferences settings = context.getSharedPreferences(PREFS_NAME, 0);
        SharedPreferences.Editor editor = settings.edit();
        editor.putString(loginID, userID);
        editor.commit();
    }

    public static void saveCampaignID(Context context, String campaign) {
        SharedPreferences settings = context.getSharedPreferences(PREFS_NAME, 0);
        SharedPreferences.Editor editor = settings.edit();
        editor.putString(campaignID, campaign);
        editor.commit();
    }

    public static void saveTokenID(Context context, String token) {
        SharedPreferences settings = context.getSharedPreferences(PREFS_NAME, 0);
        SharedPreferences.Editor editor = settings.edit();
        editor.putString(tokenID, token);
        editor.commit();
    }
}