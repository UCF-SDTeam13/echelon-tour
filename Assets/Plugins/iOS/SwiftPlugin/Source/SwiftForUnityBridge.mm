#import <CoreBluetooth/CoreBluetooth.h>

#include "SwiftPlugin-Swift.h"
#pragma mark - C interface

extern "C" {
    char* _sayHiToUnity() {
        NSString *returnString = [[SwiftForUnity shared] SayHiToUnity];
        char* cStringCopy(const char* string);
        return cStringCopy([returnString UTF8String]);
    }
    
    void _startScan() {
        [[SwiftForUnity shared] startScan];
    }
    
    void _stopScan() {
        [[SwiftForUnity shared] stopScan];
    }
    
    void _connect() {
        [[SwiftForUnity shared] connect];
    }
    
    void _sendConnectIdentifier(const char* identifier) {
        [SwiftForUnity shared].identifierString = [NSString stringWithCString:identifier encoding:NSUTF8StringEncoding];
        [[SwiftForUnity shared] connectWithIdentifier];
    }
    
    void _discoverServices() {
        [[SwiftForUnity shared] discoverServices];
    }
    
    void _stopWorkout() {
        [[SwiftForUnity shared] stopWorkout];
    }
    
    void _startWorkout() {
        [[SwiftForUnity shared] startWorkout];
    }
    
    void _pauseWorkout() {
        [[SwiftForUnity shared] pauseWorkout];
    }
    
    void _increaseResistanceLevel() {
        [[SwiftForUnity shared] increaseResistanceLevel];
    }
    
    void _decreaseResistanceLevel() {
        [[SwiftForUnity shared] decreaseResistanceLevel];
    }
}

char* cStringCopy(const char* string){
    if (string == NULL) {
        return NULL;
    }
    
    char* res = (char*)malloc(strlen(string)+1);
    strcpy(res, string);
    return res;
}
