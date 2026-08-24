using UnityEngine;
using UnityEngine.InputSystem;

public class Clase04PlayerController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference actionAction;

    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 4f;

    [Header("Feedback")]
    [SerializeField] private Renderer playerRenderer;

    private void Awake()
    {
        if (playerRenderer == null)
        {
            playerRenderer = GetComponent<Renderer>();
        }
    }

    private void OnEnable()
    {
        moveAction.action.Enable();
        actionAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        actionAction.action.Disable();
    }

    private void Update()
    {
        Move();

        if (actionAction.action.WasPressedThisFrame())
        {
            ExecuteAction();
        }
    }

    private void Move()
    {
        Vector2 input =
            moveAction.action.ReadValue<Vector2>();

        Vector3 movement =
            new Vector3(
                input.x,
                0f,
                input.y
            );

        transform.position +=
            movement *
            moveSpeed *
            Time.deltaTime;
    }

    private void ExecuteAction()
    {
        if (playerRenderer != null)
        {
            playerRenderer.material.color =
                Random.ColorHSV();
        }
    }
}