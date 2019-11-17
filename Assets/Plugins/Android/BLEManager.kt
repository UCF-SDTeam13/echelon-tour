package org.echelon.bleplugin


import android.app.Activity
import android.bluetooth.BluetoothAdapter
import android.bluetooth.BluetoothManager
import android.bluetooth.le.BluetoothLeScanner
import android.bluetooth.le.ScanCallback
import android.bluetooth.le.ScanFilter
import android.bluetooth.le.ScanResult
import android.bluetooth.le.ScanSettings

import android.content.BroadcastReceiver
import android.content.Context
import android.content.Intent
import android.content.IntentFilter
import android.os.ParcelUuid
import com.unity3d.player.UnityPlayer

// Singleton to Handle Android Side of Unity<->Android Communication
object BLEManager {
    // Bluetooth Adapter - Used for Most BLE Operations
    private lateinit var bluetoothAdapter: BluetoothAdapter
    // BLE Scanner - Specifically For (Filtered) Scanning
    private lateinit var bluetoothLeScanner: BluetoothLeScanner
    // Broadcast Receiver to Know About Bluetooth Changes
    private val broadcastReceiver = BLEBroadcastReceiver()
    // Callback to Handle Scan Results
    private val bleScanCallback = BLEScanCallback()
    // Doesn't matter what the value of this is - it's passed back to us
    private val REQUEST_ENABLE_BLE = 1337

    private val stateMap = mapOf(BluetoothAdapter.STATE_ON to 1, BluetoothAdapter.STATE_OFF to 0)

    // NOTE: Kotlin generates getter as isBLEEnabled not getIsBLEEnabled!
    @JvmStatic val isBLEEnabled: Boolean
        get() = bluetoothAdapter.isEnabled

    // Initalize the BLEManager Singleton
    @JvmStatic fun initialize (context: Context, activity: Activity) {
        // Get BluetoothAdapter using Context
        bluetoothAdapter = (context.getSystemService(Context.BLUETOOTH_SERVICE) as BluetoothManager).adapter
        // Register with Broadcast Receiver to Receive Bluetooth Changes
        val filter = IntentFilter() // TODO - Makes this more restrictive
        activity.registerReceiver(broadcastReceiver, filter)
        bluetoothLeScanner = bluetoothAdapter.bluetoothLeScanner
    }
    // Receive Message to be Sent over Bluetooth from Unity
    @JvmStatic fun receiveUnityMessage(message: String) {
        BLEDebug.logInfo("Unity Message received")
        BLEDebug.logInfo("Unity Message: {$message}")
        BLEPeripheral.sendMessage(BLEProtocol.decode(message))
    }

    // Request to Enable Bluetooth - Intent Fires Only if Not Enabled Already
    @JvmStatic fun requestEnableBLE(currentActivity: Activity) {
        BLEDebug.logInfo("Request received for BLE Enable")
        // Start Activity to Enable BLE if not enabled already
        bluetoothAdapter.takeUnless { it.isEnabled }?.apply {
            val enableBLEIntent = Intent(BluetoothAdapter.ACTION_REQUEST_ENABLE)
            currentActivity.startActivityForResult(enableBLEIntent, REQUEST_ENABLE_BLE)
        }
        // Ask for location permissions since ACCESS_COARSE_LOCATION
        // is rated dangerous and needs to be asked for at runtime
        // even though it is already in the manifest

        // NOTE: Ideally we would request that here but we
        // request on Unity side due to AndroidX issue
    }

    // Connect to a BLE Device by Mac Address
    @JvmStatic fun connect(context: Context, address: String) {
        // Stop Scanning Since We're Going to Connect
        stopScan()

        // Validate BLE Hardware Address
        BLEDebug.logInfo("Request received for BLE Connect to {$address}")
        val addressValidation = BluetoothAdapter.checkBluetoothAddress(address)
        // TODO - Handle Invalid BLE Address (Low Priority Since Addresses Should Only Come From Scanning)
        BLEDebug.logInfo("Address Validation Result: {$addressValidation}")

        // Connect to BLE Device by Address
        val bleDevice = bluetoothAdapter.getRemoteDevice(address)
        BLEPeripheral.connect(context, bleDevice)
    }

    // Scan for BLE Devices
    @JvmStatic fun scan() {
        BLEDebug.logInfo("Request received for BLE Scan")
        // Filter to Only the Device Service UUID to Ignore Other Devices
        val filter = ScanFilter.Builder()
            .setServiceUuid(ParcelUuid(BLEProtocol.deviceUUID))
            .build()
        // Set Fast Scanning / Tell Android We Want Results as They Come In
        val scanSettings = ScanSettings.Builder()
            .setScanMode(ScanSettings.SCAN_MODE_LOW_LATENCY)
            .setCallbackType(ScanSettings.CALLBACK_TYPE_ALL_MATCHES)
            .build()
        // TODO - This should have a timeout so that it doesn't look forever
        bluetoothAdapter.bluetoothLeScanner.startScan(mutableListOf<ScanFilter>(filter), scanSettings, bleScanCallback)
    }

    // Stop BLE Scanning
    @JvmStatic fun stopScan() {
        BLEDebug.logInfo("Request received to stop BLE Scan")
        bluetoothAdapter.bluetoothLeScanner.stopScan(bleScanCallback)
    }

    // Ask BLE Device What Services It Has - Necessary Because We Don't Connect to the Advertised Service
    @JvmStatic fun discoverServices() {
        BLEDebug.logInfo("BLE Service Discovery Request Received")
        BLEPeripheral.serviceDiscovery()
    }

    // Handle Application Closing
    @JvmStatic fun applicationQuit(activity: Activity) {
        BLEDebug.logInfo("BLE Application Quit Received")
        // Stop Active Scan
        bluetoothAdapter.bluetoothLeScanner.stopScan(bleScanCallback)
        // Unregister Broadcast Receiver
        activity.unregisterReceiver(broadcastReceiver)
    }

    // Send a Message Received From BLE to Unity
    fun sendUnityMessage(message: String) {
        BLEDebug.logInfo("Sending Message from Native to Unity")
        UnityPlayer.UnitySendMessage("DontDestroyOnLoad", "ReceivePluginMessage", message)
    }

    // Handle Bluetooth Adapter State Changes
    private class BLEBroadcastReceiver : BroadcastReceiver() {
        override fun onReceive(context: Context, intent: Intent) {
            BLEDebug.logInfo("Broadcast Received")
            when (intent.action) {
                BluetoothAdapter.ACTION_STATE_CHANGED -> {
                    val state = intent.getIntExtra(BluetoothAdapter.EXTRA_STATE, BluetoothAdapter.ERROR)
                    BLEDebug.logInfo("ON/OFF Broadcast Received: $state")
                    when (state) {
                        BluetoothAdapter.STATE_ON, BluetoothAdapter.STATE_OFF -> {
                            // TODO - Change to a BLE Status Callback
                            sendUnityMessage("State Changed: " + stateMap.getValue(state))
                        }
                    }
                }
            }
        }
    }
    // Handle Scan Results
    private class BLEScanCallback : ScanCallback() {
        override fun onScanResult(callbackType: Int, result: ScanResult?) {
            super.onScanResult(callbackType, result)
            // TODO - Notify Unity of MATCH_LOST
            when(callbackType) {
                ScanSettings.CALLBACK_TYPE_ALL_MATCHES -> BLEDebug.logInfo("CALLBACK_TYPE_ALL_MATCHES")
                ScanSettings.CALLBACK_TYPE_FIRST_MATCH -> BLEDebug.logInfo("CALLBACK_TYPE_FIRST_MATCH")
                ScanSettings.CALLBACK_TYPE_MATCH_LOST -> BLEDebug.logInfo("CALLBACK_TYPE_MATCH_LOST")
            }
            BLEDebug.logInfo("Scan Result Received")
            // Notify Unity of Scan Results
            result?.apply {
                val scanRecordLog = result.scanRecord.toString()
                val scanName = result.scanRecord.deviceName
                val scanAddress = result.device.address
                BLEDebug.logInfo("Match Found Name: $scanName|$scanAddress")
                UnityPlayer.UnitySendMessage("DontDestroyOnLoad", "ReceiveMatch", "$scanName|$scanAddress")
            }
        }
        // TODO - Notify Unity of Scan Errors
        override fun onScanFailed(errorCode: Int) {
            super.onScanFailed(errorCode)
            when (errorCode) {
                SCAN_FAILED_ALREADY_STARTED -> BLEDebug.logWarning("SCAN_FAILED_ALREADY_STARTED")
                SCAN_FAILED_APPLICATION_REGISTRATION_FAILED -> BLEDebug.logError("SCAN_FAILED_APPLICATION_REGISTRATION_FAILED")
                SCAN_FAILED_FEATURE_UNSUPPORTED -> BLEDebug.logError("SCAN_FAILED_FEATURE_UNSUPPORTED")
                SCAN_FAILED_INTERNAL_ERROR -> BLEDebug.logError("SCAN_FAILED_INTERNAL_ERROR")
            }
        }
    }
}