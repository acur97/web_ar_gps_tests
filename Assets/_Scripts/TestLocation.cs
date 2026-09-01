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
        float desiredAccuracyInMeters = 10f;
        float updateDistanceInMeters = 10f;

        Input.location.Start(desiredAccuracyInMeters, updateDistanceInMeters);
    }

    public void CheckLocationStart()
    {
        text.SetText(Input.location.status.ToString());
    }

    public void CheckLocation()
    {
        if (Input.location.status == LocationServiceStatus.Running)
        {
            text.SetText($"Location: {Input.location.lastData.latitude} {Input.location.lastData.longitude} {Input.location.lastData.altitude} {Input.location.lastData.horizontalAccuracy} {Input.location.lastData.timestamp}");
        }
    }

    public void LocationStop()
    {
        Input.location.Stop();
    }
}