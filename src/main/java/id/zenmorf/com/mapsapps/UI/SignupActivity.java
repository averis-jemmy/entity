package id.zenmorf.com.mapsapps.UI;

import android.app.ProgressDialog;
import android.os.Bundle;
import android.support.v7.app.AppCompatActivity;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.LinearLayout;
import android.widget.Toast;

import com.android.volley.Request;
import com.android.volley.Response;
import com.android.volley.VolleyError;
import com.android.volley.toolbox.JsonObjectRequest;
import com.android.volley.toolbox.Volley;

import org.json.JSONException;
import org.json.JSONObject;

import java.util.HashMap;
import java.util.Map;

import id.zenmorf.com.mapsapps.BusinessLogic.CacheManager;
import id.zenmorf.com.mapsapps.R;

/**
 * Created by hp on 3/3/2016.
 */
public class SignupActivity extends AppCompatActivity {
    private static final String TAG = "SignUpActivity";
    Button _signUpButton = null;
    Button _backButton1 = null;
    Button _backButton2 = null;
    Button _backButton3 = null;
    Button _nextButton1 = null;
    Button _nextButton2 = null;
    EditText _nameText = null;
    EditText _lastNameText = null;
    EditText _phoneText = null;
    EditText _plateText = null;
    EditText _emailText = null;
    EditText _passwordText = null;
    EditText _confirmPasswordText = null;
    LinearLayout _signupLayout1 = null;
    LinearLayout _signupLayout2 = null;
    LinearLayout _signupLayout3 = null;

    String email, name, lastName, password, phone, plate;

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_signup);

        _signupLayout1 = (LinearLayout)findViewById(R.id.layout_signup1);
        _signupLayout2 = (LinearLayout)findViewById(R.id.layout_signup2);
        _signupLayout3 = (LinearLayout)findViewById(R.id.layout_signup3);

        _signupLayout1.setVisibility(View.VISIBLE);

        _nameText = (EditText)findViewById(R.id.input_name);
        _lastNameText = (EditText)findViewById(R.id.input_last_name);
        _emailText = (EditText)findViewById(R.id.input_email);
        _passwordText = (EditText)findViewById(R.id.input_password);
        _confirmPasswordText = (EditText)findViewById(R.id.input_confirm_password);
        _phoneText = (EditText)findViewById(R.id.input_phone);
        _plateText = (EditText)findViewById(R.id.input_plate);

        _signUpButton = (Button)findViewById(R.id.btn_signup);
        _signUpButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                signup();
            }
        });

        _backButton1 = (Button)findViewById(R.id.btn_back1);
        _backButton1.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                // Finish the registration screen and return to the Login activity
                finish();
            }
        });

        _backButton2 = (Button)findViewById(R.id.btn_back2);
        _backButton2.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                _signupLayout2.setVisibility(View.GONE);
                _signupLayout1.setVisibility(View.VISIBLE);
            }
        });

        _backButton3 = (Button)findViewById(R.id.btn_back3);
        _backButton3.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                _signupLayout3.setVisibility(View.GONE);
                _signupLayout2.setVisibility(View.VISIBLE);
            }
        });

        _nextButton1 = (Button)findViewById(R.id.btn_next1);
        _nextButton1.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if(validate1()) {
                    _signupLayout1.setVisibility(View.GONE);
                    _signupLayout2.setVisibility(View.VISIBLE);
                }
            }
        });

        _nextButton2 = (Button)findViewById(R.id.btn_next2);
        _nextButton2.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if(validate2()) {
                    _signupLayout2.setVisibility(View.GONE);
                    _signupLayout3.setVisibility(View.VISIBLE);
                }
            }
        });
    }

    public void signup() {
        Log.d(TAG, "Signup");

        if (!validate3()) {
            return;
        }

        _signUpButton.setEnabled(false);

        final ProgressDialog progressDialog = new ProgressDialog(SignupActivity.this,
                R.style.AppTheme_Dark_Dialog);
        progressDialog.setIndeterminate(true);
        progressDialog.setMessage("Creating Account...");
        progressDialog.show();

        Map<String, String> params = new HashMap<>();
        params.put("email", email);
        params.put("password", password);
        params.put("firstName", name);
        params.put("lastName", lastName);
        params.put("phoneNo", phone);
        params.put("vehicleNo", plate);

        String url = "http://mobweb.cloudapp.net:81/api/v1/signup";
        JsonObjectRequest postRequest = new JsonObjectRequest(Request.Method.POST, url, new JSONObject(params),
                new Response.Listener<JSONObject>() {
                    @Override
                    public void onResponse(JSONObject response) {
                        try {
                            JSONObject jsonResponse = response.getJSONObject("result");
                            String status = jsonResponse.getString("status");

                            if(status.equals("SIGNUP_SUCCESS")) {
                                onSignupSuccess();
                            }
                            else {
                                Toast.makeText(CacheManager.CacheContext, status, Toast.LENGTH_LONG).show();
                                onSignupFailed();
                            }
                        } catch (JSONException e) {
                            e.printStackTrace();
                            onSignupFailed();
                        }
                        progressDialog.dismiss();
                    }
                },
                new Response.ErrorListener() {
                    @Override
                    public void onErrorResponse(VolleyError error) {
                        error.printStackTrace();
                        progressDialog.dismiss();
                        onSignupFailed();
                    }
                });

        Volley.newRequestQueue(this).add(postRequest);
        //VolleySingleton.getInstance(this).addToRequestQueue(postRequest);
    }


    public void onSignupSuccess() {
        _signUpButton.setEnabled(true);
        setResult(RESULT_OK, null);
        Toast.makeText(this, "Sign up success", Toast.LENGTH_SHORT).show();
        finish();
    }

    public void onSignupFailed() {
        Toast.makeText(getBaseContext(), "Sign up failed", Toast.LENGTH_LONG).show();
        _signUpButton.setEnabled(true);
    }

    public boolean validate1() {
        boolean valid = true;

        name = _nameText.getText().toString();
        lastName = _lastNameText.getText().toString();
        email = _emailText.getText().toString();

        if (name.isEmpty()) {
            _nameText.setError("enter your name");
            valid = false;
        } else {
            _nameText.setError(null);
        }

        if (email.isEmpty() || !android.util.Patterns.EMAIL_ADDRESS.matcher(email).matches()) {
            _emailText.setError("enter a valid email address");
            valid = false;
        } else {
            _emailText.setError(null);
        }

        return valid;
    }

    public boolean validate2() {
        boolean valid = true;

        password = _passwordText.getText().toString();
        String confirmPassword = _confirmPasswordText.getText().toString();

        if (password.isEmpty() || password.length() < 4 || password.length() > 10) {
            _passwordText.setError("between 4 and 10 alphanumeric characters");
            valid = false;
        } else {
            _passwordText.setError(null);
        }

        if (confirmPassword.isEmpty() || !confirmPassword.equals(password)) {
            _confirmPasswordText.setError("confirm password is not the same as password");
            valid = false;
        } else {
            _confirmPasswordText.setError(null);
        }

        return valid;
    }

    public boolean validate3() {
        boolean valid = true;

        phone = _phoneText.getText().toString();
        plate = _plateText.getText().toString();

        if (phone.isEmpty()) {
            _phoneText.setError("enter your phone number");
            valid = false;
        } else {
            _phoneText.setError(null);
        }

        if (plate.isEmpty()) {
            _plateText.setError("enter your vehicle plate number");
            valid = false;
        } else {
            _plateText.setError(null);
        }

        return valid;
    }
}
