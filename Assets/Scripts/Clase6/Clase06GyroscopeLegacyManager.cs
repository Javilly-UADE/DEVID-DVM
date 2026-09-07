using TMPro;
using UnityEngine;

public class Clase06GyroscopeLegacyManager : MonoBehaviour
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

    private Gyroscope gyro;

    private Quaternion currentAttitude;
    private Quaternion smoothedAttitude;
    private Quaternion baselineAttitude;

    private bool initialized;
    private bool calibrated;

    private void Start()
    {
        gyro = Input.gyro;
        gyro.enabled = true;

        statusText.text = "ESTADO: GIROSCOPIO HABILITADO";
    }

    private void Update()
    {
        UpdateAngularVelocity();
        UpdateOrientation();
    }

    private void UpdateAngularVelocity()
    {
        Vector3 angularVelocity =
            gyro.rotationRateUnbiased;

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
        currentAttitude =
            gyro.attitude;

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
            $"ESTADO: ENABLED = {gyro.enabled}";

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