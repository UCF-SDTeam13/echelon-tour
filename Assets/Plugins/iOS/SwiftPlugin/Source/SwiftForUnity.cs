using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class SwiftForUnity : MonoBehaviour
{

    #region Declare external C interface

    #if UNITY_IOS && !UNITY_EDITOR
        
        [DllImport("__Internal")]
        private static extern string _sayHiToUnity();

        [DllImport("__Internal")]
        private static extern void _startScan();

        [DllImport("__Internal")]
        private static extern void _stopScan();

        [DllImport("__Internal")]
        private static extern void _connect();

        [DllImport("__Internal")]
        private static extern void _discoverServices();

        [DllImport("__Internal")]
        private static extern void _stopWorkout();

        [DllImport("__Internal")]
        private static extern void _startWorkout();

        [DllImport("__Internal")]
        private static extern void _pauseWorkout();

        [DllImport("__Internal")]
        private static extern void _increaseResistanceLevel();

        [DllImport("__Internal")]
        private static extern void _decreaseResistanceLevel();

        [DllImport("__Internal")]
        private static extern void _sendConnectIdentifier(string identifier);
    
    #endif

    #endregion

    #region Wrapped methods and properties

    public static string HiFromSwift()
    {
        #if UNITY_IOS && !UNITY_EDITOR
            return _sayHiToUnity();
        #else
            return "No Swift found!";
        #endif
    }

    public static void StartScan()
    {
        #if UNITY_IOS && !UNITY_EDITOR
            _startScan();
        #else
            Debug.Log("error stop scanning");
        #endif
    }

    public static void StopScan()
    {
        #if UNITY_IOS && !UNITY_EDITOR
            _stopScan();
        #else
            Debug.Log("error stop scanning");
        #endif
    }

    public static void Connect()
    {
        #if UNITY_IOS && !UNITY_EDITOR
            _connect();
        #else
            Debug.Log("error scanning");
        #endif
    }

    public static void ConnectWithIdentifier(string identifier)
    {
        #if UNITY_IOS && !UNITY_EDITOR
                _sendConnectIdentifier(identifier);
        #else
            Debug.Log("error scanning");
        #endif
    }

    public static void DiscoverServices()
    {
        #if UNITY_IOS && !UNITY_EDITOR
            _discoverServices();
        #else
            Debug.Log("error discovering services");
        #endif
    }

    public static void StopWorkout()
    {
        #if UNITY_IOS && !UNITY_EDITOR
            _stopWorkout();
        #else
            Debug.Log("error stopping workout");
        #endif
    }

    public static void StartWorkout()
    {
        #if UNITY_IOS && !UNITY_EDITOR
            _startWorkout();
        #else
            Debug.Log("error starting workout");
        #endif
    }

    public static void PauseWorkout()
    {
        #if UNITY_IOS && !UNITY_EDITOR
            _pauseWorkout();
        #else
            Debug.Log("error pausing workout");
        #endif
    }

    public static void IncreaseResistanceLevel()
    {
        #if UNITY_IOS && !UNITY_EDITOR
            _increaseResistanceLevel();
        #else
            Debug.Log("error increasing resistance");
        #endif
    }

    public static void DecreaseResistanceLevel()
    {
        #if UNITY_IOS && !UNITY_EDITOR
            _decreaseResistanceLevel();
        #else
            Debug.Log("error decreasing resistance");
        #endif
    }

    #endregion

    #region Singleton implementation

    private static SwiftForUnity _instance;
    public static SwiftForUnity Instance
    {
        get
        {
            if (_instance == null)
            {
                var obj = new GameObject("SwiftForUnity");
                _instance = obj.AddComponent<SwiftForUnity>();
            }

            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    #endregion

    #region Delegates
    public void OnDiscoverServices(string byteData)
    {
        Debug.Log("StartOnDiscover");
        BLEAction.ProcessReceiveData(BLEProtocol.ConvertStringToBytes(byteData));
        Debug.Log("EndOnDiscover");
    }

    public void OnReceivePeripherals(string peripheral)
    {
        Debug.Log("StartReceivePeripherals");
        Debug.Log(peripheral);
        Debug.Log("EndReceivePeripherals");
    }
    #endregion
}
