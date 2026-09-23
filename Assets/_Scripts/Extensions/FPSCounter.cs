using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    private const string format = "{1:0.} fps\n{0:0.0} ms";

    [SerializeField] private TextMeshProUGUI textField;

    private float _deltaTime = 0.01666f;
    private float msec = 16.66f;
    public float fps = 60f;

    private void Update()
    {
        _deltaTime += (Time.deltaTime - _deltaTime) * 0.1f;
        msec = _deltaTime * 1000f;
        fps = 1f / _deltaTime;
        textField.SetText(format, msec, fps);
    }
}