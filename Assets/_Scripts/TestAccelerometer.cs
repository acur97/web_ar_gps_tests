using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestAccelerometer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    private string _text = string.Empty;

    [Space]
    [SerializeField] private Transform cube;
    [SerializeField] private Transform cubeParent;

    [Space]
    [SerializeField] private Transform compassRoot;
    [SerializeField] private RectTransform compasstrueHeading;
    [SerializeField] private RectTransform compassalphaHeading;
    [SerializeField] private RectTransform mapTest;


    [Space]
    [SerializeField] private Transform accelerationCube;
    [SerializeField] private float sensitivity = 2f;
    [SerializeField] private float maxVelocity = 2f;
    [SerializeField] private float maxHeight = 2f;
    private float velocity2;




    private float alphaHeading;
    private float alphaHeading2;
    private bool inProblemZone = false;
    private float lastGoodHeading;
    private float smoothedHeading;
    private bool initialized;



    private bool enabledSensors = false;
    private bool hasCompass = false;
    private bool preciseCompass = false;
    private Vector3 acceleration;
    private Vector3 gravity;
    private Vector3 filteredGravity;

    public void OnStart()
    {
        Debug.Log("Start Sensors...");

        Debug.Log($"Supports Accelerometer: {SystemInfo.supportsAccelerometer}");
        Debug.Log($"GravitySensor - {GravitySensor.current}");
        if (GravitySensor.current != null)
        {
            InputSystem.EnableDevice(GravitySensor.current);
            Debug.Log($"Enabled {GravitySensor.current.description} {GravitySensor.current.samplingFrequency}Hz");
        }

        Debug.Log($"LinearAccelerationSensor - {LinearAccelerationSensor.current}");
        if (LinearAccelerationSensor.current != null)
        {
            InputSystem.EnableDevice(LinearAccelerationSensor.current);
            Debug.Log($"Enabled {LinearAccelerationSensor.current.description} {LinearAccelerationSensor.current.samplingFrequency}Hz");
        }

        Debug.Log($"Supports Gyroscope: {SystemInfo.supportsGyroscope}");
        Debug.Log($"AttitudeSensor - {AttitudeSensor.current}");
        if (AttitudeSensor.current != null)
        {
            InputSystem.EnableDevice(AttitudeSensor.current);
            Debug.Log($"Enabled {AttitudeSensor.current.description} {AttitudeSensor.current.samplingFrequency}Hz");
        }

        Debug.Log($"Input.compass - {Input.compass}");
        if (Input.compass != null)
        {
            Input.compass.enabled = true;
            Debug.Log($"Enabled {Input.compass}");
        }
        // con el gyro u otro sensor, tengo para subir y bajar la camara?, osea altura?

        Debug.Log("Finish Sensors.");

        enabledSensors = true;
    }

    private float GetGravityZ()
    {
        if (GravitySensor.current == null)
            return 0f;

        return GravitySensor.current.gravity.ReadValue().z;
    }

    private float GetHeading(float _alphaHeading)
    {
        if (!initialized)
        {
            lastGoodHeading = _alphaHeading;
            initialized = true;
            return _alphaHeading;
        }

        inProblemZone =
            Mathf.Abs(GetGravityZ()) < 0.1;

        if (!inProblemZone)
        {
            lastGoodHeading = _alphaHeading;
        }
        //else
        //{
        //    return lastGoodHeading;
        //}

        float t = 1f - Mathf.Exp(-7f * Time.deltaTime);

        smoothedHeading = Mathf.LerpAngle(
            smoothedHeading,
            lastGoodHeading,
            t
        );

        return smoothedHeading;
    }

    private void Update()
    {
        if (!enabledSensors)
            return;

        _text = string.Empty;

        if (GravitySensor.current != null && GravitySensor.current.lastUpdateTime > 0) // funciona en los dos, gravedad, vector3
        {
            _text += $"\nGravitySensor: {GravitySensor.current.gravity.ReadValue()}";
        }

        if (LinearAccelerationSensor.current != null && LinearAccelerationSensor.current.lastUpdateTime > 0)
        {
            _text += $"\nLinearAccelerationSensor: {LinearAccelerationSensor.current.acceleration.ReadValue()}";
        }

        if (AttitudeSensor.current != null && AttitudeSensor.current.lastUpdateTime > 0) // solo funciona en moderno, gyro, quaternio, 
        {
            _text += $"\nAttitudeSensor: {AttitudeSensor.current.attitude.ReadValue()}";
        }

        if (!hasCompass && Input.compass.trueHeading != 0)
        {
            hasCompass = true;
        }

        if (hasCompass && !preciseCompass && Input.compass.headingAccuracy != 0)
        {
            preciseCompass = true;
        }

        if (hasCompass) // solo funciona en moderno, float
        {
            _text += $"\ncompass:{Input.compass.trueHeading}" /*{Input.compass.magneticHeading}"*/;
            //                               float (WebGL usa estos dos igual)
            _text += $" | Accuracy:{Input.compass.headingAccuracy}";
            //                   Solo en iOS muestra 20.03567

            if (preciseCompass) // iOS
            {
                compassRoot.localEulerAngles = new Vector3(0, Input.compass.trueHeading, 0);
                mapTest.localEulerAngles = new Vector3(0, 0, Input.compass.trueHeading);
                compasstrueHeading.localEulerAngles = new Vector3(0, 0, Input.compass.trueHeading);
            }
            else // Android (o hata pc)
            {
                alphaHeading = Mathf.Repeat(360f - PreciseLocation.Alpha, 360f);
                _text += $" | alphaHeading:{alphaHeading}";

                alphaHeading2 = GetHeading(alphaHeading);

                _text += $"\ninProblemZone:{inProblemZone} | correctedAlphaHeading:{alphaHeading2}";

                compassRoot.localEulerAngles = new Vector3(0, alphaHeading2, 0);
                mapTest.localEulerAngles = new Vector3(0, 0, alphaHeading2);
                compasstrueHeading.localEulerAngles = new Vector3(0, 0, Input.compass.trueHeading);
                compassalphaHeading.localEulerAngles = new Vector3(0, 0, alphaHeading);

                _text += $"\nAlpha:{PreciseLocation.Alpha} | Beta:{PreciseLocation.Beta} | Gamma:{PreciseLocation.Gamma}";
            }
        }

        text.SetText(_text);

        if (GravitySensor.current != null && GravitySensor.current.lastUpdateTime > 0)
        {
            acceleration = GravitySensor.current.gravity.ReadValue();
            gravity = new(-acceleration.x, acceleration.y, acceleration.z);

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
            cube.localRotation = Quaternion.FromToRotation(-cubeParent.up /*Vector3.down*/, filteredGravity);
            #endregion

            #region v7 -- se bugea arriba y abajo
            //gravity = -gravity.normalized;
            //Vector3 forward = Vector3.ProjectOnPlane(Vector3.forward, gravity).normalized;
            //cube.rotation = Quaternion.LookRotation(forward, gravity);
            #endregion


            // -----------------------


            Vector3 gravity2 = GravitySensor.current.gravity.ReadValue();

            // Dirección "arriba" del mundo respecto al teléfono.
            Vector3 worldUp = -gravity2.normalized;

            // ¿Cuánta aceleración hay en la dirección vertical?
            float verticalAcceleration =
                Vector3.Dot(acceleration, worldUp);

            // Aceleración -> velocidad
            velocity2 += verticalAcceleration * sensitivity * Time.deltaTime;

            velocity2 = Mathf.Clamp(
                velocity2,
                -maxVelocity,
                maxVelocity
            );

            // Velocidad -> posición
            Vector3 position = accelerationCube.position;

            position.y += velocity2 * Time.deltaTime;

            position.y = Mathf.Clamp(
                position.y,
                -maxHeight,
                maxHeight
            );

            accelerationCube.position = position;
        }
    }
}