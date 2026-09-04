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
    [SerializeField] private RectTransform compassTest;
    [SerializeField] private RectTransform mapTest;

    [Header("Dead Zone")]
    [SerializeField] private float deadZoneGravityZ = 0.05f;
    [SerializeField] private double deadZoneBetaMin = 85.0;
    [SerializeField] private double deadZoneBetaMax = 91.0;

    [Header("180° Jump Detection")]
    [SerializeField] private float jumpMin = 150f;
    [SerializeField] private float jumpMax = 210f;

    private float lastGoodHeading;
    private bool initialized;



    private bool enabledSensors = false;
    //private float compassCorrected;
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

    private float GetHeading()
    {
        float rawHeading = Input.compass.trueHeading;

        if (!initialized)
        {
            lastGoodHeading = rawHeading;
            initialized = true;
            return rawHeading;
        }

        float gravityZ = GetGravityZ();
        float beta = PreciseLocation.Beta;

        bool inProblemZone =
            Mathf.Abs(gravityZ) < deadZoneGravityZ &&
            beta > deadZoneBetaMin &&
            beta < deadZoneBetaMax;

        float difference = Mathf.Abs(
            Mathf.DeltaAngle(lastGoodHeading, rawHeading)
        );

        bool looksLike180Jump =
            difference > jumpMin &&
            difference < jumpMax;

        if (inProblemZone && looksLike180Jump)
        {
            return lastGoodHeading;
        }
        lastGoodHeading = rawHeading;

        return rawHeading;
    }

    private float GetGravityZ()
    {
        if (GravitySensor.current == null)
            return 0f;

        return GravitySensor.current.gravity.ReadValue().z;
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

        if (Input.compass != null) // solo funciona en moderno, float
        {
            _text += $"\ncompass: {Input.compass.trueHeading}" /*{Input.compass.magneticHeading}"*/ /*{Input.compass.headingAccuracy}"*/;
            //                               float (WebGL usa estos dos igual)                         Solo en iOS muestra 20.03567

            float heading = Mathf.Repeat(360f - PreciseLocation.Alpha, 360f);
            _text += $" | Corrected Alpha:{heading}";

            float heading2 = GetHeading();

            _text += $" | Corrected heading:{heading2}";

            compassRoot.localEulerAngles = new Vector3(0, heading, 0);
            compassTest.localEulerAngles = new Vector3(0, 0, Input.compass.trueHeading);
            mapTest.localEulerAngles = new Vector3(0, 0, heading);

            _text += $"\nAlpha:{PreciseLocation.Alpha} Beta:{PreciseLocation.Beta} Gamma:{PreciseLocation.Gamma}";
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
        }
    }
}