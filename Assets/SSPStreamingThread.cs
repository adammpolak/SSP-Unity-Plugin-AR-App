using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Runtime.InteropServices;
using System.Threading;
using System;

public class SSPStreamingThread : MonoBehaviour
{
    [DllImport("__Internal")]
    public static extern int ssp_server(
        [MarshalAs(UnmanagedType.LPStr)]string filename,
        [MarshalAs(UnmanagedType.LPStr)]string client_key = null,
        [MarshalAs(UnmanagedType.LPStr)]string environment_name = null,
        [MarshalAs(UnmanagedType.LPStr)]string sensor_name = null);
    
    [DllImport("__Internal")]
    public static extern void use_session(IntPtr session);

    [DllImport("__Internal")]
    public static extern void initialize(
        [MarshalAs(UnmanagedType.LPStr)]string filename,
        [MarshalAs(UnmanagedType.LPStr)]string client_key = "",
        [MarshalAs(UnmanagedType.LPStr)]string environment_name,
        [MarshalAs(UnmanagedType.LPStr)]string sensor_name);
    
    [DllImport("__Internal")]
    public static extern void pull_and_send_frames();

    private Thread serverThread;
    private string file_location;
    public ARSession arSession;

    // Currently it defaults to automatic running, switch boolean values to set to manual
    private bool launchingAuto = true;
    private bool launchingManual = false;

    // This is used in manual mode when to trigger sending a frame
    private bool screenTapped = false;

    // Start is called before the first frame update
    void Start()
    {
        // Register the touch event
        Input.simulateMouseWithTouches = true;
    }

    void RunManual()
    {
        Debug.Log("attempting to grab session");
        System.IntPtr session = (arSession.subsystem.nativePtr);
        use_session(session);
        Debug.Log(file_location);
        initialize(file_location, "2b212ecb-cd44-4004-9948-fb25787c59ac", "environment1", "adamunityiphone");
        // Now we run pull_and_send_frames at 5 frames a second, change if necessary
        while (true)
        {
            if (screenTapped)
            {
                pull_and_send_frames();
                screenTapped = false;
            }
            Thread.Sleep(200);
        }
    }

    void RunServer()
    {
        Debug.Log("attempting to grab session");
        System.IntPtr session = (arSession.subsystem.nativePtr);
        use_session(session);
        Debug.Log(file_location);
        ssp_server(file_location, "2b212ecb-cd44-4004-9948-fb25787c59ac", "environment1", "adamunityiphone");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            screenTapped = true;
        }

        if (launchingManual)
        {
            file_location = Application.dataPath + "/Raw/serve_ios_raw.yaml";
            //We create our new thread that be running the method "ListenForMessages"
            serverThread = new Thread(() => RunManual());
            //We configure the thread we just created
            serverThread.IsBackground = true;
            //We note that it is running so we don't forget to turn it off
            // threadRunning = true;
            //Now we start the thread
            serverThread.Start();
            launchingManual = false;
        }
        if (launchingAuto)
        {
            file_location = Application.dataPath + "/Raw/serve_ios_raw.yaml";
            //We create our new thread that be running the method "ListenForMessages"
            serverThread = new Thread(() => RunServer());
            //We configure the thread we just created
            serverThread.IsBackground = true;
            //We note that it is running so we don't forget to turn it off
            // threadRunning = true;
            //Now we start the thread
            serverThread.Start();
            launchingAuto = false;
        }
    }
    void OnDestroy()
    {
        serverThread.Abort();
    }
}
