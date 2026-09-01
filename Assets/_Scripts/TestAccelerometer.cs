using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
//using Gyroscope = UnityEngine.InputSystem.Gyroscope;

public class TestAccelerometer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    private string _text = string.Empty;

    [Space]
    [SerializeField] private Transform cube;

    private bool enabledSensors = false;
    //private Quaternion baseRotation;
    private Vector3 acceleration;
    private Vector3 gravity;
    private Vector3 filteredGravity;

    public void OnStart()
    {
        Debug.Log("Start Sensors...");

        Debug.Log($"Supports Accelerometer: {SystemInfo.supportsAccelerometer}"); // true
        //Debug.Log($"Accelerometer - {Accelerometer.current}"); // Accelerometer - Accelerometer:/Accelerometer
        //if (Accelerometer.current != null)
        //{
        //    InputSystem.EnableDevice(Accelerometer.current);
        //    Debug.Log($"Enabled {Accelerometer.current.description} {Accelerometer.current.samplingFrequency}Hz"); // Enabled Accelerometer (WebGL) 60Hz
        //}

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
        //Debug.Log($"Gyroscope - {Gyroscope.current}");
        //if (Gyroscope.current != null)
        //{
        //    InputSystem.EnableDevice(Gyroscope.current);
        //    Debug.Log($"Enabled {Gyroscope.current.description} {Gyroscope.current.samplingFrequency}Hz");
        //}

        //Debug.Log($"LinearAccelerationSensor - {LinearAccelerationSensor.current}");
        //if (LinearAccelerationSensor.current != null)
        //{
        //    InputSystem.EnableDevice(LinearAccelerationSensor.current);
        //    Debug.Log($"Enabled {LinearAccelerationSensor.current.description} {LinearAccelerationSensor.current.samplingFrequency}Hz");
        //}

        Debug.Log($"StepCounter - {StepCounter.current}");
        if (StepCounter.current != null)
        {
            InputSystem.EnableDevice(StepCounter.current);
            Debug.Log($"Enabled {StepCounter.current.description} {StepCounter.current.samplingFrequency}Hz");
        }

        Debug.Log($"Input.compass - {Input.compass}");
        if (Input.compass != null)
        {
            Input.compass.enabled = true;
            Debug.Log($"Enabled {Input.compass}");
        }
        // con el gyro u otro sensor, tengo para subir y bajar la camara?, osea altura?

        Debug.Log("Finish Sensors.");

        //baseRotation = cube.rotation;

        enabledSensors = true;
    }

    private void Update()
    {
        if (!enabledSensors)
            return;

        _text = string.Empty;

        // en pc sale error en todos
        //if (Accelerometer.current != null) // funciona en los dos, gravedad + aceleracion, vector3
        //{
        //    _text += $"Accelerometer: {Accelerometer.current.acceleration.ReadValue()}";
        //}

        if (AttitudeSensor.current != null) // solo funciona en moderno, gyro, quaternio, 
        {
            _text += $"\nAttitudeSensor: {AttitudeSensor.current.attitude.ReadValue()} {AttitudeSensor.current.lastUpdateTime} {AttitudeSensor.current.magnitude} {AttitudeSensor.current.updateBeforeRender} {AttitudeSensor.current.wasUpdatedThisFrame}";
        }

        if (GravitySensor.current != null) // funciona en los dos, gravedad, vector3
        {
            _text += $"\nGravitySensor: {GravitySensor.current.gravity.ReadValue()}";
        }

        //if (Gyroscope.current != null) // solo funciona en moderno, aceleracion del AttitudeSensor, vector3
        //{
        //    _text += $"\nGyroscope: {Gyroscope.current.angularVelocity.ReadValue()}";
        //}

        //if (LinearAccelerationSensor.current != null) // funciona en los dos, aceleracion, vector3
        //{
        //    _text += $"\nLinearAccelerationSensor: {LinearAccelerationSensor.current.acceleration.ReadValue()}";
        //}

        if (StepCounter.current != null) // funciona en los dos, gravedad, vector3
        {
            _text += $"\nStepCounter: {StepCounter.current.stepCounter.ReadValue()}";
        }

        if (Input.compass != null) // solo funciona en moderno, vacio vector3, funciona float, funciona float
        {
            _text += $"\ncompass: {Input.compass.magneticHeading} {Input.compass.trueHeading} {Input.compass.headingAccuracy}";
            //                     float                           float                       0  En iphone si muestra un valor de 20.03567
        }

        text.SetText(_text);

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