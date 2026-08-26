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

        Debug.Log("Find webcams before RequestUserAuthorization");
        foreach (WebCamDevice device in WebCamTexture.devices)
        {
            Debug.Log("Name: " + device.name);
        }

        rImage.enabled = true;
        Debug.Log("RequestUserAuthorization");
        await Application.RequestUserAuthorization(UserAuthorization.WebCam);

        Debug.Log("WaitUntil HasUserAuthorization");
        await UniTask.WaitUntil(() => Application.HasUserAuthorization(UserAuthorization.WebCam), cancellationToken: token.Token);

        Debug.Log("WaitUntil NextFrame");
        await UniTask.NextFrame(token.Token);

        //WebCamTexture.allowThreadedTextureCreation = false;

        Debug.Log("WaitUntil WebCamTexture.devices");
        await UniTask.WaitUntil(() => WebCamTexture.devices.Length > 0, cancellationToken: token.Token);
        Debug.Log("WaitUntil NextFrame");
        await UniTask.NextFrame(token.Token);

        Debug.Log("devices.First and Last");
        frontCameraDevice = WebCamTexture.devices.First();
        backCameraDevice = WebCamTexture.devices.Last();

        Debug.Log($"frontCameraDevice: {frontCameraDevice.name}");
        Debug.Log($"backCameraDevice: {backCameraDevice.name}");
        frontCameraTexture = new WebCamTexture(frontCameraDevice.name);
        backCameraTexture = new WebCamTexture(backCameraDevice.name);

        SetActiveCamera(backCameraTexture);

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

        //bool vflip = webCamTexture.videoVerticallyMirrored;
        //Vector2 scale = new(1, vflip ? -1 : 1);
        //Vector2 offset = new(0, vflip ? 1 : 0);
    }

    private void SetActiveCamera(WebCamTexture cameraToUse)
    {
        Debug.Log($"SetActiveCamera {cameraToUse.name}");

        if (activeCameraTexture != null)
            activeCameraTexture.Stop();

        activeCameraTexture = cameraToUse;
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
            $"resolution: {activeCameraTexture.width}x{activeCameraTexture.height} {activeCameraTexture.didUpdateThisFrame} {activeCameraTexture.isPlaying}."); // 480x640

        // Set AspectRatioFitter's ratio
        aspectFitter.aspectRatio = activeCameraTexture.width / (float)activeCameraTexture.height; ;

        // Unflip if vertically flipped
        //image.uvRect =
        //    activeCameraTexture.videoVerticallyMirrored ? fixedRect : defaultRect;

        // Mirror front-facing camera's image horizontally to look more natural
        //imageParent.localScale =
        //    activeCameraDevice.isFrontFacing ? fixedScale : defaultScale;
    }
}