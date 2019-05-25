// ------------------------------------------------------------------
// Description : 原生功能Editor平台实现
// Author      : sunlu
// Date        : 2015.01.05
// Histories   :
// ------------------------------------------------------------------

using System;

namespace QGame.Core.Device
{
    public class DeviceFactory
    {
#if UNITY_EDITOR
        public static DeviceNative Instance = new EditorDevice();//EditorDevice   EditorTestAssets WindowsDevice
#elif UNITY_ANDROID
        public static DeviceNative Instance = new AndroidDevice();
#elif UNITY_STANDALONE_WIN
        public static DeviceNative Instance = new WindowsDevice();
#elif UNITY_IOS
        public static DeviceNative Instance = new IOSDevice();
#else
        public static DeviceNative Instance = new WindowsDevice();
#endif
    }
}
