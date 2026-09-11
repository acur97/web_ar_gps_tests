using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TestMap : MonoBehaviour
{
    private const string MapsStaticAPIKey = "AIzaSyDrLcl9TazRnbQe3QHLaDewmUUkd9B7K8w";
    private const string url = "https://maps.googleapis.com/maps/api/staticmap?center={0},{1}&zoom={2}" +
        "&size=640x640" +
        //"&scale=2" +
        "&style=feature:administrative|visibility:off" +
        "&style=feature:poi|visibility:off" +
        "&style=feature:transit|visibility:off" +
        "&key={3}";

    [SerializeField] private RawImage rawImage;
    [SerializeField] private RectTransform rawImageTransform;
    [SerializeField] private RectTransform circleAccuracy;
    [SerializeField] private float mapZoom;
    [SerializeField] private Vector2 displacementMulti;
    [SerializeField] private float circleZoom;
    private float circleScale;
    [SerializeField] private TextMeshProUGUI text;

    private bool mapDownloaded = false;

    [SerializeField] private double lastLatitude;
    [SerializeField] private double lastLongitude;
    private Vector2 lastDifference = Vector2.zero;
    private Vector2 targetDifference = Vector2.zero;

    public void DownloadMap()
    {
        text.SetText("Iniciando descarga de mapa...");

        DownloadImage().Forget();
    }

    private async UniTaskVoid DownloadImage()
    {
        mapDownloaded = false;

        if (PreciseLocation.Latitude != 0 && PreciseLocation.Longitude != 0)
        {
            lastLatitude = PreciseLocation.Latitude;
            lastLongitude = PreciseLocation.Longitude;
        }

        using UnityWebRequest request = UnityWebRequestTexture.GetTexture(
            string.Format(url, lastLatitude, lastLongitude, mapZoom, MapsStaticAPIKey));

        text.SetText("Descargando mapa...");
        await request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            text.SetText(request.error);
            Debug.Log(request.error);
        }
        else
        {
            text.SetText("Mapa listo.");

            UpdateDifference();
            lastDifference = targetDifference;

            rawImage.texture = DownloadHandlerTexture.GetContent(request);

            mapDownloaded = true;
        }
    }

    private void UpdateDifference()
    {
        targetDifference.x = (float)((lastLongitude - PreciseLocation.Longitude) * displacementMulti.x);
        targetDifference.y = (float)((lastLatitude - PreciseLocation.Latitude) * displacementMulti.y);
    }

    public void UpdateDisplacementX(string txt)
    {
        displacementMulti.x = float.Parse(txt);
    }

    public void UpdateDisplacementY(string txt)
    {
        displacementMulti.y = float.Parse(txt);
    }

    private void Update()
    {
        if (mapDownloaded && Input.location.status == LocationServiceStatus.Running)
        {
            circleScale = Input.location.lastData.horizontalAccuracy * circleZoom;
            circleAccuracy.sizeDelta = new Vector2(circleScale, circleScale);

            UpdateDifference();

            lastDifference = Vector2.Lerp(lastDifference, targetDifference, 1f - Mathf.Exp(-5.25f * Time.deltaTime));

            text.SetText($"CenterMapDifference: {lastDifference}");

            rawImageTransform.anchoredPosition = lastDifference;
        }
    }
}