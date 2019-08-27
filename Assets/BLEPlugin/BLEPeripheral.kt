package org.echelon.bleplugin

import android.bluetooth.*
import android.content.Context

object BLEPeripheral {
    private lateinit var bleGatt: BluetoothGatt
    private lateinit var writeCharacteristic: BluetoothGattCharacteristic
    private lateinit var readCharacteristic: BluetoothGattCharacteristic
    private lateinit var notifyCharacteristic: BluetoothGattCharacteristic
    private val readBuffer = mutableListOf<Byte>()
    private val notifyBuffer = mutableListOf<Byte>()

    fun connect(context: Context, bluetoothDevice: BluetoothDevice) {
        bleGatt = bluetoothDevice.connectGatt(context, BLEProtocol.autoGattConnect, BLEGattCallback())
    }

    fun serviceDiscovery() {
        bleGatt.discoverServices()
    }

    fun sendMessage(data: ByteArray) {
        // BLE specifications limit to a 20 byte MTU
        // while it might be possible to use a higher MTU of up to 512
        // I doubt the factory programmed adjustable MTU into the chip
        val packets = data.toList().chunked(BLEProtocol.chunkSize)
        val stringUUID = writeCharacteristic.uuid.toString()

        BLEDebug.logInfo("UUID of Write Characteristic: {$stringUUID}")

        packets.forEach {
            BLEOperation.writeCharacteristic(bleGatt, writeCharacteristic, it.toByteArray())
        }
    }

    fun onCharacteristicDiscovery() {
        // NOTE: Order of notification descriptors seems to matter
        BLEDebug.logInfo("Subscribing to Notifications for Notify Characteristic")
        bleGatt.setCharacteristicNotification(notifyCharacteristic, true)
        val notifyDescriptor = notifyCharacteristic.getDescriptor(BLEProtocol.notifyDescriptorUUID)
        notifyDescriptor.value = BluetoothGattDescriptor.ENABLE_NOTIFICATION_VALUE
        bleGatt.writeDescriptor(notifyDescriptor)

        BLEDebug.logInfo("Subscribing to Notifications for Read Characteristic")
        bleGatt.setCharacteristicNotification(readCharacteristic, true)
        val readDescriptor = readCharacteristic.getDescriptor(BLEProtocol.notifyDescriptorUUID)
        readDescriptor.value = BluetoothGattDescriptor.ENABLE_NOTIFICATION_VALUE
        bleGatt.writeDescriptor(readDescriptor)
    }
    private fun syncBuffer(buffer: MutableList<Byte>) {
        val syncIndex = buffer.indexOf(BLEProtocol.startByte)

        if (syncIndex == -1) {
            // this (should) never happen but just in case it does
            BLEDebug.logWarning("SyncByte {${BLEProtocol.startByte}} not found, Dropping all {${buffer.size}} bytes from buffer")
            buffer.forEach{
                BLEDebug.logWarning("$it")
            }

            buffer.clear()
        } else {
            BLEDebug.logInfo("Buffer Bytes Before Drop:")
            buffer.forEach{
                BLEDebug.logInfo("${it.toString(16)}")
            }
            BLEDebug.logInfo("Dropping {$syncIndex} bytes from buffer")
            val bufferKeep = buffer.drop(syncIndex)
            buffer.clear()
            buffer.addAll(bufferKeep)
            BLEDebug.logInfo("Retained Bytes:")
            buffer.forEach{
                BLEDebug.logInfo("$it")
            }
        }
    }

    private fun processBuffer(buffer : MutableList<Byte>, characteristic : BluetoothGattCharacteristic) {
        BLEDebug.logInfo("Processing Buffer")
        buffer.addAll(characteristic.value.toList())

        // there shouldn't be any sliding, but for now we'll assume there is
        syncBuffer(buffer)
        if (buffer.size < BLEProtocol.minimumCommandLength) {
            // Received incomplete packet or buffer drop is incorrect - unlikely but recoverable
            BLEDebug.logWarning("Buffer size {${buffer.size}} is smaller than Minimum Command Length")
            return
        }
        val dataLength = buffer[BLEProtocol.frameLengthIndex] + BLEProtocol.minimumCommandLength
        if (buffer.size < dataLength) {
            // Waiting for additional packets - probably shouldn't happen
            BLEDebug.logWarning("Buffer size below Minimum Command Length")
            return
        }

        val data = buffer.take(dataLength).toByteArray()

        if (BLEProtocol.validateData(data)) {
            BLEManager.sendUnityMessage(BLEProtocol.encode(data))
            buffer.retainAll(buffer.drop(dataLength))
        }
        else {
            // Correct length but fails validation - Definitely an error
            BLEDebug.logError("Buffer failed validation")
            return
        }

    }

    private fun processWrite() {
        BLEDebug.logInfo("Characteristic Write committed")
    }

    private class BLEGattCallback : BluetoothGattCallback() {
        override fun onConnectionStateChange(gatt: BluetoothGatt?, status: Int, newState: Int) {
            super.onConnectionStateChange(gatt, status, newState)
            BLEDebug.logInfo("BLE Connection State Changed")
            when (newState) {
                BluetoothProfile.STATE_CONNECTED -> {
                    BLEDebug.logInfo("BLE Connection State Connected")
                    serviceDiscovery()
                }
                BluetoothProfile.STATE_DISCONNECTED -> BLEDebug.logInfo("BLE Connection State Changed")
            }
        }
        override fun onServicesDiscovered(gatt: BluetoothGatt?, status: Int) {
            super.onServicesDiscovered(gatt, status)
            BLEDebug.logInfo("BLE Service Discovery Completed")
            // Find Gatt Service by Service UUID
            val bleGattService = gatt?.services!!.first { it.uuid == BLEProtocol.serviceUUID }
            writeCharacteristic = bleGattService.characteristics.first{it.uuid == BLEProtocol.writeCharacteristicUUID}
            readCharacteristic = bleGattService.characteristics.first{it.uuid == BLEProtocol.readCharacteristicUUID}
            notifyCharacteristic = bleGattService.characteristics.first{it.uuid == BLEProtocol.notifyCharacteristicUUID}
            BLEDebug.logInfo("BLEPeripheral Characteristics Discovered")
            onCharacteristicDiscovery()
        }

        override fun onCharacteristicChanged(gatt: BluetoothGatt?, characteristic: BluetoothGattCharacteristic?) {
            super.onCharacteristicChanged(gatt, characteristic)
            BLEDebug.logInfo("Characteristic Change Detected")
            BLEDebug.logInfo("${characteristic?.uuid}")
            when (characteristic?.uuid) {
                BLEProtocol.readCharacteristicUUID -> processBuffer(readBuffer, readCharacteristic)
                BLEProtocol.writeCharacteristicUUID -> processWrite()
                BLEProtocol.notifyCharacteristicUUID -> {
                    BLEDebug.logInfo("Notify Changed")
                    processBuffer(notifyBuffer, notifyCharacteristic)
                }
            }
        }
    }
}