package id.zenmorf.com.mapsapps.UI;

import android.app.ProgressDialog;
import android.content.Intent;
import android.os.Bundle;
import android.support.v7.app.AppCompatActivity;
import android.text.Editable;
import android.text.TextWatcher;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import com.android.volley.Request;
import com.android.volley.Response;
import com.android.volley.VolleyError;
import com.android.volley.toolbox.JsonObjectRequest;

import org.json.JSONException;
import org.json.JSONObject;

import java.util.HashMap;
import java.util.Map;

import id.zenmorf.com.mapsapps.BusinessLogic.CacheManager;
import id.zenmorf.com.mapsapps.Dialog.LoginFailedDialog;
import id.zenmorf.com.mapsapps.R;
import id.zenmorf.com.mapsapps.DataStorage.SettingsHelper;
import id.zenmorf.com.mapsapps.BusinessLogic.VolleySingleton;

/**
 * Created by hp on 3/3/2016.
 */
public class LoginActivity extends AppCompatActivity {
    private static final String TAG = "LoginActivity";
    private static final int REQUEST_SIGNUP = 0;
    Button _loginButton = null;
    TextView _signUpLink = null;
    EditText _emailText = null;
    EditText _passwordText = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_login);

        _emailText = (EditText)findViewById(R.id.input_email);
        _passwordText = (EditText)findViewById(R.id.input_password);

        _loginButton = (Button)findViewById(R.id.btn_login);
        _loginButton.setOnClickListener(new View.OnClickListener() {

            @Override
            public void onClick(View v) {
                login();
            }
        });

        _signUpLink = (TextView)findViewById(R.id.link_signup);
        _signUpLink.setOnClickListener(new View.OnClickListener() {

            @Override
            public void onClick(View v) {
                // Start the Signup activity
                Intent intent = new Intent(getApplicationContext(), SignupActivity.class);
                startActivityForResult(intent, REQUEST_SIGNUP);
            }
        });

        _emailText.addTextChangedListener(new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence s, int start, int count, int after) {

            }

            @Override
            public void onTextChanged(CharSequence s, int start, int before, int count) {

            }

            @Override
            public void afterTextChanged(Editable s) {
                if(!_emailText.getText().equals("") && !_passwordText.getText().equals("")) {
                    _signUpLink.setVisibility(View.INVISIBLE);
                } else {
                    _signUpLink.setVisibility(View.VISIBLE);
                }
            }
        });

        _passwordText.addTextChangedListener(new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence s, int start, int count, int after) {

            }

            @Override
            public void onTextChanged(CharSequence s, int start, int before, int count) {

            }

            @Override
            public void afterTextChanged(Editable s) {
                if(!_emailText.getText().equals("") && !_passwordText.getText().equals("")) {
                    _signUpLink.setVisibility(View.INVISIBLE);
                } else {
                    _signUpLink.setVisibility(View.VISIBLE);
                }
            }
        });
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        if (requestCode == REQUEST_SIGNUP) {
            if (resultCode == RESULT_OK) {
                // TODO: Implement successful signup logic here
                // By default we just finish the Activity and log them in automatically

                //this.finish();
            }
        }
    }

    @Override
    public void onBackPressed() {
        // Disable going back to the MainActivity
        moveTaskToBack(true);
    }

    public void login() {
        Log.d(TAG, "Login");

        if (!validate()) {
            return;
        }

        _loginButton.setEnabled(false);

        final ProgressDialog progressDialog = new ProgressDialog(LoginActivity.this,
                R.style.AppTheme_Dark_Dialog);
        progressDialog.setIndeterminate(true);
        progressDialog.setMessage("Authenticating...");
        progressDialog.show();

        Map<String, String>  params = new HashMap<>();
        params.put("email", _emailText.getText().toString());
        params.put("password", _passwordText.getText().toString());

        String url = "http://mobweb.cloudapp.net:81/api/v1/login";
        JsonObjectRequest postRequest = new JsonObjectRequest(Request.Method.POST, url, new JSONObject(params),
                new Response.Listener<JSONObject>() {
                    @Override
                    public void onResponse(JSONObject response) {
                        try {
                            JSONObject jsonResponse = response.getJSONObject("result");
                            String status = jsonResponse.getString("status");

                            if(status.equals("LOGIN_SUCCESS")) {
                                String sid = jsonResponse.getString("sid");
                                CacheManager.UserID = _emailText.getText().toString();
                                CacheManager.TokenID = sid;
                                SettingsHelper.saveLoginID(CacheManager.CacheContext, CacheManager.UserID);
                                SettingsHelper.saveTokenID(CacheManager.CacheContext, sid);
                                onLoginSuccess();
                            }
                            else {
                                onLoginFailed();
                            }
                        } catch (JSONException e) {
                            e.printStackTrace();
                            onLoginFailed();
                        }
                        progressDialog.dismiss();
                    }
                },
                new Response.ErrorListener() {
                    @Override
                    public void onErrorResponse(VolleyError error) {
                        error.printStackTrace();
                        progressDialog.dismiss();
                        onLoginFailed();
                    }
                });

        VolleySingleton.getInstance(this).addToRequestQueue(postRequest);
    }

    public void onLoginSuccess() {
        _loginButton.setEnabled(true);
        Toast.makeText(this, "Login success", Toast.LENGTH_SHORT).show();

        Intent intent = new Intent(this, MainActivity.class);
        startActivity(intent);
        finish();
    }

    public void onLoginFailed() {
        _loginButton.setEnabled(true);
        LoginFailedDialog cdd = new LoginFailedDialog(LoginActivity.this);
        cdd.show();
    }

    public boolean validate() {
        boolean valid = true;

        String email = _emailText.getText().toString();
        String password = _passwordText.getText().toString();

        if (email.isEmpty() || !android.util.Patterns.EMAIL_ADDRESS.matcher(email).matches()
                || (!email.toUpperCase().contains("VINO") && !email.toUpperCase().contains("JEMMY")
                && !email.toUpperCase().contains("BUDI") && !email.toUpperCase().contains("TIM"))) {
            _emailText.setError("enter a valid email address");
            valid = false;
        } else {
            _emailText.setError(null);
        }

        if (password.isEmpty() || password.length() < 4 || password.length() > 10) {
            _passwordText.setError("between 4 and 10 alphanumeric characters");
            valid = false;
        } else {
            _passwordText.setError(null);
        }

        return valid;
    }
}