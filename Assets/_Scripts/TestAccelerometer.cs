using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Gyroscope = UnityEngine.InputSystem.Gyroscope;

public class TestAccelerometer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    private bool enabledSensors = false;

    public void OnStart()
    {
        Debug.Log("Start Sensors...");

        Debug.Log($"Accelerometer - {Accelerometer.current}");
        if (Accelerometer.current != null)
        {
            InputSystem.EnableDevice(Accelerometer.current);
            Debug.Log("Enabled Accelerometer");
        }

        Debug.Log($"AttitudeSensor - {AttitudeSensor.current}");
        if (AttitudeSensor.current != null)
        {
            InputSystem.EnableDevice(AttitudeSensor.current);
            Debug.Log("Enabled AttitudeSensor");
        }

        Debug.Log($"GravitySensor - {GravitySensor.current}");
        if (GravitySensor.current != null)
        {
            InputSystem.EnableDevice(GravitySensor.current);
            Debug.Log("Enabled GravitySensor");
        }

        Debug.Log($"Gyroscope - {Gyroscope.current}");
        if (Gyroscope.current != null)
        {
            InputSystem.EnableDevice(Gyroscope.current);
            Debug.Log("Enabled Gyroscope");
        }

        Debug.Log($"LinearAccelerationSensor - {LinearAccelerationSensor.current}");
        if (LinearAccelerationSensor.current != null)
        {
            InputSystem.EnableDevice(LinearAccelerationSensor.current);
            Debug.Log("Enabled LinearAccelerationSensor");
        }

        Debug.Log($"MagneticFieldSensor - {MagneticFieldSensor.current}");
        if (MagneticFieldSensor.current != null)
        {
            InputSystem.EnableDevice(MagneticFieldSensor.current);
            Debug.Log("Enabled MagneticFieldSensor");
        }

        Debug.Log("Finish Sensors.");
        enabledSensors = true;
    }

    private void Update()
    {
        if (!enabledSensors)
            return;

        //if (Accelerometer.current != null)
        //{
        //    //gameObject.transform.rotation = AttitudeSensor.current.attitude.ReadValue();
        //    text.text = Accelerometer.current.acceleration.ReadValue().ToString();
        //}
        //else
        //{
        //    text.text = Accelerometer.current.ToString();
        //}

        //else gameObject.transform.rotation = Quaternion.Euler(0, (float)System.DateTimeOffset.Now.TimeOfDay.Milliseconds * 360 / 1000f, 0);

        text.text = "";

        if (Accelerometer.current != null)
        {
            text.text += $"Accelerometer: {Accelerometer.current.acceleration.ReadValue()} \n";
        }

        if (AttitudeSensor.current != null)
        {
            text.text += $"AttitudeSensor: {AttitudeSensor.current.attitude.ReadValue()} \n";
        }

        if (GravitySensor.current != null)
        {
            text.text += $"GravitySensor: {GravitySensor.current.gravity.ReadValue()} \n";
        }

        if (Gyroscope.current != null)
        {
            text.text += $"Gyroscope: {Gyroscope.current.angularVelocity.ReadValue()} \n";
        }

        if (LinearAccelerationSensor.current != null)
        {
            text.text += $"LinearAccelerationSensor: {LinearAccelerationSensor.current.acceleration.ReadValue()} \n";
        }

        if (MagneticFieldSensor.current != null)
        {
            text.text += $"MagneticFieldSensor: {MagneticFieldSensor.current.magneticField.ReadValue()}";
        }
    }
}