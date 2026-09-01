using TMPro;
using UnityEngine;

public class TestLocation : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    private LocationService _locationService;

    private void Awake()
    {
        _locationService = Input.location;
    }

    public void IsEnabledByUser()
    {
        Debug.Log($"Supports Location: {SystemInfo.supportsLocationService}");
        text.SetText(_locationService.isEnabledByUser.ToString());
    }

    public void LocationStart()
    {
        float desiredAccuracyInMeters = 1f;
        float updateDistanceInMeters = 0.001f;

        _locationService.Start(desiredAccuracyInMeters, updateDistanceInMeters);
    }

    public void CheckLocationStart()
    {
        text.SetText(_locationService.status.ToString());
    }

    public void CheckLocation()
    {
        if (_locationService.status == LocationServiceStatus.Running)
        {
            text.SetText($"Location: {_locationService.lastData.latitude} {_locationService.lastData.longitude} {_locationService.lastData.altitude} {_locationService.lastData.horizontalAccuracy} {_locationService.lastData.verticalAccuracy}");
        }
    }

    public void LocationStop()
    {
        _locationService.Stop();
    }
}