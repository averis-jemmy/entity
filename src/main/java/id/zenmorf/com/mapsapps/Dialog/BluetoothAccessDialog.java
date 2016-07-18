package id.zenmorf.com.mapsapps.Dialog;

import android.app.Activity;
import android.app.Dialog;
import android.os.Bundle;
import android.view.View;
import android.view.Window;
import android.widget.Button;
import android.widget.EditText;

import id.zenmorf.com.mapsapps.UI.MainActivity;
import id.zenmorf.com.mapsapps.R;

/**
 * Created by hp on 4/6/2016.
 */
public class BluetoothAccessDialog extends Dialog implements View.OnClickListener {

    public Activity mActivity;
    public Dialog mDialog;
    public EditText etPassword;
    public Button btnContinue;

    public BluetoothAccessDialog(Activity activity) {
        super(activity);
        this.mActivity = activity;
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        requestWindowFeature(Window.FEATURE_NO_TITLE);
        setContentView(R.layout.bluetooth_access_dialog);
        btnContinue = (Button)findViewById(R.id.btn_access_continue);
        etPassword = (EditText)findViewById(R.id.txt_access_password);
        btnContinue.setOnClickListener(this);
    }

    @Override
    public void onClick(View v) {
        switch(v.getId()) {
            case R.id.btn_access_continue:
                dismiss();
                if(etPassword.getText().toString().equals("P@$$w0rd"))
                    ((MainActivity)mActivity).onBluetoothAccessPressed();
                else
                    ((MainActivity)mActivity).onBluetoothAccessFailed();
                break;
            default:
                break;
        }
        dismiss();
    }
}
