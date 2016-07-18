package id.zenmorf.com.mapsapps.BusinessLogic;

import android.app.ActivityManager;
import android.app.Application;
import android.app.Notification;
import android.app.PendingIntent;
import android.content.Context;
import android.content.Intent;
import android.os.Build;

import java.util.List;
import java.util.Timer;
import java.util.TimerTask;

import id.zenmorf.com.mapsapps.R;

/**
 * Created by hp on 29/3/2016.
 */
public class MyApplication extends Application {
    private static final int NOTIFICATION_ID = 0;
    private Timer mActivityTransitionTimer;
    private TimerTask mActivityTransitionTimerTask;
    public boolean wasInBackground;
    private final long MAX_ACTIVITY_TRANSITION_TIME_MS = 3000;

    public void StopNotification() {
        try {
            CacheManager.NotificationManagerInstance.cancelAll();
        } catch (Exception ex) {

        }
    }

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

    public void startActivityTransitionTimer() {
        this.mActivityTransitionTimer = new Timer();
        this.mActivityTransitionTimerTask = new TimerTask() {
            public void run() {
                MyApplication.this.wasInBackground = true;

                Intent nIntent = getPreviousIntent();
                PendingIntent pi = PendingIntent.getActivity(CacheManager.CacheContext, 0, nIntent, 0);

                Notification notification = new Notification.Builder(CacheManager.CacheContext)
                        .setContentTitle("NEW NOTIFICATION")
                        .setContentText("HELLO!!! YOUR APP IS STILL RUNNING")
                        .setContentIntent(pi)
                        .setSmallIcon(R.drawable.icon)
                        .build();

                notification.flags = Notification.FLAG_NO_CLEAR;

                CacheManager.NotificationManagerInstance.notify(NOTIFICATION_ID, notification);
            }
        };

        this.mActivityTransitionTimer.schedule(mActivityTransitionTimerTask,
                MAX_ACTIVITY_TRANSITION_TIME_MS);
    }

    public void stopActivityTransitionTimer() {
        if (this.mActivityTransitionTimerTask != null) {
            this.mActivityTransitionTimerTask.cancel();
        }

        if (this.mActivityTransitionTimer != null) {
            this.mActivityTransitionTimer.cancel();
        }

        this.wasInBackground = false;
    }
}
