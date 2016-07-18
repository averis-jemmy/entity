package id.zenmorf.com.mapsapps.Dialog;

import android.app.Activity;
import android.app.Dialog;
import android.os.Bundle;
import android.view.View;
import android.view.Window;
import android.widget.Button;
import android.widget.TextView;

import id.zenmorf.com.mapsapps.R;

/**
 * Created by hp on 31/5/2016.
 */
public class LoginFailedDialog extends Dialog implements View.OnClickListener {

    public Activity mActivity;
    public Dialog mDialog;
    public Button btnYes;
    public TextView tvForgotPassword;

    public LoginFailedDialog(Activity activity) {
        super(activity);
        this.mActivity = activity;
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        requestWindowFeature(Window.FEATURE_NO_TITLE);
        setContentView(R.layout.login_failed_dialog);
        btnYes = (Button)findViewById(R.id.btn_try_again);
        tvForgotPassword = (TextView)findViewById(R.id.link_forgot_password);
        btnYes.setOnClickListener(this);
        tvForgotPassword.setOnClickListener(this);
    }

    @Override
    public void onClick(View v) {
        switch(v.getId()) {
            case R.id.btn_try_again:
            case R.id.link_forgot_password:
                dismiss();
                break;
            default:
                break;
        }
        dismiss();
    }
}
