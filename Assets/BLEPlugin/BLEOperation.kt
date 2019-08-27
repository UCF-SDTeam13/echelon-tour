package org.echelon.bleplugin

import android.bluetooth.BluetoothGatt
import android.bluetooth.BluetoothGattCharacteristic

// TODO handle background queue / threading to prevent blocking the main thread

object BLEOperation {

    fun writeCharacteristic(bluetoothGatt : BluetoothGatt, characteristic: BluetoothGattCharacteristic, data: ByteArray) {
        characteristic.writeType = BluetoothGattCharacteristic.WRITE_TYPE_DEFAULT
        BLEDebug.logInfo("Write Data: {$data}")
        characteristic.value = data

        bluetoothGatt.writeCharacteristic(characteristic)
    }
}
