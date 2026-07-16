using TMPro;
using UnityEngine;

public class TestAccelerometer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    private void Update()
    {
        text.text = Input.acceleration.ToString();
    }
}