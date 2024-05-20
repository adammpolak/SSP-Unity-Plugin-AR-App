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
    private Thread serverThread;
    private string file_location;
    public ARSession arSession;
    private bool hasntLaunched = true;
    // Start is called before the first frame update
    void Start()
    {


    }
    void RunServer()
    {
        Debug.Log("attempting to grab session");
        System.IntPtr session = (arSession.subsystem.nativePtr);
        use_session(session);
        Debug.Log(file_location);
        ssp_server(file_location);
    }
    // Update is called once per frame
    void Update()
    {
        if (hasntLaunched)
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
            hasntLaunched = false;
        }
    }
    void OnDestroy()
    {
        serverThread.Abort();
    }
}

