using Cysharp.Threading.Tasks;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TestCamera : MonoBehaviour
{
    [SerializeField] private RawImage rImage;
    [SerializeField] private AspectRatioFitter aspectFitter;

    [Space]
    [SerializeField] private WebCamTexture webCamTexture;
    [SerializeField] private int cameraIndex = -1;
    private WebCamDevice[] devices;

    public void CameraIndex(string index)
    {
        cameraIndex = int.Parse(index);
    }

    public void StartCamera()
    {
        StartAwaitCamera().Forget();
    }

    private string DescribeResolution(Resolution res)
    {
        return $"{res.width}x{res.height}@{res.refreshRateRatio.value}Hz";
    }

    private string DescribeResolutions(WebCamDevice dev)
    {
        return string.Join(", ", dev.availableResolutions.Select(res => DescribeResolution(res)));
    }

    public async UniTaskVoid StartAwaitCamera()
    {
        rImage.enabled = true;
        await Application.RequestUserAuthorization(UserAuthorization.WebCam);

        webCamTexture = new WebCamTexture();

        webCamTexture.Stop();

        while (WebCamTexture.devices.Length == 0)
            //await UniTask.NextFrame();
            await Awaitable.NextFrameAsync();

            devices = WebCamTexture.devices;

        foreach (WebCamDevice device in devices)
        {
            string desc = $"name: {device.name}. type: {device.kind}. ";

            if (device.depthCameraName != null)
                desc += $"Depth support: ({device.depthCameraName}). ";

            desc += $"Direction: {(device.isFrontFacing ? "Front" : "Rear")}. ";

            if (device.isAutoFocusPointSupported)
                desc += "Auto focus support. ";

            if (device.availableResolutions != null)
                desc += $"Supported resolutions: {DescribeResolutions(device)}. ";

            Debug.LogWarning(desc);
        }

        webCamTexture.deviceName = devices[cameraIndex].name;
        Debug.Log("Play");
        webCamTexture.Play();

        while (webCamTexture.width < 32)
            await Awaitable.NextFrameAsync();

        Debug.Log($"resolution: {webCamTexture.width}x{webCamTexture.height} {webCamTexture.requestedFPS}fps.");

        aspectFitter.aspectRatio = webCamTexture.width / webCamTexture.height;
        rImage.texture = webCamTexture;

        //bool vflip = webCamTexture.videoVerticallyMirrored;
        //Vector2 scale = new(1, vflip ? -1 : 1);
        //Vector2 offset = new(0, vflip ? 1 : 0);
    }
}