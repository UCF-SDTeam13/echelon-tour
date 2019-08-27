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

object BLEManager {
    private lateinit var bluetoothAdapter: BluetoothAdapter
    private lateinit var bluetoothLeScanner: BluetoothLeScanner
    private val broadcastReceiver = BLEBroadcastReceiver()
    private val bleScanCallback = BLEScanCallback()
    // Doesn't matter what these are, they're passed back to us
    private val REQUEST_ENABLE_BLE = 1337
    //private val REQUEST_COARSE_LOCATION = 7331

    private val stateMap = mapOf(BluetoothAdapter.STATE_ON to 1, BluetoothAdapter.STATE_OFF to 0)

    @JvmStatic fun initialize (context: Context, activity: Activity) {
        bluetoothAdapter = (context.getSystemService(Context.BLUETOOTH_SERVICE) as BluetoothManager).adapter
        val filter = IntentFilter()
        activity.registerReceiver(broadcastReceiver, filter)
        bluetoothLeScanner = bluetoothAdapter.bluetoothLeScanner
    }

    @JvmStatic fun receiveUnityMessage(message: String) {
        BLEDebug.logInfo("Unity Message received")
        BLEDebug.logInfo("Unity Message: {$message}")
        BLEPeripheral.sendMessage(BLEProtocol.decode(message))
    }

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

        // NOTE2: Request on Unity side not here due to AndroidX issue
        /*
        val manifestPermissions = arrayOf(
            Manifest.permission.ACCESS_COARSE_LOCATION
        )

        ActivityCompat.requestPermissions(currentActivity, manifestPermissions, REQUEST_COARSE_LOCATION)
        */
    }
    @JvmStatic fun connect(context: Context, address: String) {
        stopScan()

        BLEDebug.logInfo("Request received for BLE Connect to {$address}")
        val addressValidation = BluetoothAdapter.checkBluetoothAddress(address)
        BLEDebug.logInfo("Address Validation Result: {$addressValidation}")
        val bleDevice = bluetoothAdapter.getRemoteDevice(address)
        BLEPeripheral.connect(context, bleDevice)
    }

    @JvmStatic fun scan() {
        BLEDebug.logInfo("Request received for BLE Scan")
        val filter = ScanFilter.Builder()
            .setServiceUuid(ParcelUuid(BLEProtocol.deviceUUID))
            .build()
        val scanSettings = ScanSettings.Builder()
            .setScanMode(ScanSettings.SCAN_MODE_LOW_LATENCY)
            .setCallbackType(ScanSettings.CALLBACK_TYPE_ALL_MATCHES)
            .build()
        bluetoothAdapter.bluetoothLeScanner.startScan(mutableListOf<ScanFilter>(filter), scanSettings, bleScanCallback)
        // bluetoothAdapter.bluetoothLeScanner.startScan(bleScanCallback)
    }

    @JvmStatic fun stopScan() {
        BLEDebug.logInfo("Request received to stop BLE Scan")
        bluetoothAdapter.bluetoothLeScanner.stopScan(bleScanCallback)
    }

    @JvmStatic fun discoverServices() {
        BLEDebug.logInfo("BLE Service Discovery Request Received")
        BLEPeripheral.serviceDiscovery()
    }

    @JvmStatic fun applicationQuit(activity: Activity) {
        BLEDebug.logInfo("BLE Application Quit Received")
        bluetoothAdapter.bluetoothLeScanner.stopScan(bleScanCallback)
        activity.unregisterReceiver(broadcastReceiver)
    }

    fun sendUnityMessage(message: String) {
        BLEDebug.logInfo("Sending Message from Native to Unity")
        UnityPlayer.UnitySendMessage("BLEPlugin", "ReceivePluginMessage", message)
    }

    private class BLEBroadcastReceiver : BroadcastReceiver() {
        override fun onReceive(context: Context, intent: Intent) {
            BLEDebug.logInfo("Broadcast Received")
            when (intent.action) {
                BluetoothAdapter.ACTION_STATE_CHANGED -> {
                    val state = intent.getIntExtra(BluetoothAdapter.EXTRA_STATE, BluetoothAdapter.ERROR)
                    BLEDebug.logInfo("ON/OFF Broadcast Received: $state")
                    when (state) {
                        BluetoothAdapter.STATE_ON, BluetoothAdapter.STATE_OFF -> {
                            sendUnityMessage("State Changed: " + stateMap.getValue(state))
                        }
                    }
                }
            }
        }
    }

    private class BLEScanCallback : ScanCallback() {
        override fun onScanResult(callbackType: Int, result: ScanResult?) {
            super.onScanResult(callbackType, result)
            BLEDebug.logInfo("Scan Result Received")
            when (callbackType) {
                SCAN_FAILED_ALREADY_STARTED -> BLEDebug.logInfo("SCAN_FAILED_ALREADY_STARTED")
                SCAN_FAILED_APPLICATION_REGISTRATION_FAILED -> BLEDebug.logError("SCAN_FAILED_APPLICATION_REGISTRATION_FAILED")
                SCAN_FAILED_FEATURE_UNSUPPORTED -> BLEDebug.logError("SCAN_FAILED_FEATURE_UNSUPPORTED")
                SCAN_FAILED_INTERNAL_ERROR -> BLEDebug.logError("SCAN_FAILED_INTERNAL_ERROR")
            }
            result?.apply {
                val scanRecord = result.scanRecord.toString()
                BLEDebug.logInfo("Scan Record: {$scanRecord}")
            }
        }
        override fun onScanFailed(errorCode: Int) {
            super.onScanFailed(errorCode)
            BLEDebug.logWarning("Scan failed - Error Code: {$errorCode}")
        }
    }
}