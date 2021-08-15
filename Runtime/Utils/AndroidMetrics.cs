/* Copyright (C) 2021 Vadimskyi - All Rights Reserved
 * Github - https://github.com/Vadimskyi
 * Website - https://www.vadimskyi.com/
 * You may use, distribute and modify this code under the
 * terms of the GPL-3.0 License.
 */
using UnityEngine;

namespace VadimskyiLab.Android
{
    public static class AndroidMetrics
    {
        // The logical density of the display
        public static float Density { get; private set; }

        // The screen density expressed as dots-per-inch
        public static int DensityDPI { get; private set; }

        // The absolute height of the display in pixels
        public static int HeightPixels { get; private set; }

        // The absolute width of the display in pixels
        public static int WidthPixels { get; private set; }

        // A scaling factor for fonts displayed on the display
        public static float ScaledDensity { get; private set; }

        // The exact physical pixels per inch of the screen in the X dimension
        public static float XDPI { get; private set; }

        // The exact physical pixels per inch of the screen in the Y dimension
        public static float YDPI { get; private set; }

        public static float ScreenDpi
        {
            get { return (XDPI + YDPI) * 0.5f; }
        }

        static AndroidMetrics()
        {
            CalculateAndroidMetrics();
        }

        public static void CalculateAndroidMetrics()
        {
            // The following is equivalent to this Java code:
            //
            // metricsInstance = new DisplayMetrics();
            // UnityPlayer.currentActivity.getWindowManager().getDefaultDisplay().getMetrics(metricsInstance);
            //
            // ... which is pretty much equivalent to the code on this page:
#if UNITY_EDITOR
            if (Screen.width == 800 && Screen.height == 1200) // Samsung Galaxy Tab Active2
            {
                XDPI = 189;
                YDPI = 189;
                Density = 1.5f;
            }
            else if (Screen.width == 720 && Screen.height == 1448) // Samsung Galaxy S10+ low res         
            {
                XDPI = 267;
                YDPI = 267;
                Density = 1.5f;
            }
            else if (Screen.width == 720 && Screen.height == 1280) // Samsung Galaxy J7             
            {
                XDPI = 267;
                YDPI = 267;
                Density = 1.5f;
            }
            else if (Screen.width == 1440 && Screen.height == 3200) // Samsung S20 Ultra
            {
                XDPI = 511;
                YDPI = 511;
                Density = 3.5f;
            }
            else if (Screen.width == 1440 && Screen.height == 2560) // Samsung Galaxy S7
            {
                XDPI = 577;
                YDPI = 577;
                Density = 3f;
            }
            else if (Screen.width == 1668 && Screen.height == 2388) // iPad Pro 11
            {
                XDPI = 265;
                YDPI = 265;
                Density = 2f;
            }
            else if (Screen.width == 1600 && Screen.height == 2560)    // Samsung Galaxy Tab S 10.5
            {
                XDPI = 288;
                YDPI = 288;
                Density = 2f;
            }
            else if (Screen.width == 1080 && Screen.height == 1920)    // Nexus 5
            {
                XDPI = 445;
                YDPI = 445;
                Density = 3f;
            }
            else if (Screen.width == 1440 && Screen.height == 2960)    // S9
            {
                XDPI = 570;
                YDPI = 570;
                Density = 3.5f;
            }
            else
            {
                XDPI = 403.2f;
                YDPI = 403.2f;
                Density = 2.5f;
            }

#else
            using (
                AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"),
                metricsClass = new AndroidJavaClass("android.util.DisplayMetrics")
            )
            {
                using (
                    AndroidJavaObject metricsInstance = new AndroidJavaObject("android.util.DisplayMetrics"),
                    activityInstance = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity"),
                    windowManagerInstance = activityInstance.Call<AndroidJavaObject>("getWindowManager"),
                    displayInstance = windowManagerInstance.Call<AndroidJavaObject>("getDefaultDisplay")
                )
                {
                    displayInstance.Call("getRealMetrics", metricsInstance);
                    Density = metricsInstance.Get<float>("density");
                    DensityDPI = metricsInstance.Get<int>("densityDpi");
                    HeightPixels = metricsInstance.Get<int>("heightPixels");
                    WidthPixels = metricsInstance.Get<int>("widthPixels");
                    ScaledDensity = metricsInstance.Get<float>("scaledDensity");
                    XDPI = metricsInstance.Get<float>("xdpi");
                    YDPI = metricsInstance.Get<float>("ydpi");
                }
            }
#endif
        }
    }
}
