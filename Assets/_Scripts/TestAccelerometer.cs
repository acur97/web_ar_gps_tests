using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Gyroscope = UnityEngine.InputSystem.Gyroscope;

public class TestAccelerometer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    [Space]
    [SerializeField] private Transform cube;

    private bool enabledSensors = false;

    public void OnStart()
    {
        Debug.Log("Start Sensors...");

        Debug.Log($"Supports Accelerometer: {SystemInfo.supportsAccelerometer}");
        Debug.Log($"Accelerometer - {Accelerometer.current}");
        if (Accelerometer.current != null)
        {
            InputSystem.EnableDevice(Accelerometer.current);
            Debug.Log($"Enabled {Accelerometer.current.description}");
        }

        Debug.Log($"AttitudeSensor - {AttitudeSensor.current}");
        if (AttitudeSensor.current != null)
        {
            InputSystem.EnableDevice(AttitudeSensor.current);
            Debug.Log($"Enabled {AttitudeSensor.current.description}");
        }

        Debug.Log($"GravitySensor - {GravitySensor.current}");
        if (GravitySensor.current != null)
        {
            InputSystem.EnableDevice(GravitySensor.current);
            Debug.Log($"Enabled {GravitySensor.current.description}");
        }

        Debug.Log($"Supports Gyroscope: {SystemInfo.supportsGyroscope}");
        Debug.Log($"Gyroscope - {Gyroscope.current}");
        if (Gyroscope.current != null)
        {
            InputSystem.EnableDevice(Gyroscope.current);
            Debug.Log($"Enabled {Gyroscope.current.description}");
        }

        Debug.Log($"LinearAccelerationSensor - {LinearAccelerationSensor.current}");
        if (LinearAccelerationSensor.current != null)
        {
            InputSystem.EnableDevice(LinearAccelerationSensor.current);
            Debug.Log($"Enabled {LinearAccelerationSensor.current.description}");
        }

        Debug.Log($"MagneticFieldSensor - {MagneticFieldSensor.current}");
        if (MagneticFieldSensor.current != null)
        {
            InputSystem.EnableDevice(MagneticFieldSensor.current);
            Debug.Log($"Enabled {MagneticFieldSensor.current.description}");
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

        // en pc sale error en todos
        if (Accelerometer.current != null) // funciona en los dos, orientacion, vector3
        {
            text.text += $"Accelerometer: {Accelerometer.current.acceleration.ReadValue()} \n";
        }

        if (AttitudeSensor.current != null) // solo funciona en moderno, quaternio?, 
        {
            text.text += $"AttitudeSensor: {AttitudeSensor.current.attitude.ReadValue()} \n";
        }

        if (GravitySensor.current != null) // funciona en los dos, parece igual que Accelerometer, vector3
        {
            text.text += $"GravitySensor: {GravitySensor.current.gravity.ReadValue()} \n";
        }

        if (Gyroscope.current != null) // solo funciona en moderno, cantidad de giro, vector3
        {
            text.text += $"Gyroscope: {Gyroscope.current.angularVelocity.ReadValue()} \n";
        }

        if (LinearAccelerationSensor.current != null) // solo funciona en moderno, un suavizado de no se que, parece Gyroscope, vector3
        {
            text.text += $"LinearAccelerationSensor: {LinearAccelerationSensor.current.acceleration.ReadValue()} \n";
        }

        if (MagneticFieldSensor.current != null) // error en viejo, en nuevo nada, no muestra en ninguno de los dos
        {
            text.text += $"MagneticFieldSensor: {MagneticFieldSensor.current.magneticField.ReadValue()}";
        }

        if (Accelerometer.current != null)
        {
            cube.localEulerAngles = new Vector3(
                Accelerometer.current.acceleration.ReadValue().y * 90,
                Accelerometer.current.acceleration.ReadValue().z * 90,
                Accelerometer.current.acceleration.ReadValue().x * 90);
        }
    }
}