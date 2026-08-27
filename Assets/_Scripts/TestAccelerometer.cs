using System;
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
    private Quaternion baseRotation;
    private Vector3 acceleration;
    private Vector3 gravity;
    private Vector3 filteredGravity;

    public void OnStart()
    {
        Debug.Log("Start Sensors...");

        Debug.Log($"Supports Accelerometer: {SystemInfo.supportsAccelerometer}");
        Debug.Log($"Accelerometer - {Accelerometer.current}");
        if (Accelerometer.current != null)
        {
            InputSystem.EnableDevice(Accelerometer.current);
            Debug.Log($"Enabled {Accelerometer.current.description} {Accelerometer.current.samplingFrequency}Hz"); //60Hz
        }

        Debug.Log($"AttitudeSensor - {AttitudeSensor.current}");
        if (AttitudeSensor.current != null)
        {
            InputSystem.EnableDevice(AttitudeSensor.current);
            Debug.Log($"Enabled {AttitudeSensor.current.description} {AttitudeSensor.current.samplingFrequency}Hz");
        }

        Debug.Log($"GravitySensor - {GravitySensor.current}");
        if (GravitySensor.current != null)
        {
            InputSystem.EnableDevice(GravitySensor.current);
            Debug.Log($"Enabled {GravitySensor.current.description} {GravitySensor.current.samplingFrequency}Hz");
        }

        Debug.Log($"Supports Gyroscope: {SystemInfo.supportsGyroscope}");
        Debug.Log($"Gyroscope - {Gyroscope.current}");
        if (Gyroscope.current != null)
        {
            InputSystem.EnableDevice(Gyroscope.current);
            Debug.Log($"Enabled {Gyroscope.current.description} {Gyroscope.current.samplingFrequency}Hz");
        }

        Debug.Log($"LinearAccelerationSensor - {LinearAccelerationSensor.current}");
        if (LinearAccelerationSensor.current != null)
        {
            InputSystem.EnableDevice(LinearAccelerationSensor.current);
            Debug.Log($"Enabled {LinearAccelerationSensor.current.description} {LinearAccelerationSensor.current.samplingFrequency}Hz");
        }

        Debug.Log($"MagneticFieldSensor - {MagneticFieldSensor.current}");
        if (MagneticFieldSensor.current != null)
        {
            InputSystem.EnableDevice(MagneticFieldSensor.current);
            Debug.Log($"Enabled {MagneticFieldSensor.current.description} {MagneticFieldSensor.current.samplingFrequency}Hz");
        }

        Debug.Log($"Input.compass - {Input.compass}");
        if (Input.compass != null)
        {
            Input.compass.enabled = true;
            Debug.Log($"Enabled {Input.compass}");
        }
        //Debug.Log($"Input.gyro - {Input.gyro}");
        //if (Input.gyro != null)
        //{
        //    Input.gyro.enabled = true;
        //    Debug.Log($"Enabled {Input.gyro}");
        //}
        // con el gyro u otro sensor, tengo para subir y bajar la camara?, osea altura?

        Debug.Log("Finish Sensors.");

        baseRotation = cube.rotation;

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
        if (Accelerometer.current != null) // funciona en los dos, gravedad + aceleracion, vector3
        {
            text.text += $"Accelerometer: {Accelerometer.current.acceleration.ReadValue()}";
        }

        if (AttitudeSensor.current != null) // solo funciona en moderno, gyro, quaternio, 
        {
            text.text += $"\nAttitudeSensor: {AttitudeSensor.current.attitude.ReadValue()}";
        }

        if (GravitySensor.current != null) // funciona en los dos, gravedad, vector3
        {
            text.text += $"\nGravitySensor: {GravitySensor.current.gravity.ReadValue()}";
        }

        if (Gyroscope.current != null) // solo funciona en moderno, aceleracion del AttitudeSensor, vector3
        {
            text.text += $"\nGyroscope: {Gyroscope.current.angularVelocity.ReadValue()}";
        }

        if (LinearAccelerationSensor.current != null) // funciona en los dos, aceleracion, vector3
        {
            text.text += $"\nLinearAccelerationSensor: {LinearAccelerationSensor.current.acceleration.ReadValue()}";
        }

        if (MagneticFieldSensor.current != null) // error en viejo, en nuevo nada, no muestra en ninguno de los dos
        {
            text.text += $"\nMagneticFieldSensor: {MagneticFieldSensor.current.magneticField.ReadValue()}";
        }

        if (Input.compass != null) // solo funciona en moderno, vacio vector3, funciona float, funciona float
        {
            text.text += $"\ncompass: {Input.compass.rawVector} {Input.compass.magneticHeading} {Input.compass.trueHeading} {Input.compass.headingAccuracy}";
            //                                       0, 0, 0                    float                           float                       0
        }

        //if (Input.gyro != null) // solo funciona en moderno, vacio vector3, funciona float, funciona float
        //{
        //    text.text += $"\ngyro: {Input.gyro.rotationRate} {Input.gyro.userAcceleration} {Input.gyro.gravity}\n{Input.gyro.rotationRate} {Input.gyro.attitude} {Input.gyro.updateInterval}";
        //    //                                 gyroscope                 linear acceleration           acceleration                                    atitude               0.016666
        //}

        if (GravitySensor.current != null)
        {
            acceleration = GravitySensor.current.gravity.ReadValue();
            //gravity = new(-acceleration.x, -acceleration.y, acceleration.z);
            gravity = new(-acceleration.x, acceleration.y, acceleration.z); // no se invierte si giramos es la camara
            //gravity.Normalize();

            filteredGravity = Vector3.Lerp(
                filteredGravity,
                gravity,
                1f - MathF.Exp(-21 * Time.deltaTime));

            filteredGravity.Normalize();

            #region v1, al tener vertical el telefono se descontrola el cubo
            //cube.rotation = Quaternion.LookRotation(gravity);
            #endregion

            #region v2, se va desfazando, --- el mejor por ahora ---
            //Quaternion tilt = Quaternion.FromToRotation(cube.up, gravity);
            //cube.rotation = tilt * cube.rotation;
            #endregion

            #region v3 -- ultimo mejor con desfaces tambien --
            //Quaternion tilt = Quaternion.FromToRotation(-cube.up, gravity);
            //cube.rotation = tilt * cube.rotation;
            #endregion

            #region v4, no desfasa pero girar a los lados gira mal
            //Quaternion tilt = Quaternion.FromToRotation(baseRotation * Vector3.up, gravity);
            //cube.rotation = tilt * baseRotation;
            #endregion

            #region v5
            //Quaternion tilt = Quaternion.FromToRotation(Vector3.up, gravity);
            //cube.rotation = tilt * baseRotation;
            #endregion

            #region v6 -- bastante bueno, no se desfasa, pero teniendo el celular acostado hace rotaciones raras, pero casi nunca se haran
            cube.rotation = Quaternion.FromToRotation(Vector3.down, filteredGravity);
            #endregion

            #region v7 -- se bugea arriba y abajo
            //gravity = -gravity.normalized;
            //Vector3 forward = Vector3.ProjectOnPlane(Vector3.forward, gravity).normalized;

            //cube.rotation = Quaternion.LookRotation(forward, gravity);
            #endregion
        }
    }
}