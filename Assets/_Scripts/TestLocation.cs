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
            Input.location.Stop();

        Input.location.Start(1, 0);
        text.SetText("Initializing.");
    }

    private void Update()
    {
        switch (Input.location.status)
        {
            case LocationServiceStatus.Running:
                text.SetText($"\nLocation: {Input.location.lastData.latitude}° | {Input.location.lastData.longitude}° | {Input.location.lastData.altitude}m | {Input.location.lastData.horizontalAccuracy}m" +
                    $"\nPrecise: {PreciseLocation.Latitude}° | {PreciseLocation.Longitude}°");
                // Android tiene 7-9 numeros de precision, iOS y Pc tiene 15 de precision
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
        if (Input.location.status == LocationServiceStatus.Running)
            text.text += "\nStopped.";

        Input.location.Stop();
    }
}