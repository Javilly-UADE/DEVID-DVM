using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class Clase03MultitouchExerciseManager : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Camera worldCamera;
    [SerializeField] private GameObject touchIndicatorPrefab;
    [SerializeField] private Transform indicatorsParent;
    [SerializeField] private TMP_Text activeTouchesText;

    [Header("Materiales")]
    [SerializeField] private Material[] materials;

    private Dictionary<int, GameObject> touchIndicators =
        new Dictionary<int, GameObject>();

    private void Awake()
    {
        if (worldCamera == null)
        {
            worldCamera = Camera.main;
        }

        UpdateTouchesText();
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
        foreach (Touch touch in Touch.activeTouches)
        {
            int touchId = touch.touchId;

            if (touch.phase == TouchPhase.Began)
            {
                CreateIndicator(touchId);
            }

            if (touchIndicators.ContainsKey(touchId))
            {
                MoveIndicator(
                    touchIndicators[touchId],
                    touch.screenPosition
                );
            }

            if (touch.phase == TouchPhase.Ended ||
                touch.phase == TouchPhase.Canceled)
            {
                RemoveIndicator(touchId);
            }
        }

        UpdateTouchesText();
    }

    private void CreateIndicator(int touchId)
    {
        if (touchIndicators.ContainsKey(touchId))
        {
            return;
        }

        GameObject newIndicator = Instantiate(
            touchIndicatorPrefab,
            indicatorsParent
        );

        newIndicator.name =
            $"TouchIndicator_{touchId}";

        AssignRandomMaterial(newIndicator);

        touchIndicators.Add(
            touchId,
            newIndicator
        );
    }

    private void RemoveIndicator(int touchId)
    {
        if (!touchIndicators.ContainsKey(touchId))
        {
            return;
        }

        Destroy(touchIndicators[touchId]);

        touchIndicators.Remove(touchId);
    }

    private void AssignRandomMaterial(GameObject indicator)
    {
        if (materials == null ||
            materials.Length == 0)
        {
            return;
        }

        Renderer indicatorRenderer =
            indicator.GetComponentInChildren<Renderer>();

        if (indicatorRenderer == null)
        {
            return;
        }

        int randomIndex =
            Random.Range(0, materials.Length);

        indicatorRenderer.sharedMaterial =
            materials[randomIndex];
    }

    private void MoveIndicator(
        GameObject indicator,
        Vector2 screenPosition
    )
    {
        if (worldCamera == null ||
            indicator == null)
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

    private void UpdateTouchesText()
    {
        if (activeTouchesText != null)
        {
            activeTouchesText.text =
                $"Touches activos: {touchIndicators.Count}";
        }
    }
}