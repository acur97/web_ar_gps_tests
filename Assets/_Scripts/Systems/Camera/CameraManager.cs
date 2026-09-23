using Cysharp.Threading.Tasks;
#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.LowLevel;

public class CameraManager : MonoBehaviour
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern float JS_WebCamVideo_GetFrameRate(int deviceId);
#endif

    [SerializeField] private RawImage rawImage;
    [SerializeField] private AspectRatioFitter aspectFitter;

    private WebCamDevice[] devices;
    private WebCamDevice cameraDevice;
    private int cameraIndex = -1;

    [Header("Runtime")]
    [SerializeField] private WebCamTexture cameraTexture;

    private bool cameraSet;

    private void Awake()
    {
        PlayerLoopSystem loop = PlayerLoop.GetCurrentPlayerLoop();
        PlayerLoopHelper.Initialize(ref loop, InjectPlayerLoopTimings.Minimum);
    }

    public void StartCamera()
    {
        StartAwaitCamera().Forget();
    }

    public async UniTaskVoid StartAwaitCamera()
    {
        rawImage.enabled = true;
        Debug.Log("RequestUserAuthorization");
        await Application.RequestUserAuthorization(UserAuthorization.WebCam);

        if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            Debug.LogWarning("Authorization error");
            return;
        }

        if (FPSCounter.fps < 55)
        {
            await UniTask.WaitForSeconds(1);
        }
        else if (FPSCounter.fps <= 45)
        {
            Debug.Log("2 second delay");
            await UniTask.WaitForSeconds(2); // delay for slow devices
        }
        else if (FPSCounter.fps <= 30)
        {
            Debug.Log("4 second delay");
            await UniTask.WaitForSeconds(4); // delay for slow devices
        }

        await UniTask.WaitUntil(() => WebCamTexture.devices.Length > 0);
        devices = WebCamTexture.devices;

        //Debug.Log("devices:");
        //string desc;
        //foreach (WebCamDevice device in devices)
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

        cameraIndex = devices.Length - 1;
        cameraDevice = devices[cameraIndex];

        if (cameraDevice.isFrontFacing && !devices[0].isFrontFacing) // fix for inverse order of camera array
        {
            cameraIndex = 0;
            cameraDevice = devices[0];
        }

        if (cameraDevice.isFrontFacing)
        {
            Debug.LogWarning($"Camera ´{cameraDevice.name}´ is Front Facing!");
        }

        Debug.Log($"cameraDevice: {cameraDevice.name}"); // camera 0, facing back
        cameraTexture = new WebCamTexture(cameraDevice.name); // mejor sin aumentar resolucion, fps no se envian a webGl

        rawImage.texture = cameraTexture;
        cameraTexture.Play();
        cameraSet = false;
    }

    public void StopCameras()
    {
        rawImage.enabled = false;
        rawImage.texture = null;
        cameraSet = false;

        if (cameraTexture != null)
        {
            cameraTexture.Stop();
            Destroy(cameraTexture);
            cameraDevice = default;
        }
    }

    private void Update()
    {
        if (cameraSet || cameraTexture == null)
            return;

        // Skip making adjustment for incorrect camera data
        if (cameraTexture.width < 100)
        {
            Debug.LogWarning($"Still waiting another frame for correct info. width:{cameraTexture.width}");
            return;
            // si dura muchisimo tiempo aqui, hay que intentar de nuevo habilitar la camara, con mas delay de espera
        }

#if UNITY_WEBGL && !UNITY_EDITOR
        float cameraFPS = JS_WebCamVideo_GetFrameRate(cameraIndex);
        Debug.Log($"Fps: {cameraFPS}");
#endif

        Debug.Log($"graphicsFormat:{cameraTexture.graphicsFormat} isReadable:{cameraTexture.isReadable} videoRotationAngle:{cameraTexture.videoRotationAngle} videoVerticallyMirrored:{cameraTexture.videoVerticallyMirrored}");
        Debug.Log($"currentResolution:{cameraTexture.width}x{cameraTexture.height} | UpdateThisFrame:{cameraTexture.didUpdateThisFrame} | isPlaying:{cameraTexture.isPlaying}");
        // currentResolution:480x640 | UpdateThisFrame:True | isPlaying:True
        // currentResolution:480x640 | UpdateThisFrame:True | isPlaying:false

        // Set AspectRatioFitter's ratio
        aspectFitter.aspectRatio = cameraTexture.width / (float)cameraTexture.height;

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