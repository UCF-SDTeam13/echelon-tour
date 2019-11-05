import CoreBluetooth
import Foundation
import UIKit

let kCallbackTarget = "SwiftForUnity"

let bikeDeviceCBUUID = CBUUID(string: "0bf669f0-45f2-11e7-9598-0800200c9a66")
let bikeServiceCBUUID = CBUUID(string: "0bf669f1-45f2-11e7-9598-0800200c9a66")
let bikeWriteCBUUID = CBUUID(string: "0bf669f2-45f2-11e7-9598-0800200c9a66")
let bikeReadCBUUID = CBUUID(string: "0bf669f3-45f2-11e7-9598-0800200c9a66")
let bikeNotifyCBUUID = CBUUID(string: "0bf669f4-45f2-11e7-9598-0800200c9a66")

let LengthIndex = 2
let MinCmdLen = 4

@objc class SwiftForUnity: UIViewController {
    @objc static let shared = SwiftForUnity()
    @objc var centralManager: CBCentralManager!
    @objc var bikePeripheral: CBPeripheral!
    @objc var writeCharacteristic: CBCharacteristic!
    
    @objc var resistanceLevel:Int = 0
    
    @objc func SayHiToUnity() -> String {
        return "Hello from Swift"
    }
    
    @objc func startScan() {
        print("start scan")
        centralManager = CBCentralManager(delegate: self, queue: nil)
    }
    
    @objc func stopScan() {
        centralManager.stopScan()
    }
    
    @objc func connect() {
        stopScan()
        centralManager.connect(bikePeripheral)
    }
    
    @objc func discoverServices() {
        bikePeripheral.discoverServices([bikeServiceCBUUID])
    }
    
    @objc func message(unityString: String) {
        
    }
    
    @objc func stopWorkout() {
        let data:[UInt8] = [0xF0, 0xB0, 0x01, UInt8(0), 0x00]
        handleSendData(from: data)
    }
    
    @objc func startWorkout() {
        let data:[UInt8] = [0xF0, 0xB0, 0x01, UInt8(1), 0x00]
        handleSendData(from: data)
    }
    
    @objc func pauseWorkout() {
        let data:[UInt8] = [0xF0, 0xB0, 0x01, UInt8(2), 0x00]
        handleSendData(from: data)
    }
    
    @objc func increaseResistanceLevel() {
        if (resistanceLevel+1 <= 32) {
            resistanceLevel += 1
            
            print("Resistance level: ", resistanceLevel)
            let data:[UInt8] = [0xF0, 0xB1, 0x01, UInt8(resistanceLevel), 0x00]
            handleSendData(from: data)
        }
        else {
            print("Resistence Level at max")
        }
    }
    
    @objc func decreaseResistanceLevel() {
        if (resistanceLevel-1 >= 0) {
            resistanceLevel -= 1
            
            print("Resistance level: ", resistanceLevel)
            let data:[UInt8] = [0xF0, 0xB1, 0x01, UInt8(resistanceLevel), 0x00]
            handleSendData(from: data)
        }
        else {
            print("Resistence Level at min")
        }
    }
    
    @objc func getResistanceLevel() {
        //    let data:[UInt8] = [0xF0, 0xA5, 0x01, 0x00, 0x00]
        //    handleSendData(from: data)
    }
    
    @objc func getWorkoutStatus() {
        
    }
    
    @objc func handleSendData(from iData: [UInt8]) {
        var data = iData
        let checksum = calculateChecksum(from: data)
        data[data.count-1] = UInt8(checksum)
        
        let writeData =  Data(_ : data)
        
        print([UInt8](writeData))
        
        bikePeripheral.writeValue(writeData, for: self.writeCharacteristic, type: CBCharacteristicWriteType.withResponse)
    }
}

@objc extension SwiftForUnity: CBCentralManagerDelegate {
    func centralManagerDidUpdateState(_ central: CBCentralManager) {
        switch central.state {
        case .unknown:
            print("central.state is .unknown")
        case .resetting:
            print("central.state is .resetting")
        case .unsupported:
            print("central.state is .unsupported")
        case .unauthorized:
            print("central.state is .unauthorized")
        case .poweredOff:
            print("central.state is .poweredOff")
            if (self.bikePeripheral != nil) {
                self.bikePeripheral = nil;
            }
        case .poweredOn:
            print("central.state is .poweredOn")
            print(bikeDeviceCBUUID)
            centralManager.scanForPeripherals(withServices: [bikeDeviceCBUUID], options: [CBCentralManagerScanOptionAllowDuplicatesKey: true])
        @unknown default:
            print("default")
        }
    }
    
    func centralManager(_ central: CBCentralManager, didDiscover peripheral: CBPeripheral, advertisementData: [String : Any], rssi RSSI: NSNumber) {
        print(peripheral)
        bikePeripheral = peripheral
        bikePeripheral.delegate = self
        centralManager.stopScan()
        //    centralManager.connect(bikePeripheral)
    }
    
    func centralManager(_ central: CBCentralManager, didConnect peripheral: CBPeripheral) {
        print("Connected!")
    }
    
    func cancelConnection() {
        centralManager.cancelPeripheralConnection(self.bikePeripheral)
        self.bikePeripheral = nil
    }
    
    func centralManager(_ central: CBCentralManager, didFailToConnect peripheral: CBPeripheral, error: Error?) {
        print("Failed to connect")
        
        if(bikePeripheral != nil) {
            //reset
        }
        // todo?
        //    if (bikePeripheral != nil) {
        //      bikePeripheral = nil
        //    }
    }
}

@objc extension SwiftForUnity: CBPeripheralDelegate {
    
    func peripheral(_ peripheral: CBPeripheral, didDiscoverServices error: Error?) {
        guard let services = peripheral.services else { return }
        
        for service in services {
            print(service)
            peripheral.discoverCharacteristics(nil, for: service)
        }
    }
    
    func peripheral(_ peripheral: CBPeripheral, didDiscoverCharacteristicsFor service: CBService,
                    error: Error?) {
        if(service.uuid == bikeServiceCBUUID) {
            
            guard let characteristics = service.characteristics else { return }
            
            for characteristic in characteristics {
                print(characteristic)
                if characteristic.properties.contains(.read) {
                    print("\(characteristic.uuid): properties contains .read")
                    peripheral.readValue(for: characteristic)
                }
                if characteristic.properties.contains(.notify) {
                    print("\(characteristic.uuid): properties contains .notify")
                    peripheral.setNotifyValue(true, for: characteristic)
                }
                if characteristic.properties.contains(.write) {
                    print("\(characteristic.uuid): properties contains .write")
                    peripheral.setNotifyValue(true, for: characteristic)
                    writeCharacteristic = characteristic
                }
            }
        }
    }
    
    func peripheral(_ peripheral: CBPeripheral, didUpdateValueFor characteristic: CBCharacteristic,
                    error: Error?) {
        switch characteristic.uuid {
        case bikeWriteCBUUID:
            print("write")
            let bikeWriteValue = bikeNotifyHandler(from: characteristic)
        case bikeReadCBUUID:
            print("read")
        case bikeNotifyCBUUID:
            let bikeNotifyValue = bikeNotifyHandler(from: characteristic)
            
//            if (bikeNotifyValue == -1) {
//                UnitySendMessage(kCallbackTarget, "OnDiscoverServices", "test")
//            }
//            else {
//                print("error! bikeNotifyValue = -1")
//            }
        default:
            print("Unhandled Characteristic UUID: \(characteristic.uuid)")
        }
    }
    
    func peripheral(_ peripheral: CBPeripheral, didWriteValueFor characteristic: CBCharacteristic, error: Error?) {
        print("value written")
        peripheral.readValue(for: characteristic)
    }
    
    func centralManager(_ central: CBCentralManager, didDisconnectPeripheral peripheral: CBPeripheral, error: Error?) {
        if (bikePeripheral != nil) {
            self.bikePeripheral = nil
            
            // todo?
        }
    }
    
    private func bikeNotifyHandler(from characteristic: CBCharacteristic) -> Int {
        guard let characteristicData = characteristic.value else { return -1 }
        
        let byteArray = [UInt8](characteristicData)
        let totalLength = byteArray.count
        
        if (byteArray.count < MinCmdLen) {
            print("Array size is smaller than the Minimum Command Length")
            return -1
        }
        
        let dataLength = Int(byteArray[LengthIndex]) + MinCmdLen
        
        if(totalLength < dataLength) {
            print("Array size below Minimum Command Length")
            return -1
        }
        
        if(verifyChecksum(from: byteArray)) {
            //send unity message
            processCommand(from: byteArray)
            return 1
        }
        else {
            print("byteArray failed validation.")
            return -1
        }
    }
    
    private func verifyChecksum(from byteArray: [UInt8]) -> Bool {
        if (byteArray.count < MinCmdLen) {
            return false
        }
        
        let frameLength = Int(byteArray[LengthIndex])
        if ((MinCmdLen + frameLength) != byteArray.count) {
            print("Validation Failed: Total Length Incorrect")
            return false
        }
        
        return validateChecksum(from: byteArray)
    }
    
    private func validateChecksum(from byteArray: [UInt8]) -> Bool {
        let checksum = calculateChecksum(from: byteArray)
        print("Checksum expected", byteArray[byteArray.count-1], "Checksum Received", checksum)
        
        return (checksum == byteArray.last)
    }
    
    private func calculateChecksum(from byteArray: [UInt8]) -> UInt8 {
        var sum = 0
        var count = 0
        
        for byte in byteArray {
            count+=1
            if (count != byteArray.count) {
                sum = sum + Int(byte)
            }
        }
        
        return UInt8(sum & 0xff)
    }
    
    private func processCommand(from byteArray: [UInt8]) {
        
    }
}

