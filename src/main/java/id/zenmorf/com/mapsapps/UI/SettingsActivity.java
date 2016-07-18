package id.zenmorf.com.mapsapps.UI;

import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.os.Bundle;
import android.support.v4.app.NavUtils;
import android.support.v7.app.AlertDialog;
import android.support.v7.app.AppCompatActivity;
import android.view.MenuItem;
import android.view.View;
import android.widget.TextView;

import id.zenmorf.com.mapsapps.BusinessLogic.CacheManager;
import id.zenmorf.com.mapsapps.BusinessLogic.MyApplication;
import id.zenmorf.com.mapsapps.R;

/**
 * Created by hp on 30/3/2016.
 */
public class SettingsActivity extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_settings);

        android.support.v7.app.ActionBar actionBar = getSupportActionBar();
        actionBar.setHomeButtonEnabled(true);
        actionBar.setDisplayHomeAsUpEnabled(true);

        TextView tvProfile = (TextView)findViewById(R.id.link_profile);
        tvProfile.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                //Intent i = new Intent(SettingsActivity.this, ProfileActivity.class);
                //startActivity(i);
            }
        });

        TextView tvChangePassword = (TextView)findViewById(R.id.link_change_password);
        tvChangePassword.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                Intent i = new Intent(SettingsActivity.this, ChangePasswordActivity.class);
                startActivity(i);
            }
        });

        TextView tvLogout = (TextView)findViewById(R.id.link_logout);
        tvLogout.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                AlertMessage(SettingsActivity.this, "Log Out", "Are you sure?", 2);
            }
        });
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
            finish();
        } catch (Exception ex) {
        }

        super.onPause();
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        switch (item.getItemId()) {
            // Respond to the action bar's Up/Home button
            case android.R.id.home:
                NavUtils.navigateUpFromSameTask(this);
                return true;
        }
        return super.onOptionsItemSelected(item);
    }

    public void AlertMessage(final Context context, String title,String message,int type)
    {
        final AlertDialog.Builder builder = new AlertDialog.Builder(context, R.style.AppTheme_Dark_Dialog);
        builder.setTitle(title);
        builder.setMessage(message);
        if( type == 0)
        {
            builder.setPositiveButton("OK", null);
        }
        if(type == 1)
        {
            builder.setPositiveButton("OK", null);
            builder.setNegativeButton("Cancel",null);
        }
        if( type == 2)
        {
            builder.setPositiveButton("Yes",new DialogInterface.OnClickListener() {
                @Override
                public void onClick(DialogInterface dialog, int id) {
                    setResult(RESULT_OK, null);
                    finish();
                }
            });

            builder.setNegativeButton("No", null);
        }
        if(type == 3)
        {
            builder.setPositiveButton("OK", new DialogInterface.OnClickListener() {
                @Override
                public void onClick(DialogInterface dialog, int id) {

                }
            });
        }
        builder.show();
    }
}