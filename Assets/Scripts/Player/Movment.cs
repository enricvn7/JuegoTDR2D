using UnityEngine;
using System.Collections;
using System.Collections.Generic;


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

    [Header("Knockback")]
    public float knockbackRecoverTime = 0.15f;
    private bool isKnockbackActive = false;

    [Header("Doble Salto")]
    public int maxJumps = 2; // número total de saltos (2 = doble salto)
    private int jumpCount = 0;

    private Rigidbody2D rb;
    private bool isGrounded = false;
    public bool facingRight = true;
    private bool isTouchingCeiling = false;

    [Header("Animación")]
    private Animator animator;

    // --- ADD: Wall fix variables ---
    [Header("Wall Fix Settings")]
    public LayerMask wallMask;
    public float wallCheckDistance = 0.3f;
    public float wallPushForce = 3f;

    private bool infiniteJump = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {   

        if (isKnockbackActive)
        {
            UpdateAnimatorParameters(Mathf.Abs(rb.linearVelocity.x));
            return;
        }

        if (isDashing) return;

        // --- Movimiento horizontal ---
        float moveInput = 0f;
        if (Input.GetKey(KeyCode.A)) moveInput = -1f;
        if (Input.GetKey(KeyCode.D)) moveInput = 1f;

        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // --- Salto y doble salto ---
        if (Input.GetKeyDown(KeyCode.Space) && (jumpCount < maxJumps || infiniteJump) && !isTouchingCeiling)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f); // reinicia velocidad vertical
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            if (!infiniteJump)
                jumpCount++;
        }



        // --- Flip del sprite ---
        if (moveInput > 0 && !facingRight) Flip();
        else if (moveInput < 0 && facingRight) Flip();

        // --- Dash ---
        if (Input.GetKeyDown(KeyCode.F) && canDash && !isTouchingCeiling)
        {
            StartCoroutine(DoDash());
        }

        // --- Wall Stuck Fix ---
        CheckWallStuck(moveInput);

        // --- Animaciones ---
        float speed = Mathf.Abs(moveInput);
        UpdateAnimatorParameters(speed);
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
    public void ApplyKnockback(Vector2 direction, float force)
    {
        StartCoroutine(KnockbackCoroutine(direction, force));
    }

    private IEnumerator KnockbackCoroutine(Vector2 direction, float force)
    {
        // Cancelar dash si está activo (opcional)
        isDashing = false;

        isKnockbackActive = true;

        // Aplicar velocidad de knockback
        Vector2 knockDirection = direction.sqrMagnitude > 0f ? direction.normalized : Vector2.zero;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(new Vector2(knockDirection.x * force, knockDirection.y * force * 0.5f), ForceMode2D.Impulse);

        // Pequeño delay antes de devolver control
        yield return new WaitForSeconds(knockbackRecoverTime);

        isKnockbackActive = false;
    }

    public void StartInfiniteJump(float duration)
    {
        if (!infiniteJump)
            StartCoroutine(InfiniteJumpCoroutine(duration));
    }

    private System.Collections.IEnumerator InfiniteJumpCoroutine(float duration)
    {
        infiniteJump = true;
        yield return new WaitForSeconds(duration);
        infiniteJump = false;
    }

    private void UpdateAnimatorParameters(float speed)
    {
        animator.SetFloat("Speed", speed);
        animator.SetBool("isJumping", !isGrounded);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isDashing", isDashing);
    }

}

