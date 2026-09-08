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




    private float alphaHeading;
    private float alphaHeading2;
    private bool inProblemZone = false;
    private float lastGoodHeading;
    private float smoothedHeading;
    private bool initialized;



    private bool enabledSensors = false;
    private bool hasCompass = false;
    private bool preciseCompass = false;
    private Vector3 gravity;
    private Vector3 gravityOriented;
    private Vector3 gravityFiltered;



    private Quaternion gyroscope;




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
                //compassRoot.localEulerAngles = new Vector3(0, Input.compass.trueHeading, 0);
                mapTest.localEulerAngles = new Vector3(0, 0, Input.compass.trueHeading);
                compasstrueHeading.localEulerAngles = new Vector3(0, 0, Input.compass.trueHeading);
            }
            else // Android (o hata pc)
            {
                alphaHeading = Mathf.Repeat(360f - PreciseLocation.Alpha, 360f);
                _text += $" | alphaHeading:{alphaHeading}";

                alphaHeading2 = GetHeading(alphaHeading);

                _text += $"\ninProblemZone:{inProblemZone} | correctedAlphaHeading:{alphaHeading2}";

                //compassRoot.localEulerAngles = new Vector3(0, alphaHeading2, 0);
                mapTest.localEulerAngles = new Vector3(0, 0, alphaHeading2);
                compasstrueHeading.localEulerAngles = new Vector3(0, 0, Input.compass.trueHeading);
                compassalphaHeading.localEulerAngles = new Vector3(0, 0, alphaHeading);

                _text += $"\nAlpha:{PreciseLocation.Alpha} | Beta:{PreciseLocation.Beta} | Gamma:{PreciseLocation.Gamma}";
            }
        }

        text.SetText(_text);

        if (AttitudeSensor.current != null && AttitudeSensor.current.lastUpdateTime > 0)
        {
            gyroscope = AttitudeSensor.current.attitude.ReadValue();
            cube.localRotation = new Quaternion(gyroscope.y, -gyroscope.z, -gyroscope.x, gyroscope.w);
        }
        else if (GravitySensor.current != null && GravitySensor.current.lastUpdateTime > 0)
        {
            gravity = GravitySensor.current.gravity.ReadValue();
            gravityOriented = new(-gravity.x, gravity.y, gravity.z);

            gravityFiltered = Vector3.Lerp(
                gravityFiltered,
                gravityOriented,
                1f - MathF.Exp(-21 * Time.deltaTime));

            gravityFiltered.Normalize();
            cube.localRotation = Quaternion.FromToRotation(-cubeParent.up, gravityFiltered);
        }
    }
}