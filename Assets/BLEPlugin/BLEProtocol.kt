package org.echelon.bleplugin

import android.util.Base64
import java.util.UUID

object BLEProtocol {
    const val autoGattConnect = false
    const val chunkSize = 20
    val deviceUUID : UUID                   = UUID.fromString("0bf669f0-45f2-11e7-9598-0800200c9a66")
    val serviceUUID : UUID                  = UUID.fromString("0bf669f1-45f2-11e7-9598-0800200c9a66")
    val writeCharacteristicUUID : UUID      = UUID.fromString("0bf669f2-45f2-11e7-9598-0800200c9a66")
    val readCharacteristicUUID : UUID       = UUID.fromString("0bf669f3-45f2-11e7-9598-0800200c9a66")
    val notifyCharacteristicUUID : UUID     = UUID.fromString("0bf669f4-45f2-11e7-9598-0800200c9a66")
    // CCCD for Enabling Notifications
    val notifyDescriptorUUID : UUID         = UUID.fromString("00002902-0000-1000-8000-00805f9b34fb")
    const val startByte : Byte = 0xF0.toByte()
    const val frameLengthIndex = 2
    const val frameStart = 3
    const val minimumCommandLength = 4

    fun validateData(data: ByteArray): Boolean {
        if (data.size < minimumCommandLength) {
            return false
        }

        val frameLength = data[frameLengthIndex]

        if (minimumCommandLength + frameLength != data.size) {
            BLEDebug.logError("BLEProtocol Validation Failed: Total Length Incorrect")
            return false
        }

        return validateChecksum(data)
    }

    private fun validateChecksum(data: ByteArray):Boolean {
        val checksum = calculateChecksum(data)
        BLEDebug.logInfo("BLEProtocol Checksum Expected {$checksum}")
        return (checksum == data[data.lastIndex])
    }

    private fun calculateChecksum(data: ByteArray):Byte {
        return data.dropLast(1).sum().toByte()
    }

    fun decode(message: String): ByteArray {
        // once API version moves to 26+
        // use java.util.Base64 instead of android.util.Base64
        // NO_WRAP so that it doesn't expect line endings
        return Base64.decode(message, Base64.NO_WRAP)
    }

    fun encode(data: ByteArray): String {
        // NO_WRAP so that it doesn't add line endings
        return Base64.encodeToString(data, Base64.NO_WRAP)
    }
}