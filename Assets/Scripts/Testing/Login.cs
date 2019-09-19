using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


using Amazon;
using Amazon.Runtime;
using Amazon.CognitoIdentity;
using Amazon.CognitoSync;
using Amazon.CognitoSync.SyncManager;

public class SyncClient : MonoBehaviour
{
    public const string AppClientID = "59a98gf8s34jdqb90l07epsogs";
    public const string UserPoolID = "us-east-2_JX78GvZOE";
    public const string UserPoolName = "USERPOOLID";
    RegionEndpoint CognitoIdentityRegion = RegionEndpoint.USEast1;

    string username;
    string password;
    Dataset playerInfo;
    CognitoSyncManager syncManager;
    CognitoAWSCredentials credentials;

    // Start is called before the first frame update
    void Start()
    {
        UnityInitializer.AttachToGameObject(this.gameObject);

        // Remove if you want to build on IOS device.
        AWSConfigs.LoggingConfig.LogTo = LoggingOptions.UnityLogger;

        // Identity pool ID found under AWS >> Edit identity pool
        CognitoAWSCredentials credentials = new CognitoAWSCredentials(
            "us-east-2_JX78GvZOE", // IdentityPoolID
            RegionEndpoint.USEast1) // Region
        ;
    
    }
}
