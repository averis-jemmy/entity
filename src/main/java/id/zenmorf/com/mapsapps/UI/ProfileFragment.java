package id.zenmorf.com.mapsapps.UI;

import android.app.ProgressDialog;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.view.WindowManager;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import id.zenmorf.com.mapsapps.Dialog.BluetoothAccessDialog;
import id.zenmorf.com.mapsapps.R;

/**
 * Created by hp on 7/6/2016.
 */
public class ProfileFragment extends Fragment {
    Button _saveButton = null;
    EditText _nameText = null;
    EditText _lastNameText = null;
    EditText _passwordText = null;
    EditText _plateText = null;
    EditText _bankNameText = null;
    EditText _accountNumberText = null;
    //TextView _nameTextView = null;

    public static ProfileFragment newInstance() {
        ProfileFragment fragment = new ProfileFragment();
        return fragment;
    }

    public ProfileFragment() {

    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
        View rootView = inflater.inflate(R.layout.fragment_profile, container, false);

        _nameText = (EditText)rootView.findViewById(R.id.input_name_profile);
        _lastNameText = (EditText)rootView.findViewById(R.id.input_last_name_profile);
        _passwordText = (EditText)rootView.findViewById(R.id.input_password_profile);
        _plateText = (EditText)rootView.findViewById(R.id.input_plate_profile);
        _bankNameText = (EditText)rootView.findViewById(R.id.input_bank_name);
        _accountNumberText = (EditText)rootView.findViewById(R.id.input_account_number);
        //_nameTextView = (TextView)rootView.findViewById(R.id.tv_name_profile);

        _saveButton = (Button) rootView.findViewById(R.id.btn_save_profile);
        _saveButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                saveProfile();
            }
        });

        return  rootView;
    }

    public void onSaveSuccess() {
        //_nameTextView.setText(_nameText.getText().toString());
        _saveButton.setEnabled(true);
        Toast.makeText(getActivity(), "Saved successfully", Toast.LENGTH_LONG).show();
    }

    public void onSaveFailed() {
        Toast.makeText(getActivity(), "Save failed", Toast.LENGTH_LONG).show();
        _saveButton.setEnabled(true);
    }

    public boolean validate() {
        boolean valid = true;

        String name = _nameText.getText().toString();
        String password = _passwordText.getText().toString();
        String plate = _plateText.getText().toString();

        if (name.isEmpty()) {
            _nameText.setError("enter your name");
            valid = false;
        } else {
            _nameText.setError(null);
        }

        if (password.isEmpty() || password.length() < 4 || password.length() > 10) {
            _passwordText.setError("between 4 and 10 alphanumeric characters");
            valid = false;
        } else {
            _passwordText.setError(null);
        }

        if (plate.isEmpty()) {
            _plateText.setError("enter your vehicle plate number");
            valid = false;
        } else {
            _plateText.setError(null);
        }

        return valid;
    }

    public void saveProfile() {
        if (!validate()) {
            return;
        }

        final ProgressDialog progressDialog = new ProgressDialog(getActivity(),
                R.style.AppTheme_Dark_Dialog);
        progressDialog.setIndeterminate(true);
        progressDialog.setMessage("Saving...");
        progressDialog.show();

        new android.os.Handler().postDelayed(
                new Runnable() {
                    public void run() {
                        // On complete call either onLoginSuccess or onLoginFailed
                        onSaveSuccess();
                        // onLoginFailed();
                        progressDialog.dismiss();
                    }
                }, 3000);
    }
}
