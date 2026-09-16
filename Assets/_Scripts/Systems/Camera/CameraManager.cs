using Cysharp.Threading.Tasks;
#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern float JS_WebCamVideo_GetFrameRate(int deviceId);
#endif

    [SerializeField] private RawImage rawImage;
    [SerializeField] private AspectRatioFitter aspectFitter;

    private WebCamDevice backCameraDevice;
    private int backCameraIndex = -1;

    [Header("Runtime")]
    [SerializeField] private WebCamTexture backCameraTexture;

    private CancellationTokenSource token;
    private bool cameraSet;

    private void Awake()
    {
        ResetSettings();
    }

    public void StartCamera()
    {
        StartAwaitCamera().Forget();
    }

    public async UniTaskVoid StartAwaitCamera()
    {
        token?.Cancel();
        token = new CancellationTokenSource();

        await UniTask.SwitchToMainThread(token.Token);

        rawImage.enabled = true;
        Debug.Log("RequestUserAuthorization");
        await Application.RequestUserAuthorization(UserAuthorization.WebCam);

        if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            Debug.LogWarning("Authorization error");
            return;
        }

        await UniTask.WaitUntil(() => WebCamTexture.devices.Length > 0, cancellationToken: token.Token);

        await UniTask.WaitForSeconds(2); // slow phones fix

        //Debug.Log("devices:");
        //string desc;
        //foreach (WebCamDevice device in WebCamTexture.devices)
        //{
        //    desc = $"Name: {device.name}. Type: {device.kind}. "; // name: camera 1, facing frony. type: WideAngle.

        //    desc += $"Direction: {(device.isFrontFacing ? "Front" : "Rear")}."; // Direction: Front.

        //    desc += $"\nAvailableResolutions:{device.availableResolutions.Length}";

        //    foreach (Resolution resoluton in device.availableResolutions)
        //    {
        //        desc += $"\nResolution:{resoluton.width}x{resoluton.height} {resoluton.refreshRateRatio}Hz";
        //    }

        //    Debug.LogWarning(desc);
        //}

        backCameraIndex = WebCamTexture.devices.Length - 1;
        backCameraDevice = WebCamTexture.devices[backCameraIndex];

        if (backCameraDevice.isFrontFacing && !WebCamTexture.devices[0].isFrontFacing)
        {
            backCameraIndex = 0;
            backCameraDevice = WebCamTexture.devices[0];
        }

        Debug.Log($"backCameraDevice: {backCameraDevice.name}"); // camera 0, facing back
        backCameraTexture = new WebCamTexture(backCameraDevice.name); // mejor sin aumentar resolucion, fps no se envian a webGl

        rawImage.texture = backCameraTexture;
        backCameraTexture.Play();
        ResetSettings();
    }

    private void ResetSettings()
    {
        cameraSet = false;
        Application.targetFrameRate = -1;
    }

    public void StopCameras()
    {
        token?.Cancel();
        rawImage.enabled = false;
        rawImage.texture = null;
        ResetSettings();

        if (backCameraTexture != null)
        {
            backCameraTexture.Stop();
            Destroy(backCameraTexture);
            backCameraDevice = default;
        }
    }

    private void Update()
    {
        if (cameraSet || backCameraTexture == null)
            return;

        // Skip making adjustment for incorrect camera data
        if (backCameraTexture.width < 100)
        {
            Debug.LogWarning($"Still waiting another frame for correct info. width:{backCameraTexture.width}");
            return;
        }

#if UNITY_WEBGL && !UNITY_EDITOR
        float cameraFPS = JS_WebCamVideo_GetFrameRate(backCameraIndex);
        Debug.Log(cameraFPS);

        //Application.targetFrameRate = Mathf.RoundToInt(cameraFPS);
#endif

        //Debug.LogWarning($"graphicsFormat:{backCameraTexture.graphicsFormat} isReadable:{backCameraTexture.isReadable} videoRotationAngle:{backCameraTexture.videoRotationAngle} videoVerticallyMirrored:{backCameraTexture.videoVerticallyMirrored}");

        Debug.LogWarning($"currentResolution:{backCameraTexture.width}x{backCameraTexture.height} | UpdateThisFrame:{backCameraTexture.didUpdateThisFrame} | isPlaying:{backCameraTexture.isPlaying}");
        // currentResolution:480x640 | UpdateThisFrame:True | isPlaying:True
        // currentResolution:480x640 | UpdateThisFrame:True | isPlaying:false

        // Set AspectRatioFitter's ratio
        aspectFitter.aspectRatio = backCameraTexture.width / (float)backCameraTexture.height;

        cameraSet = true;

        //bool vflip = webCamTexture.videoVerticallyMirrored;
        //Vector2 scale = new(1, vflip ? -1 : 1);
        //Vector2 offset = new(0, vflip ? 1 : 0);

        // Rotate image to show correct orientation 
        //rotationVector.z = -activeCameraTexture.videoRotationAngle;
        //image.rectTransform.localEulerAngles = rotationVector;

        // Unflip if vertically flipped
        //image.uvRect =
        //    activeCameraTexture.videoVerticallyMirrored ? fixedRect : defaultRect;

        // Mirror front-facing camera's image horizontally to look more natural
        //imageParent.localScale =
        //    activeCameraDevice.isFrontFacing ? fixedScale : defaultScale;
    }
}