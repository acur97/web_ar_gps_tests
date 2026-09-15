using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private RawImage rawImage;
    [SerializeField] private AspectRatioFitter aspectFitter;

    private WebCamDevice backCameraDevice;

    [Header("Runtime")]
    [SerializeField] private WebCamTexture backCameraTexture;

    private CancellationTokenSource token;
    private bool cameraSet = false;

    public void StartCamera()
    {
        StartAwaitCamera().Forget();
    }

    public async UniTaskVoid StartAwaitCamera()
    {
        token?.Cancel();
        token = new CancellationTokenSource();

        rawImage.enabled = true;
        Debug.Log("RequestUserAuthorization");
        await Application.RequestUserAuthorization(UserAuthorization.WebCam);

        if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            Debug.LogWarning("Authorization error");
            return;
        }

        await UniTask.WaitUntil(() => WebCamTexture.devices.Length > 0, cancellationToken: token.Token);

        //Debug.Log("devices:");
        //string desc;
        //foreach (WebCamDevice device in WebCamTexture.devices)
        //{
        //    desc = $"name: {device.name}. type: {device.kind}. "; // name: camera 1, facing frony. type: WideAngle.

        //    desc += $"Direction: {(device.isFrontFacing ? "Front" : "Rear")}. "; // Direction: Front.

        //    Debug.LogWarning(desc);
        //}

        backCameraDevice = WebCamTexture.devices[^1];

        if (backCameraDevice.isFrontFacing && !WebCamTexture.devices[0].isFrontFacing)
        {
            backCameraDevice = WebCamTexture.devices[0];
        }

        Debug.Log($"backCameraDevice: {backCameraDevice.name}"); // camera 0, facing back
        backCameraTexture = new WebCamTexture(backCameraDevice.name); // mejor sin aumentar resolucion, fps no se envian a webGl

        rawImage.texture = backCameraTexture;
        backCameraTexture.Play();
        cameraSet = false;
    }

    public void StopCameras()
    {
        token?.Cancel();
        rawImage.enabled = false;
        rawImage.texture = null;
        cameraSet = false;

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