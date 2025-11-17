using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    [Header("Dash")]
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    private bool isDashing = false;
    private bool canDash = true;

    [Header("Doble Salto")]
    public int maxJumps = 2; // número total de saltos (2 = doble salto)
    private int jumpCount = 0;

    private Rigidbody2D rb;
    private bool isGrounded = false;
    private bool facingRight = true;
    private bool isTouchingCeiling = false;

    [Header("Animación")]
    private Animator animator;

    // --- ADD: Wall fix variables ---
    [Header("Wall Fix Settings")]
    public LayerMask wallMask;
    public float wallCheckDistance = 0.3f;
    public float wallPushForce = 3f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDashing) return;

        // --- Movimiento horizontal ---
        float moveInput = 0f;
        if (Input.GetKey(KeyCode.A)) moveInput = -1f;
        if (Input.GetKey(KeyCode.D)) moveInput = 1f;

        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // --- Salto y doble salto ---
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumps)
            if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumps && !isTouchingCeiling)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f); // reinicia la velocidad vertical para consistencia
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                jumpCount++;
            }

        // --- Flip del sprite ---
        if (moveInput > 0 && !facingRight) Flip();
        else if (moveInput < 0 && facingRight) Flip();

        // --- Dash ---
        if (Input.GetKeyDown(KeyCode.F) && canDash)
            if (Input.GetKeyDown(KeyCode.F) && canDash && !isTouchingCeiling)
            {
                StartCoroutine(DoDash());
            }

        // --- Wall Stuck Fix ---
        CheckWallStuck(moveInput);

        // --- Animaciones ---
        float speed = Mathf.Abs(moveInput);
        animator.SetFloat("Speed", speed);
        animator.SetBool("isJumping", !isGrounded);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isDashing", isDashing);
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            isGrounded = true;
            jumpCount = 0; // Reinicia los saltos al tocar el suelo
        }

        if (IsCeilingCollision(collision))
        {
            isTouchingCeiling = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            isGrounded = false;
        }

        isTouchingCeiling = false;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (IsCeilingCollision(collision))
        {
            isTouchingCeiling = true;
        }
    }

    private System.Collections.IEnumerator DoDash()
    {
        canDash = false;
        isDashing = true;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;

        float dashDirection = facingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(dashDirection * dashSpeed, 0f);

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    // --- FIXED: Wall Fix Method ---
    private void CheckWallStuck(float moveInput)
    {
        // Cast ray to detect wall in front
        Vector2 direction = facingRight ? Vector2.right : Vector2.left;
        RaycastHit2D hitWall = Physics2D.Raycast(transform.position, direction, wallCheckDistance, wallMask);

        if (hitWall.collider != null && Mathf.Abs(moveInput) > 0.1f)
        {
            // Small push away from wall to prevent sticking
            rb.AddForce(-direction * wallPushForce, ForceMode2D.Force);
        }
    }

    private bool IsCeilingCollision(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y < -0.5f)
            {
                return true;
            }
        }

        return false;
    }
}

