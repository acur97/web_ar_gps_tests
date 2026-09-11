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
    private static extern float PreciseCompass_GetAlpha();

    [DllImport("__Internal")]
    private static extern float PreciseCompass_GetBeta();

    [DllImport("__Internal")]
    private static extern float PreciseCompass_GetGamma();

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

    public static float Alpha
    {
        get
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            return PreciseCompass_GetAlpha();
#else
            return 0f;
#endif
        }
    }

    public static float Beta
    {
        get
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            return PreciseCompass_GetBeta();
#else
            return 0f;
#endif
        }
    }

    public static float Gamma
    {
        get
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            return PreciseCompass_GetGamma();
#else
            return 0f;
#endif
        }
    }
}