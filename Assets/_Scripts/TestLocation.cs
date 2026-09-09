using TMPro;
using UnityEngine;

public class TestLocation : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    private void Awake()
    {
        text.SetText("Uninitialized.");
    }

    public void IsEnabledByUser()
    {
        text.SetText($"SupportsLocation:{SystemInfo.supportsLocationService} | hasBeenEnabledByUser:{Input.location.isEnabledByUser}");
    }

    public void LocationStart()
    {
        PreciseLocation.Install();

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Input.location.Stop();
        }

        Input.location.Start(1, 0);
        text.SetText("Initializing.");
    }

    private void Update()
    {
        switch (Input.location.status)
        {
            //case LocationServiceStatus.Initializing:
            //    text.SetText("Initializing.");
            //    break;
            case LocationServiceStatus.Running:
                text.SetText("Running.");

                text.text += $"\nLocation: {Input.location.lastData.latitude} | {Input.location.lastData.longitude} | {Input.location.lastData.altitude} | {Input.location.lastData.horizontalAccuracy}" /*{Input.location.lastData.verticalAccuracy}"*/;
                //                                                                                                                                                            WebGL usa estos dos valores por igual
                text.text += $"\nPrecise: {PreciseLocation.Latitude} | {PreciseLocation.Longitude}";
                // Android tiene 7-9 numeros de precision, iOS y Pc tiene 15 de precision, una locura de diferencia
                break;
            //case LocationServiceStatus.Stopped:
            //    text.SetText("Stopped.");
            //    break;
            case LocationServiceStatus.Failed:
                text.SetText("Failed.");
                break;
            default:
                break;
        }
    }

    public void LocationStop()
    {
        if (Input.location.status == LocationServiceStatus.Running)
        {
            text.text += "\nStopped.";
        }
        Input.location.Stop();
    }
}