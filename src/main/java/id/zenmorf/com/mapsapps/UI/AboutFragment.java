package id.zenmorf.com.mapsapps.UI;

import android.os.Bundle;
import android.os.Handler;
import android.support.v4.app.Fragment;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;

import id.zenmorf.com.mapsapps.Dialog.BluetoothAccessDialog;
import id.zenmorf.com.mapsapps.R;

/**
 * Created by hp on 3/6/2016.
 */
public class AboutFragment extends Fragment {
    int iPressed = 0;
    Handler mHandler = new Handler();
    Runnable mRunnable;

    public static AboutFragment newInstance() {
        AboutFragment fragment = new AboutFragment();
        return fragment;
    }

    public AboutFragment() {

    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
        View rootView = inflater.inflate(R.layout.fragment_about, container, false);

        mRunnable = new Runnable() {
            @Override
            public void run() {
                iPressed = 0;
            }
        };

        TextView tvAbout = (TextView)rootView.findViewById(R.id.tv_about);
        tvAbout.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                try {
                    try {
                        mHandler.removeCallbacks(mRunnable);
                    } catch (Exception ex) {

                    }

                    if (iPressed > 13) {
                        BluetoothAccessDialog bad = new BluetoothAccessDialog(getActivity());
                        bad.show();
                    }

                    iPressed++;
                    mHandler.postDelayed(mRunnable, 2000);
                }catch (Exception ex) {

                }
            }
        });

        return  rootView;
    }
}
