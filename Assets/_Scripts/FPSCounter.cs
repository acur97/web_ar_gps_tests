using System.Text;
using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textField;
    [SerializeField] private float updateInterval = 0.5F;

    private float accum = 0;
    private int frames = 0;
    private float deltaTime;
    private float timeleft;
    private int fps;

    private readonly StringBuilder stringBuilder = new(3);

    private void Awake()
    {
        timeleft = updateInterval;
    }

    private void Update()
    {
        deltaTime = Time.deltaTime;
        timeleft -= deltaTime;
        accum += Time.timeScale / deltaTime;
        ++frames;

        if (timeleft <= 0.0)
        {
            stringBuilder.Remove(0, stringBuilder.Length);

            fps = (int)accum / frames;

            stringBuilder.Append(fps);
            textField.text = stringBuilder.ToString();

            timeleft = updateInterval;
            accum = 0.0F;
            frames = 0;
        }
    }
}