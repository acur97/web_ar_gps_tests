using TMPro;
using UnityEngine;

public class LocationManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    private LocationServiceStatus lastStatus;

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
        text.SetText("PreciseLocation");
        PreciseLocation.Install();

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            text.SetText("Stop");
            Input.location.Stop();
        }

        text.SetText("Start");
        Input.location.Start(1, 0);
    }

    private void Update()
    {
        if (Input.location.status == LocationServiceStatus.Running)
        {
            text.SetText($"Location: {Input.location.lastData.latitude}° | {Input.location.lastData.longitude}° | {Input.location.lastData.altitude}m | {Input.location.lastData.horizontalAccuracy}m" +
                $"\nPrecise: {PreciseLocation.Latitude}° | {PreciseLocation.Longitude}°");
            // Android tiene 7-9 numeros de precision, iOS tiene 15
            // la ubicacion aproximada da Doubles muy grandes, pero altitude 0 y accuracy 2000+
            // horizontalAccuracy de 200+ viene de pc´s
        }

        if (lastStatus != Input.location.status)
        {
            lastStatus = Input.location.status;
            ChangedStatus();
        }
    }

    private void ChangedStatus()
    {
        switch (lastStatus)
        {
            case LocationServiceStatus.Stopped:
                text.text += "\nStopped.";
                break;
            case LocationServiceStatus.Initializing:
                text.SetText("Initializing.");
                break;
            //case LocationServiceStatus.Running:
            //    break;
            case LocationServiceStatus.Failed:
                text.text += "\nFailed.";
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