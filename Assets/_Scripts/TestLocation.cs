using TMPro;
using UnityEngine;

public class TestLocation : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    public void IsEnabledByUser()
    {
        text.SetText($"Supports Location:{SystemInfo.supportsLocationService} isEnabledByUser:{Input.location.isEnabledByUser}");
    }

    public void LocationStart()
    {
        float desiredAccuracyInMeters = 1f;
        float updateDistanceInMeters = 0f;

        PreciseLocation.Install();
        Input.location.Start(desiredAccuracyInMeters, updateDistanceInMeters);
        //Input.compass.enabled = true;
    }

    private void Update()
    {
        switch (Input.location.status)
        {
            case LocationServiceStatus.Initializing:
                text.SetText("Initializing.");
                break;
            case LocationServiceStatus.Running:
                text.SetText("Running.");

                text.text += $"\nLocation: {Input.location.lastData.latitude} | {Input.location.lastData.longitude} | {Input.location.lastData.altitude} | {Input.location.lastData.horizontalAccuracy}" /*{Input.location.lastData.verticalAccuracy}"*/;
                //                                                                                                                                                      WebGL usa estos dos valores por igual
                text.text += $"\nPrecise: {PreciseLocation.Latitude} | {PreciseLocation.Longitude}";
                // Android tiene 8-9 numeros de precision, iphone tiene 15 de precision, una locura
                break;
            case LocationServiceStatus.Stopped:
                text.SetText("Stopped.");
                break;
            case LocationServiceStatus.Failed:
                text.SetText("Failed.");
                break;
            default:
                break;
        }
    }

    public void LocationStop()
    {
        Input.location.Stop();
    }
}