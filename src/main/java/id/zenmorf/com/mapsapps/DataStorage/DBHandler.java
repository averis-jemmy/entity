package id.zenmorf.com.mapsapps.DataStorage;

import android.content.ContentValues;
import android.content.Context;
import android.database.Cursor;
import android.database.sqlite.SQLiteDatabase;
import android.database.sqlite.SQLiteOpenHelper;

import java.util.ArrayList;
import java.util.List;

import id.zenmorf.com.mapsapps.Model.LocationInfo;

/**
 * Created by hp on 22/4/2016.
 */
public class DBHandler extends SQLiteOpenHelper {
    private static final int DATABASE_VERSION = 1;
    private static final String DATABASE_NAME = "locationDB.db";
    private static final String TABLE_LOCATION = "location";

    public static final String COLUMN_ID = "id";
    public static final String COLUMN_ACTION = "action";
    public static final String COLUMN_USER = "user";
    public static final String COLUMN_LATITUDE = "latitude";
    public static final String COLUMN_LONGITUDE = "longitude";
    public static final String COLUMN_ACCURACY = "accuracy";
    public static final String COLUMN_CURRENT_TIME_MILLIS = "currentTimeMillis";

    public DBHandler(Context context, String name,
                       SQLiteDatabase.CursorFactory factory, int version) {
        super(context, DATABASE_NAME, factory, DATABASE_VERSION);
    }

    @Override
    public void onCreate(SQLiteDatabase db) {
        String CREATE_LOCATION_TABLE = "CREATE TABLE " +
                TABLE_LOCATION + "("
                + COLUMN_ID + " INTEGER PRIMARY KEY AUTOINCREMENT," + COLUMN_ACTION + " TEXT," +
                COLUMN_USER + " TEXT," + COLUMN_LATITUDE + " TEXT," + COLUMN_LONGITUDE + " REAL," +
                COLUMN_ACCURACY + " REAL," + COLUMN_CURRENT_TIME_MILLIS + " INTEGER" + ")";
        db.execSQL(CREATE_LOCATION_TABLE);
    }

    @Override
    public void onUpgrade(SQLiteDatabase db, int oldVersion,
                          int newVersion) {
        db.execSQL("DROP TABLE IF EXISTS " + TABLE_LOCATION);
        onCreate(db);
    }

    public void addLocation(LocationInfo location) {
        try {
            ContentValues values = new ContentValues();
            values.put(COLUMN_ACTION, location.getAction());
            values.put(COLUMN_USER, location.getUserID());
            values.put(COLUMN_LATITUDE, location.getLatitude());
            values.put(COLUMN_LONGITUDE, location.getLongitude());
            values.put(COLUMN_ACCURACY, location.getAccuracy());
            values.put(COLUMN_CURRENT_TIME_MILLIS, location.getCurrentTimeMillis());

            SQLiteDatabase db = this.getWritableDatabase();

            db.insert(TABLE_LOCATION, null, values);
            db.close();
        } catch (Exception ex) {
            ex.printStackTrace();
        }
    }

    public List<LocationInfo> findLocation() {
        String query = "Select * FROM " + TABLE_LOCATION + " ORDER BY id";

        SQLiteDatabase db = this.getWritableDatabase();

        Cursor cursor = db.rawQuery(query, null);

        List<LocationInfo> list = new ArrayList<LocationInfo>();

        try {
            if( (cursor != null) && cursor.moveToFirst() )
            {
                cursor.moveToFirst();
                do
                {
                    LocationInfo location = new LocationInfo();
                    location.setID(Integer.parseInt(cursor.getString(0)));
                    location.setAction(cursor.getString(1));
                    location.setUserID(cursor.getString(2));
                    location.setLatitude(Double.parseDouble(cursor.getString(3)));
                    location.setLongitude(Double.parseDouble(cursor.getString(4)));
                    location.setAccuracy(Double.parseDouble(cursor.getString(5)));
                    location.setCurrentTimeMillis(Integer.parseInt(cursor.getString(6)));
                    list.add(location);
                } while (cursor.moveToNext());
                cursor.close();
            }
            db.close();
        } catch (Exception ex) {
            ex.printStackTrace();
        }

        return list;
    }

    public boolean deleteLocation(int id) {

        boolean result = false;

        try {
            String query = "Select * FROM " + TABLE_LOCATION + " WHERE " + COLUMN_ID + " =  \"" + id + "\"";

            SQLiteDatabase db = this.getWritableDatabase();

            Cursor cursor = db.rawQuery(query, null);

            LocationInfo location = new LocationInfo();

            if (cursor.moveToFirst()) {
                location.setID(Integer.parseInt(cursor.getString(0)));
                db.delete(TABLE_LOCATION, COLUMN_ID + " = ?",
                        new String[] { String.valueOf(location.getID()) });
                cursor.close();
                result = true;
            }
            db.close();
        } catch (Exception ex) {
            ex.printStackTrace();
        }
        return result;
    }
}
