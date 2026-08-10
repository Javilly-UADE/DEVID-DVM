using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class Clase02TouchManager : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Camera worldCamera;
    [SerializeField] private GameObject touchIndicator;
    [SerializeField] private TMP_Text statusText;

    [Header("Umbrales")]
    [SerializeField] private float tapMaxDuration = 0.35f;
    [SerializeField] private float holdMinDuration = 0.8f;
    [SerializeField] private float moveTolerance = 80f;
    [SerializeField] private float swipeMinDistance = 150f;

    [Header("Colores")]
    [SerializeField] private Color beganColor = Color.white;
    [SerializeField] private Color tapColor = Color.green;
    [SerializeField] private Color holdColor = Color.blue;
    [SerializeField] private Color dragColor = Color.yellow;
    [SerializeField] private Color swipeColor = Color.magenta;
    [SerializeField] private Color canceledColor = Color.gray;

    private Vector2 startPosition;
    private float startTime;

    private bool isDragging;
    private bool holdDetected;

    private Renderer indicatorRenderer;

    private void Awake()
    {
        if (worldCamera == null)
        {
            worldCamera = Camera.main;
        }

        if (touchIndicator != null)
        {
            indicatorRenderer = touchIndicator.GetComponent<Renderer>();
            touchIndicator.SetActive(false);
        }

        SetStatus("Esperando touch...");
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

        switch (touch.phase)
        {
            case TouchPhase.Began:
                OnTouchBegan(touch);
                break;

            case TouchPhase.Moved:
                OnTouchMoved(touch);
                break;

            case TouchPhase.Stationary:
                OnTouchStationary(touch);
                break;

            case TouchPhase.Ended:
                OnTouchEnded(touch);
                break;

            case TouchPhase.Canceled:
                OnTouchCanceled();
                break;
        }
    }

    private void OnTouchBegan(Touch touch)
    {
        startPosition = touch.screenPosition;
        startTime = Time.time;

        isDragging = false;
        holdDetected = false;

        touchIndicator.SetActive(true);

        MoveIndicator(touch.screenPosition);
        SetIndicatorColor(beganColor);

        SetStatus("Touch iniciado");
    }

    private void OnTouchMoved(Touch touch)
    {
        float distance = Vector2.Distance(
            startPosition,
            touch.screenPosition
        );

        // Permitimos un pequeño movimiento accidental antes
        // de considerar que realmente comenzó un drag.
        if (distance >= moveTolerance)
        {
            isDragging = true;

            MoveIndicator(touch.screenPosition);
            SetIndicatorColor(dragColor);

            SetStatus(
                $"Drag\n" +
                $"Distancia: {distance:F0}px"
            );
        }
    }

    private void OnTouchStationary(Touch touch)
    {
        float duration = Time.time - startTime;

        float distance = Vector2.Distance(
            startPosition,
            touch.screenPosition
        );

        if (!holdDetected &&
            duration >= holdMinDuration &&
            distance < moveTolerance)
        {
            holdDetected = true;

            SetIndicatorColor(holdColor);

            SetStatus(
                $"Pulsación prolongada\n" +
                $"Duración: {duration:F2}s"
            );
        }
    }

    private void OnTouchEnded(Touch touch)
    {
        Vector2 endPosition = touch.screenPosition;

        float duration = Time.time - startTime;

        float distance = Vector2.Distance(
            startPosition,
            endPosition
        );

        // Si recorrió suficiente distancia, interpretamos Swipe.
        if (distance >= swipeMinDistance)
        {
            DetectSwipe(endPosition);
        }
        // Si ya habíamos detectado una pulsación prolongada,
        // no queremos interpretarla otra vez como Tap.
        else if (holdDetected)
        {
            SetStatus("Pulsación prolongada finalizada");
        }
        // Poco tiempo + poco movimiento = Tap.
        else if (duration <= tapMaxDuration &&
                 distance < moveTolerance)
        {
            SetIndicatorColor(tapColor);

            SetStatus(
                $"Tap\n" +
                $"Duración: {duration:F2}s"
            );
        }
        // Si hubo movimiento, pero no alcanzó para Swipe.
        else if (isDragging)
        {
            SetStatus("Drag finalizado");
        }
        else
        {
            SetStatus("Touch finalizado");
        }
    }

    private void OnTouchCanceled()
    {
        touchIndicator.SetActive(false);

        SetIndicatorColor(canceledColor);
        SetStatus("Touch cancelado");
    }

    private void DetectSwipe(Vector2 endPosition)
    {
        Vector2 difference =
            endPosition - startPosition;

        string direction;

        // Determinamos cuál de los dos ejes tuvo
        // un desplazamiento mayor.
        if (Mathf.Abs(difference.x) >
            Mathf.Abs(difference.y))
        {
            direction = difference.x > 0
                ? "Derecha"
                : "Izquierda";
        }
        else
        {
            direction = difference.y > 0
                ? "Arriba"
                : "Abajo";
        }

        SetIndicatorColor(swipeColor);

        SetStatus(
            $"Swipe {direction}\n" +
            $"Distancia: {difference.magnitude:F0}px"
        );
    }

    private void MoveIndicator(Vector2 screenPosition)
    {
        if (worldCamera == null ||
            touchIndicator == null)
        {
            return;
        }

        float distanceFromCamera = Mathf.Abs(
            worldCamera.transform.position.z -
            touchIndicator.transform.position.z
        );

        Vector3 worldPosition =
            worldCamera.ScreenToWorldPoint(
                new Vector3(
                    screenPosition.x,
                    screenPosition.y,
                    distanceFromCamera
                )
            );

        worldPosition.z =
            touchIndicator.transform.position.z;

        touchIndicator.transform.position =
            worldPosition;
    }

    private void SetIndicatorColor(Color color)
    {
        if (indicatorRenderer != null)
        {
            indicatorRenderer.material.color = color;
        }
    }

    private void SetStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }

        Debug.Log(message);
    }
}