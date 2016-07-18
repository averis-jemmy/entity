package id.zenmorf.com.mapsapps.Dialog;

import android.app.Activity;
import android.app.Dialog;
import android.os.Bundle;
import android.view.View;
import android.view.Window;
import android.widget.Button;

import id.zenmorf.com.mapsapps.UI.MainActivity;
import id.zenmorf.com.mapsapps.R;

/**
 * Created by hp on 2/6/2016.
 */
public class LogoutDialog extends Dialog implements View.OnClickListener {

    public Activity mActivity;
    public Dialog mDialog;
    public Button btnYes, btnNo;

    public LogoutDialog(Activity activity) {
        super(activity);
        this.mActivity = activity;
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        requestWindowFeature(Window.FEATURE_NO_TITLE);
        setContentView(R.layout.logout_dialog);
        btnYes = (Button)findViewById(R.id.btn_logout_yes);
        btnNo = (Button)findViewById(R.id.btn_logout_no);
        btnYes.setOnClickListener(this);
        btnNo.setOnClickListener(this);
    }

    @Override
    public void onClick(View v) {
        switch(v.getId()) {
            case R.id.btn_logout_yes:
                dismiss();
                ((MainActivity)mActivity).onLogoutPressed();
                break;
            case R.id.btn_logout_no:
                dismiss();
                ((MainActivity)mActivity).onLogoutFailed();
                break;
            default:
                break;
        }
        dismiss();
    }
}