using UnityEngine;

public class DeviceCapabilities : MonoBehaviour
{
    public enum Sensors
    {
        None,
        Accelerometer,
        //Gyroscope,
        AccelerometerCompass,
        GyroscopeCompass
    }
    public Sensors SensorsSupport;

    public enum Compass
    {
        None,
        AlphaHeading,
        WebKit
    }
    public Compass CompassSupport;

    [System.Flags]
    public enum Location
    {
        None = 0,
        LatitudeLongitude = 1 << 0,
        Altitude = 1 << 1,
        Accuracy = 1 << 2,
    }
    public Location LocationSupport;

    public enum Camera
    {
        None,
        BackCamera,
        FrontCamera
    }
    public Camera CameraSupport;

    //public void 
}