using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    private const string format = "{1:0.} fps\n{0:0.0} ms";

    [SerializeField] private TextMeshProUGUI textField;

    private float _deltaTime = 0.0f;
    private float msec = 0.0f;
    private float fps = 0.0f;

    private void Update()
    {
        _deltaTime += (Time.deltaTime - _deltaTime) * 0.1f;
        msec = _deltaTime * 1000.0f;
        fps = 1.0f / _deltaTime;
        textField.SetText(format, msec, fps);
    }
}