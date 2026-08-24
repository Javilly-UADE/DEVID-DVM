using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class Clase03MultitouchManager : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Camera worldCamera;

    [SerializeField] private GameObject touchIndicator1;
    [SerializeField] private GameObject touchIndicator2;

    [SerializeField] private TMP_Text activeTouchesText;
    [SerializeField] private TMP_Text touch1Text;
    [SerializeField] private TMP_Text touch2Text;

    private void Awake()
    {
        if (worldCamera == null)
        {
            worldCamera = Camera.main;
        }

        if (touchIndicator1 != null)
        {
            touchIndicator1.SetActive(false);
        }

        if (touchIndicator2 != null)
        {
            touchIndicator2.SetActive(false);
        }

        UpdateTexts(0);
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
        int touchCount = Touch.activeTouches.Count;

        activeTouchesText.text =
            $"Touches activos: {touchCount}";

        if (touchCount >= 1)
        {
            Touch touch1 = Touch.activeTouches[0];

            touchIndicator1.SetActive(true);

            MoveIndicator(
                touchIndicator1,
                touch1.screenPosition
            );

            touch1Text.text =
                $"Touch 1: {touch1.screenPosition}";
        }
        else
        {
            touchIndicator1.SetActive(false);

            touch1Text.text =
                "Touch 1: -";
        }

        if (touchCount >= 2)
        {
            Touch touch2 = Touch.activeTouches[1];

            touchIndicator2.SetActive(true);

            MoveIndicator(
                touchIndicator2,
                touch2.screenPosition
            );

            touch2Text.text =
                $"Touch 2: {touch2.screenPosition}";
        }
        else
        {
            touchIndicator2.SetActive(false);

            touch2Text.text =
                "Touch 2: -";
        }
    }

    private void MoveIndicator(
        GameObject indicator,
        Vector2 screenPosition
    )
    {
        if (worldCamera == null || indicator == null)
        {
            return;
        }

        float indicatorZ =
            indicator.transform.position.z;

        float distanceFromCamera = Mathf.Abs(
            indicatorZ -
            worldCamera.transform.position.z
        );

        Vector3 worldPosition =
            worldCamera.ScreenToWorldPoint(
                new Vector3(
                    screenPosition.x,
                    screenPosition.y,
                    distanceFromCamera
                )
            );

        worldPosition.z = indicatorZ;

        indicator.transform.position =
            worldPosition;
    }

    private void UpdateTexts(int touchCount)
    {
        if (activeTouchesText != null)
        {
            activeTouchesText.text =
                $"Touches activos: {touchCount}";
        }

        if (touch1Text != null)
        {
            touch1Text.text = "Touch 1: -";
        }

        if (touch2Text != null)
        {
            touch2Text.text = "Touch 2: -";
        }
    }
}