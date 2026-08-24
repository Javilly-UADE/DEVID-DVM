using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class Clase03GestureManager : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform manipulableObject;
    [SerializeField] private TMP_Text gestureText;

    [Header("Pinch")]
    [SerializeField] private float pinchSensitivity = 0.005f;
    [SerializeField] private float minScale = 0.5f;
    [SerializeField] private float maxScale = 3f;

    [Header("Rotación")]
    [SerializeField] private float rotationSensitivity = 1f;

    private Vector3 initialScale;
    private float currentScale = 1f;

    private void Awake()
    {
        if (manipulableObject != null)
        {
            initialScale =
                manipulableObject.localScale;
        }

        SetGestureText("Gesto: -");
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
        if (Touch.activeTouches.Count < 2)
        {
            SetGestureText(
                "Gesto: esperando dos dedos"
            );

            return;
        }

        Touch touch1 = Touch.activeTouches[0];
        Touch touch2 = Touch.activeTouches[1];

        DetectPinch(touch1, touch2);
        DetectRotation(touch1, touch2);
    }

    private void DetectPinch(
        Touch touch1,
        Touch touch2
    )
    {
        Vector2 currentPosition1 =
            touch1.screenPosition;

        Vector2 currentPosition2 =
            touch2.screenPosition;

        Vector2 previousPosition1 =
            currentPosition1 - touch1.delta;

        Vector2 previousPosition2 =
            currentPosition2 - touch2.delta;

        float currentDistance =
            Vector2.Distance(
                currentPosition1,
                currentPosition2
            );

        float previousDistance =
            Vector2.Distance(
                previousPosition1,
                previousPosition2
            );

        float distanceDelta =
            currentDistance - previousDistance;

        currentScale +=
            distanceDelta * pinchSensitivity;

        currentScale = Mathf.Clamp(
            currentScale,
            minScale,
            maxScale
        );

        manipulableObject.localScale =
            initialScale * currentScale;

        SetGestureText(
            $"Pinch\n" +
            $"Distancia: {currentDistance:F0}px\n" +
            $"Delta: {distanceDelta:F1}px"
        );
    }

    private void DetectRotation(
        Touch touch1,
        Touch touch2
    )
    {
        Vector2 currentPosition1 =
            touch1.screenPosition;

        Vector2 currentPosition2 =
            touch2.screenPosition;

        Vector2 previousPosition1 =
            currentPosition1 - touch1.delta;

        Vector2 previousPosition2 =
            currentPosition2 - touch2.delta;

        Vector2 previousDirection =
            previousPosition2 -
            previousPosition1;

        Vector2 currentDirection =
            currentPosition2 -
            currentPosition1;

        float angleDelta =
            Vector2.SignedAngle(
                previousDirection,
                currentDirection
            );

        manipulableObject.Rotate(
            0f,
            0f,
            angleDelta * rotationSensitivity
        );
    }

    private void SetGestureText(string message)
    {
        if (gestureText != null)
        {
            gestureText.text = message;
        }
    }
}