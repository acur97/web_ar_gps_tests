using Cysharp.Threading.Tasks;
using System.Linq;
//using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class TestCamera : MonoBehaviour
{
    [SerializeField] private RawImage rImage;
    [SerializeField] private AspectRatioFitter aspectFitter;

    [Space]
    //[SerializeField] private WebCamTexture webCamTexture;
    //[SerializeField] private int cameraIndex = -1;
    //private WebCamDevice[] devices;

    private WebCamDevice frontCameraDevice;
    private WebCamDevice backCameraDevice;
    private WebCamDevice activeCameraDevice;

    [SerializeField] private WebCamTexture frontCameraTexture;
    [SerializeField] private WebCamTexture backCameraTexture;
    [SerializeField] private WebCamTexture activeCameraTexture;

    private CancellationTokenSource token;

    //public void CameraIndex(string index)
    //{
    //    if (!string.IsNullOrEmpty(index))
    //        cameraIndex = int.Parse(index);
    //}

    public void StartCamera()
    {
        StartAwaitCamera().Forget();
    }

    //private string DescribeResolution(Resolution res)
    //{
    //    return $"{res.width}x{res.height}@{res.refreshRateRatio.value}Hz";
    //}

    //private string DescribeResolutions(WebCamDevice dev)
    //{
    //    return string.Join(", ", dev.availableResolutions.Select(res => DescribeResolution(res)));
    //}

    public async UniTaskVoid StartAwaitCamera()
    {
        Debug.Log("StartAwaitCamera");
        token?.Cancel();
        token = new CancellationTokenSource();

        rImage.enabled = true;
        Debug.Log("RequestUserAuthorization");
        await Application.RequestUserAuthorization(UserAuthorization.WebCam);

        await UniTask.WaitForSeconds(1, cancellationToken: token.Token);

        Debug.Log("WaitUntil WebCamTexture.devices");
        await UniTask.WaitUntil(() => WebCamTexture.devices.Length > 0, cancellationToken: token.Token);
        await UniTask.NextFrame(token.Token);

        await UniTask.WaitForSeconds(1, cancellationToken: token.Token);

        //if (webCamTexture != null)
        //    webCamTexture.Stop();

        // Get the device's cameras and create WebCamTextures with them
        frontCameraDevice = WebCamTexture.devices.First();
        backCameraDevice = WebCamTexture.devices.Last();

        Debug.Log($"frontCameraDevice: {frontCameraDevice.name}");
        Debug.Log($"backCameraDevice: {backCameraDevice.name}");
        frontCameraTexture = new WebCamTexture(frontCameraDevice.name);
        backCameraTexture = new WebCamTexture(backCameraDevice.name);

        SetActiveCamera(backCameraTexture);

        //webCamTexture = new WebCamTexture();

        //devices = WebCamTexture.devices;

        //foreach (WebCamDevice device in devices)
        //{
        //    string desc = $"name: {device.name}. type: {device.kind}. ";

        //    if (device.depthCameraName != null)
        //        desc += $"Depth support: ({device.depthCameraName}). ";

        //    desc += $"Direction: {(device.isFrontFacing ? "Front" : "Rear")}. ";

        //    if (device.isAutoFocusPointSupported)
        //        desc += "Auto focus support. ";

        //    if (device.availableResolutions != null)
        //        desc += $"Supported resolutions: {DescribeResolutions(device)}. ";

        //    Debug.LogWarning(desc);
        //}

        //webCamTexture.deviceName = devices[cameraIndex].name;
        //Debug.Log("Play");
        //webCamTexture.Play();

        //await UniTask.WaitUntil(() => webCamTexture.width > 0);
        //await UniTask.NextFrame(token.Token);

        //Debug.Log($"resolution: {webCamTexture.width}x{webCamTexture.height}."); // 16x16
        //Debug.Log($"resolution2: {webCamTexture.requestedWidth}x{webCamTexture.requestedHeight} {webCamTexture.requestedFPS}fps."); // 0x0 0fps.

        //aspectFitter.aspectRatio = webCamTexture.width / webCamTexture.height;
        //rImage.texture = webCamTexture;

        //Debug.Log(webCamTexture.deviceName); // camera 1, facing front

        //bool vflip = webCamTexture.videoVerticallyMirrored;
        //Vector2 scale = new(1, vflip ? -1 : 1);
        //Vector2 offset = new(0, vflip ? 1 : 0);
    }

    private void SetActiveCamera(WebCamTexture cameraToUse)
    {
        Debug.Log($"SetActiveCamera {cameraToUse}");

        if (activeCameraTexture != null)
            activeCameraTexture.Stop();

        activeCameraTexture = cameraToUse;
        //activeCameraDevice = WebCamTexture.devices.FirstOrDefault(device =>
        //    device.name == cameraToUse.deviceName);
        activeCameraDevice = cameraToUse.Equals(frontCameraTexture) ? frontCameraDevice : backCameraDevice;

        rImage.texture = activeCameraTexture;

        activeCameraTexture.Play();
    }

    public void SwitchCamera()
    {
        SetActiveCamera(activeCameraTexture.Equals(frontCameraTexture) ? backCameraTexture : frontCameraTexture);
    }

    public void StopCameras()
    {
        token?.Cancel();
        rImage.enabled = false;
        rImage.texture = null;

        if (activeCameraTexture != null)
        {
            activeCameraTexture.Stop();
            Destroy(activeCameraTexture);
            activeCameraDevice = default;
        }
        if (frontCameraTexture != null)
        {
            frontCameraTexture.Stop();
            Destroy(frontCameraTexture);
            frontCameraDevice = default;
        }
        if (backCameraTexture != null)
        {
            backCameraTexture.Stop();
            Destroy(backCameraTexture);
            backCameraDevice = default;
        }
    }

    private void Update()
    {
        if (activeCameraTexture == null)
            return;

        // Skip making adjustment for incorrect camera data
        if (activeCameraTexture.width < 100)
        {
            Debug.LogWarning("Still waiting another frame for correct info...");
            return;
        }

        // Rotate image to show correct orientation 
        //rotationVector.z = -activeCameraTexture.videoRotationAngle;
        //image.rectTransform.localEulerAngles = rotationVector;

        Debug.LogWarning(
            $"resolution: {activeCameraTexture.width}x{activeCameraTexture.height} {activeCameraTexture.didUpdateThisFrame} {activeCameraTexture.isPlaying} {activeCameraTexture.texelSize}."); // 480x640

        // Set AspectRatioFitter's ratio
        float videoRatio = activeCameraTexture.width / (float)activeCameraTexture.height;
        aspectFitter.aspectRatio = videoRatio;

        // Unflip if vertically flipped
        //image.uvRect =
        //    activeCameraTexture.videoVerticallyMirrored ? fixedRect : defaultRect;

        // Mirror front-facing camera's image horizontally to look more natural
        //imageParent.localScale =
        //    activeCameraDevice.isFrontFacing ? fixedScale : defaultScale;
    }
}