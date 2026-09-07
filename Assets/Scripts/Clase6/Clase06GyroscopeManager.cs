using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

using InputGyroscope = UnityEngine.InputSystem.Gyroscope;

public class Clase06GyroscopeManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text angularVelocityText;
    [SerializeField] private TMP_Text orientationText;
    [SerializeField] private TMP_Text relativeOrientationText;
    [SerializeField] private TMP_Text statusText;

    [Header("Objeto")]
    [SerializeField] private Transform feedbackObject;
    [SerializeField] private Renderer feedbackRenderer;

    [Header("Configuración")]
    [SerializeField] private float smoothing = 5f;

    private Quaternion currentAttitude;
    private Quaternion smoothedAttitude;
    private Quaternion baselineAttitude;

    private bool initialized;
    private bool calibrated;

    private void OnEnable()
    {
        if (InputGyroscope.current != null)
        {
            InputSystem.EnableDevice(InputGyroscope.current);
        }

        if (AttitudeSensor.current != null)
        {
            InputSystem.EnableDevice(AttitudeSensor.current);
        }
    }

    private void OnDisable()
    {
        if (InputGyroscope.current != null)
        {
            InputSystem.DisableDevice(InputGyroscope.current);
        }

        if (AttitudeSensor.current != null)
        {
            InputSystem.DisableDevice(AttitudeSensor.current);
        }
    }

    private void Update()
    {
        UpdateAngularVelocity();
        UpdateOrientation();
    }

    private void UpdateAngularVelocity()
    {
        if (InputGyroscope.current == null)
        {
            angularVelocityText.text =
                "VELOCIDAD ANGULAR: NO DISPONIBLE";

            return;
        }

        Vector3 angularVelocity =
            InputGyroscope.current.angularVelocity.ReadValue();

        Vector3 degreesPerSecond =
            angularVelocity * Mathf.Rad2Deg;

        angularVelocityText.text =
            $"VELOCIDAD ANGULAR: " +
            $"X: {degreesPerSecond.x:F2} " +
            $"Y: {degreesPerSecond.y:F2} " +
            $"Z: {degreesPerSecond.z:F2}";
    }

    private void UpdateOrientation()
    {
        if (AttitudeSensor.current == null)
        {
            orientationText.text =
                "ORIENTACIÓN: NO DISPONIBLE";

            relativeOrientationText.text =
                "ORIENTACIÓN RELATIVA: NO DISPONIBLE";

            statusText.text =
                "ESTADO: SENSOR NO DISPONIBLE";

            return;
        }

        currentAttitude =
            AttitudeSensor.current.attitude.ReadValue();

        if (!initialized)
        {
            smoothedAttitude = currentAttitude;
            initialized = true;
        }

        smoothedAttitude =
            Quaternion.Slerp(
                smoothedAttitude,
                currentAttitude,
                smoothing * Time.deltaTime
            );

        Vector3 euler =
            smoothedAttitude.eulerAngles;

        euler.x = NormalizeAngle(euler.x);
        euler.y = NormalizeAngle(euler.y);
        euler.z = NormalizeAngle(euler.z);

        orientationText.text =
            $"ORIENTACIÓN: " +
            $"X: {euler.x:F2} " +
            $"Y: {euler.y:F2} " +
            $"Z: {euler.z:F2}";

        statusText.text =
            "ESTADO: OK";

        UpdateRelativeOrientation();
    }

    private void UpdateRelativeOrientation()
    {
        if (!calibrated)
        {
            relativeOrientationText.text =
                "ORIENTACIÓN RELATIVA: SIN CALIBRAR";

            return;
        }

        Quaternion relativeAttitude =
            Quaternion.Inverse(baselineAttitude) *
            smoothedAttitude;

        Vector3 relativeEuler =
            relativeAttitude.eulerAngles;

        relativeEuler.x =
            NormalizeAngle(relativeEuler.x);

        relativeEuler.y =
            NormalizeAngle(relativeEuler.y);

        relativeEuler.z =
            NormalizeAngle(relativeEuler.z);

        relativeOrientationText.text =
            $"ORIENTACIÓN RELATIVA: " +
            $"X: {relativeEuler.x:F2} " +
            $"Y: {relativeEuler.y:F2} " +
            $"Z: {relativeEuler.z:F2}";

        if (feedbackObject != null)
        {
            feedbackObject.localRotation =
                relativeAttitude;
        }
    }

    public void Calibrate()
    {
        if (!initialized)
        {
            return;
        }

        baselineAttitude =
            smoothedAttitude;

        calibrated = true;
    }

    public void Action()
    {
        if (feedbackRenderer != null)
        {
            feedbackRenderer.material.color =
                Random.ColorHSV();
        }
    }

    private float NormalizeAngle(float angle)
    {
        if (angle > 180f)
        {
            angle -= 360f;
        }

        return angle;
    }
}