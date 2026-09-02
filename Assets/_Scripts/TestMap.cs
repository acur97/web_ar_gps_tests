using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TestMap : MonoBehaviour
{
    private const string MapsStaticAPIKey = "AIzaSyDrLcl9TazRnbQe3QHLaDewmUUkd9B7K8w";
    //private const string url = "https://maps.googleapis.com/maps/api/staticmap?center={0},{1}&zoom={2}&size=640x640&scale=2&key={3}";
    private const string url = "https://maps.googleapis.com/maps/api/staticmap?center={0},{1}&zoom={2}&size=640x640&key={3}";

    [SerializeField] private RawImage rawImage;
    [SerializeField] private TextMeshProUGUI text;

    public void DownloadMap()
    {
        text.SetText("Iniciando descarga...");

        DownloadImage().Forget();
    }

    private async UniTaskVoid DownloadImage()
    {
        using UnityWebRequest request = UnityWebRequestTexture.GetTexture(string.Format(url, PreciseLocation.Latitude, PreciseLocation.Longitude, 15, MapsStaticAPIKey));

        text.SetText("Descargando...");
        await request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            text.SetText(request.error);
            Debug.Log(request.error);
        }
        else
        {
            text.SetText("Listo.");
            rawImage.texture = DownloadHandlerTexture.GetContent(request);
        }
    }
}