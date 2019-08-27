package org.echelon.bleplugin

import android.util.Log

object BLEDebug {
    private const val enable = true
    const val tag = "UnityNativeBLE"

    fun logInfo(message: String) {
        if (enable) {
            Log.i(tag, message)
        }
    }

    fun logWarning(message: String) {
        Log.w(tag, message)
    }

    fun logError(message: String) {
        Log.e(tag, message)
    }
}