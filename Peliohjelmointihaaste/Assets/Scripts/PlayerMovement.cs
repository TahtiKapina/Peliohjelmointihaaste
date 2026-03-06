using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Units per second")]
    [SerializeField] private float speed = 5f;

    [Tooltip("Ignore small controller/joystick inputs")]
    [SerializeField] private float deadZone = 0.2f;

    [Tooltip("When false movement is constrained to cardinal directions (no diagonal).")]
    [SerializeField] private bool allowDiagonal = false;

    private Rigidbody2D rb;
    private Vector2 inputAxis;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // Ensure no gravity so the player 'floats'
        rb.gravityScale = 0f;
        // Prevent rotation from collisions
        rb.freezeRotation = true;
    }

    private void Update()
    {
        // Read controller / keyboard axes (works with Unity's old Input Manager)
        float rawX = Input.GetAxisRaw("Horizontal");
        float rawY = Input.GetAxisRaw("Vertical");

        // Apply deadzone to avoid stick drift
        rawX = Mathf.Abs(rawX) < deadZone ? 0f : rawX;
        rawY = Mathf.Abs(rawY) < deadZone ? 0f : rawY;

        inputAxis = new Vector2(rawX, rawY);

        if (!allowDiagonal && inputAxis != Vector2.zero)
        {
            // Keep only the stronger axis to enforce up/down/left/right movement
            if (Mathf.Abs(inputAxis.x) > Mathf.Abs(inputAxis.y))
            {
                inputAxis.y = 0f;
                inputAxis.x = Mathf.Sign(inputAxis.x);
            }
            else
            {
                inputAxis.x = 0f;
                inputAxis.y = Mathf.Sign(inputAxis.y);
            }
        }

        // Normalize to avoid faster diagonal movement when diagonal allowed
        if (inputAxis.sqrMagnitude > 1f)
            inputAxis.Normalize();
    }

    private void FixedUpdate()
    {
        // Use MovePosition for stable, gravity-free movement
        Vector2 displacement = inputAxis * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + displacement);
    }

    // Helpers
    public void SetSpeed(float newSpeed) => speed = newSpeed;
    public void SetAllowDiagonal(bool allowed) => allowDiagonal = allowed;
}
