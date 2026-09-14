using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Clase07DeviceCapabilitiesManager : MonoBehaviour
{
    [Header("Cámara")]
    [SerializeField] private RawImage cameraPreview;
    [SerializeField] private TMP_Text cameraStatusText;

    [Header("Ubicación")]
    [SerializeField] private TMP_Text latitudeText;
    [SerializeField] private TMP_Text longitudeText;
    [SerializeField] private TMP_Text headingText;
    [SerializeField] private RectTransform compassArrow;

    [Header("Estado")]
    [SerializeField] private TMP_Text permissionText;
    [SerializeField] private TMP_Text lifecycleText;

    private WebCamTexture webCamTexture;

    private bool locationStarted;
    private bool locationInitializing;

    private void Start()
    {
        cameraStatusText.text = "CÁMARA: APAGADA";

        latitudeText.text = "LAT: -";
        longitudeText.text = "LON: -";
        headingText.text = "HEADING: -";

        permissionText.text = "PERMISOS: -";
        lifecycleText.text = "APP: ACTIVA";
    }

    private void Update()
    {
        UpdateLocation();
        UpdateCompass();
    }

    // CÁMARA

    public void ToggleCamera()
    {
        if (webCamTexture == null)
        {
            StartCamera();
            return;
        }

        if (webCamTexture.isPlaying)
        {
            StopCamera();
        }
        else
        {
            webCamTexture.Play();

            cameraStatusText.text =
                "CÁMARA: ENCENDIDA";
        }
    }

    private void StartCamera()
    {
        WebCamDevice[] devices =
            WebCamTexture.devices;

        if (devices.Length == 0)
        {
            cameraStatusText.text =
                "CÁMARA: NO DISPONIBLE";

            return;
        }

        webCamTexture =
            new WebCamTexture(
                devices[0].name
            );

        cameraPreview.texture =
            webCamTexture;

        webCamTexture.Play();

        cameraStatusText.text =
            "CÁMARA: ENCENDIDA";
    }

    private void StopCamera()
    {
        if (webCamTexture == null)
        {
            return;
        }

        if (webCamTexture.isPlaying)
        {
            webCamTexture.Stop();
        }

        cameraStatusText.text =
            "CÁMARA: APAGADA";
    }

    // UBICACIÓN

    public void StartLocation()
    {
        if (locationInitializing)
        {
            return;
        }

        if (locationStarted)
        {
            StopLocation();
            return;
        }

        StartCoroutine(
            StartLocationService()
        );
    }

    private IEnumerator StartLocationService()
    {
        locationInitializing = true;

        permissionText.text =
            "UBICACIÓN: COMPROBANDO...";

        if (!Input.location.isEnabledByUser)
        {
            permissionText.text =
                "UBICACIÓN: DESACTIVADA";

            locationInitializing = false;

            yield break;
        }

        permissionText.text =
            "UBICACIÓN: INICIANDO...";

        Input.location.Start(
            10f,
            1f
        );

        Input.compass.enabled = true;

        int maxWait = 20;

        while (
            Input.location.status ==
            LocationServiceStatus.Initializing &&
            maxWait > 0
        )
        {
            yield return new WaitForSeconds(1f);

            maxWait--;
        }

        if (maxWait <= 0)
        {
            permissionText.text =
                "UBICACIÓN: TIMEOUT";

            Input.location.Stop();
            Input.compass.enabled = false;

            locationInitializing = false;

            yield break;
        }

        if (
            Input.location.status ==
            LocationServiceStatus.Failed
        )
        {
            permissionText.text =
                "UBICACIÓN: ERROR";

            Input.location.Stop();
            Input.compass.enabled = false;

            locationInitializing = false;

            yield break;
        }

        locationStarted = true;
        locationInitializing = false;

        permissionText.text =
            "UBICACIÓN: ACTIVA";
    }

    private void UpdateLocation()
    {
        if (!locationStarted)
        {
            return;
        }

        if (
            Input.location.status !=
            LocationServiceStatus.Running
        )
        {
            return;
        }

        LocationInfo data =
            Input.location.lastData;

        latitudeText.text =
            $"LAT: {data.latitude:F6}";

        longitudeText.text =
            $"LON: {data.longitude:F6}";
    }

    private void StopLocation()
    {
        Input.location.Stop();

        Input.compass.enabled = false;

        locationStarted = false;

        latitudeText.text = "LAT: -";
        longitudeText.text = "LON: -";
        headingText.text = "HEADING: -";

        permissionText.text =
            "UBICACIÓN: DETENIDA";

        if (compassArrow != null)
        {
            compassArrow.localEulerAngles =
                Vector3.zero;
        }
    }

    // BRÚJULA

    private void UpdateCompass()
    {
        if (!locationStarted)
        {
            return;
        }

        if (!Input.compass.enabled)
        {
            return;
        }

        float heading =
            Input.compass.trueHeading;

        headingText.text =
            $"HEADING: {heading:F1}°";

        if (compassArrow != null)
        {
            compassArrow.localEulerAngles =
                new Vector3(
                    0f,
                    0f,
                    -heading
                );
        }
    }

    // VIBRACIÓN

    public void Vibrate()
    {
        Handheld.Vibrate();
    }

    // CICLO DE VIDA

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            lifecycleText.text =
                "APP: PAUSADA";
        }
        else
        {
            lifecycleText.text =
                "APP: ACTIVA";
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            lifecycleText.text =
                "APP: CON FOCO";
        }
        else
        {
            lifecycleText.text =
                "APP: SIN FOCO";
        }
    }

    // LIMPIEZA

    private void OnDisable()
    {
        if (
            webCamTexture != null &&
            webCamTexture.isPlaying
        )
        {
            webCamTexture.Stop();
        }

        if (
            Input.location.status ==
            LocationServiceStatus.Running
        )
        {
            Input.location.Stop();
        }

        if (Input.compass.enabled)
        {
            Input.compass.enabled = false;
        }
    }
}