package com.AssetManagementAndroidTestApp;

import android.app.Activity;
import android.widget.TextView;
import android.os.Bundle;
import android.content.res.AssetManager;

public class AssetManagementAndroidTestApp extends Activity
{
    /** Called when the activity is first created. */
    @Override
    public void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);

        /* Create a TextView and set its text to "Hello world" */
        TextView  tv = new TextView(this);

        try{
            System.loadLibrary("AssetManagement");
            assetManager = getResources().getAssets();
            tv.setText("Test result: " + Integer.toString(runTest(assetManager)));
        }
        catch(Exception e){
            tv.setText(e.getMessage());
        }

        setContentView(tv);
    }

    public static native int runTest(AssetManager assetManager);

    private AssetManager assetManager;
}