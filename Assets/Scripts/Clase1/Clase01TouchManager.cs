using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class Clase01TouchManager : MonoBehaviour
{
    [SerializeField] private Camera worldCamera;
    [SerializeField] private GameObject touchIndicator;
    [SerializeField] private TMP_Text statusText;

    private void Awake()
    {
        if (worldCamera == null)
        {
            worldCamera = Camera.main;
        }

        if (touchIndicator != null)
        {
            touchIndicator.SetActive(false);
        }

        if (statusText != null)
        {
            statusText.text = "Esperando touch...";
        }
    }

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    private void Update()
    {
        if (Touch.activeTouches.Count == 0)
        {
            return;
        }

        Touch touch = Touch.activeTouches[0];

        if (touch.phase == TouchPhase.Began)
        {
            ShowTouch(touch.screenPosition);
        }
    }

    private void ShowTouch(Vector2 screenPosition)
    {
        if (worldCamera == null || touchIndicator == null)
        {
            Debug.LogWarning("Faltan referencias en Clase01TouchManager.");
            return;
        }

        Vector3 worldPosition = ScreenToWorldPosition(screenPosition);

        touchIndicator.transform.position = worldPosition;
        touchIndicator.SetActive(true);

        if (statusText != null)
        {
            statusText.text =
                "Touch detectado\n" +
                $"Pantalla: {screenPosition}\n" +
                $"Mundo: {worldPosition}";
        }

        Debug.Log($"Touch detectado en pantalla: {screenPosition}");
    }

    private Vector3 ScreenToWorldPosition(Vector2 screenPosition)
    {
        float indicatorZ = touchIndicator.transform.position.z;

        float distanceFromCamera = Mathf.Abs(
            indicatorZ - worldCamera.transform.position.z
        );

        Vector3 worldPosition = worldCamera.ScreenToWorldPoint(
            new Vector3(
                screenPosition.x,
                screenPosition.y,
                distanceFromCamera
            )
        );

        worldPosition.z = indicatorZ;

        return worldPosition;
    }
}
