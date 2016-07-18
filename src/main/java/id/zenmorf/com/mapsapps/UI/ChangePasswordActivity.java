package id.zenmorf.com.mapsapps.UI;

import android.content.Intent;
import android.os.Bundle;
import android.support.v7.app.AppCompatActivity;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;

import id.zenmorf.com.mapsapps.BusinessLogic.CacheManager;
import id.zenmorf.com.mapsapps.BusinessLogic.MyApplication;
import id.zenmorf.com.mapsapps.R;

/**
 * Created by hp on 30/3/2016.
 */
public class ChangePasswordActivity extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_change_password);

        android.support.v7.app.ActionBar actionBar = getSupportActionBar();
        actionBar.setHomeButtonEnabled(true);
        actionBar.setDisplayHomeAsUpEnabled(true);

        Button _changePasswordButton = (Button)findViewById(R.id.btn_change_password);
        _changePasswordButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                Intent i = new Intent(ChangePasswordActivity.this, SettingsActivity.class);
                startActivity(i);
            }
        });
    }

    @Override
    public void onBackPressed() {
        Intent i = new Intent(this, SettingsActivity.class);
        startActivity(i);
        return;
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
                Intent i = new Intent(this, SettingsActivity.class);
                startActivity(i);
                return true;
        }
        return super.onOptionsItemSelected(item);
    }
}