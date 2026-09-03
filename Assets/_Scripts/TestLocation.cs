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
        float updateDistanceInMeters = 0f;

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
            text.text = $"Location: {Input.location.lastData.latitude} {Input.location.lastData.longitude} {Input.location.lastData.altitude} {Input.location.lastData.horizontalAccuracy} {Input.location.lastData.verticalAccuracy}";
            //                                                                                                                                                      WebGL usa estos dos valores por igual
            text.text += $"\nPreciseLocation: {PreciseLocation.Latitude} {PreciseLocation.Longitude} {PreciseLocation.Accuracy}";
            // Android tiene como 7 numeros de precision, iphone tiene como 15 de precision, una locura
        }
    }

    public void LocationStop()
    {
        Input.location.Stop();
    }
}