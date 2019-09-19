using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;

using Amazon;
using Amazon.Runtime;
using Amazon.CognitoIdentity;
using Amazon.CognitoSync;
using Amazon.CognitoSync.SyncManager;

public class SyncClient : MonoBehaviour
{
    public GameObject LoginUI;
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
        credentials = new CognitoAWSCredentials("us-east-2:7d10094b-a03d-4641-bb01-0c077a64df98", RegionEndpoint.USEast1);
        syncManager = new CognitoSyncManager(credentials, RegionEndpoint.USEast1);
        playerInfo = syncManager.OpenOrCreateDataset("playerInfo");
    
    }

    public void ChangeUsername(string newUsername)
    {
        username = newUsername;
        playerInfo.Put("username", newUsername);
    }

    public void ChangePassword(string newPassword)
    {
        password = newPassword;
        playerInfo.Put("password", newPassword);
    }

    public void Synchronize()
    {
        playerInfo.SynchronizeOnConnectivity();
    }
}
