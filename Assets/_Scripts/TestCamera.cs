using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class TestCamera : MonoBehaviour
{
    [SerializeField] private RawImage rImage;
    [SerializeField] private AspectRatioFitter aspectFitter;

    [Space]
    [SerializeField] private WebCamTexture _webcam;

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
        await Application.RequestUserAuthorization(UserAuthorization.WebCam);

        //_webcam = new WebCamTexture("OBS Virtual Camera"); // con el string se cambia entre diferentes camaras, vacio es la primera
        _webcam = new WebCamTexture();

        foreach (WebCamDevice device in WebCamTexture.devices)
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

        _webcam.Play();

        while (_webcam.width < 32)
            await UniTask.NextFrame();

        Debug.Log($"resolution: {_webcam.width}x{_webcam.height} {_webcam.requestedFPS}fps.");

        aspectFitter.aspectRatio = _webcam.width / _webcam.height;
        rImage.texture = _webcam;
        rImage.enabled = true;

        //bool vflip = _webcam.videoVerticallyMirrored;
        //Vector2 scale = new(1, vflip ? -1 : 1);
        //Vector2 offset = new(0, vflip ? 1 : 0);
    }
}