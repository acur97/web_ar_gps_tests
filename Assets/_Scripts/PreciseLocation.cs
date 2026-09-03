#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

public static class PreciseLocation
{
#if UNITY_WEBGL && !UNITY_EDITOR

    [DllImport("__Internal")]
    private static extern void PreciseLocation_Install();

    [DllImport("__Internal")]
    private static extern double PreciseLocation_GetLatitude();

    [DllImport("__Internal")]
    private static extern double PreciseLocation_GetLongitude();

    [DllImport("__Internal")]
    private static extern double PreciseLocation_GetAccuracy();

#endif

    public static void Install()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        PreciseLocation_Install();
#endif
    }

    public static double Latitude
    {
        get
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            return PreciseLocation_GetLatitude();
#else
            return 0.0;
#endif
        }
    }

    public static double Longitude
    {
        get
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            return PreciseLocation_GetLongitude();
#else
            return 0.0;
#endif
        }
    }

    public static double Accuracy
    {
        get
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            return PreciseLocation_GetAccuracy();
#else
            return 0.0;
#endif
        }
    }
}