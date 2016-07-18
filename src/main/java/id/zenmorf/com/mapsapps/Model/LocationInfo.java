package id.zenmorf.com.mapsapps.Model;

/**
 * Created by hp on 22/4/2016.
 */
public class LocationInfo {

    private int _id;
    private String _action;
    private String _userID;
    private double _latitude;
    private double _longitude;
    private double _accuracy;
    private int _currentTimeMillis;

    public LocationInfo() {

    }

    public LocationInfo(int id, String action, String userID, double latitude, double longitude, double accuracy, int currentTimeMillis) {
        this._id = id;
        this._action = action;
        this._userID = userID;
        this._latitude = latitude;
        this._longitude = longitude;
        this._accuracy = accuracy;
        this._currentTimeMillis = currentTimeMillis;
    }

    public LocationInfo(String action, String userID, double latitude, double longitude, double accuracy, int currentTimeMillis) {
        this._action = action;
        this._userID = userID;
        this._latitude = latitude;
        this._longitude = longitude;
        this._accuracy = accuracy;
        this._currentTimeMillis = currentTimeMillis;
    }

    public void setID(int id) {
        this._id = id;
    }

    public int getID() {
        return this._id;
    }

    public void setAction(String action) {
        this._action = action;
    }

    public String getAction() {
        return this._action;
    }

    public void setUserID(String userID) {
        this._userID = userID;
    }

    public String getUserID() {
        return this._userID;
    }

    public void setLatitude(double latitude) {
        this._latitude = latitude;
    }

    public double getLatitude() {
        return this._latitude;
    }

    public void setLongitude(double longitude) {
        this._longitude = longitude;
    }

    public double getLongitude() {
        return this._longitude;
    }

    public void setAccuracy(double accuracy) {
        this._accuracy = accuracy;
    }

    public double getAccuracy() {
        return this._accuracy;
    }

    public void setCurrentTimeMillis(int currentTimeMillis) {
        this._currentTimeMillis = currentTimeMillis;
    }

    public int getCurrentTimeMillis() {
        return this._currentTimeMillis;
    }
}
