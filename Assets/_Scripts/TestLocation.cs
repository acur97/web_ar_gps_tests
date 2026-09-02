using TMPro;
using UnityEngine;

public class TestLocation : MonoBehaviour
{
    public float latitude;
    public float longitude;

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
        //Input.compass.enabled = true;
    }

    public void CheckLocationStart()
    {
        text.SetText(_locationService.status.ToString());
    }

    public void CheckLocation()
    {
        if (_locationService.status == LocationServiceStatus.Running)
        {
            latitude = _locationService.lastData.latitude;
            longitude = _locationService.lastData.longitude;

            text.SetText($"Location: {latitude} {longitude} {_locationService.lastData.altitude} {_locationService.lastData.horizontalAccuracy} {_locationService.lastData.verticalAccuracy}");
        }
    }

    public void LocationStop()
    {
        _locationService.Stop();
    }
}