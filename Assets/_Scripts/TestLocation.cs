using TMPro;
using UnityEngine;

public class TestLocation : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    public void IsEnabledByUser()
    {
        Debug.Log($"Supports Location: {SystemInfo.supportsLocationService}");
        text.SetText(Input.location.isEnabledByUser.ToString());
    }

    public void LocationStart()
    {
        float desiredAccuracyInMeters = 1f;
        float updateDistanceInMeters = 0.001f;

        PreciseLocation.Install();
        Input.location.Start(desiredAccuracyInMeters, updateDistanceInMeters);
        //Input.compass.enabled = true;
    }

    public void CheckLocationStart()
    {
        text.SetText(Input.location.status.ToString());
    }

    public void CheckLocation()
    {
        if (Input.location.status == LocationServiceStatus.Running)
        {
            text.SetText($"Location: {Input.location.lastData.latitude} {Input.location.lastData.longitude} {Input.location.lastData.altitude} {Input.location.lastData.horizontalAccuracy} {Input.location.lastData.verticalAccuracy}");
            text.text += $"\nPreciseLocation: {PreciseLocation.Latitude} {PreciseLocation.Longitude} {PreciseLocation.Altitude}";
        }
    }

    public void LocationStop()
    {
        Input.location.Stop();
    }
}