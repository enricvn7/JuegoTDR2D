using UnityEngine;
using UnityEngine.SceneManagement;

public class Movment : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    [Header("Configuración del portal")]
    public string nombreEscenaDestino = "Game";
    public GameObject mensajeInteractuar;

    [Header("Dash")]
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    private bool isDashing = false;
    private bool canDash = true;

    [Header("Doble Salto")]
    public int maxJumps = 2;
    private int jumpCount = 0;

    private Rigidbody2D rb;
    private bool facingRight = true;

    [Header("Animación")]
    private Animator animator;

    [Header("Wall Fix Settings")]
    public LayerMask wallMask;
    public float wallCheckDistance = 0.3f;
    public float wallPushForce = 3f;
    private bool jugadorCerca = false;

    [Header("GroundCheck")]
    public GroundCheck groundCheck; // referencia al objeto hijo

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        if (mensajeInteractuar != null)
            mensajeInteractuar.SetActive(false);
    }

    void Update()
    {
        if (isDashing) return;

        float moveInput = 0f;
        if (Input.GetKey(KeyCode.A)) moveInput = -1f;
        if (Input.GetKey(KeyCode.D)) moveInput = 1f;

        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // --- Salto y doble salto ---
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumps && groundCheck.isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumpCount++;
        }

        if (jugadorCerca && Input.GetKeyDown(KeyCode.X))
        {
            CambiarEscena();
        }

        // --- Flip del sprite ---
        if (moveInput > 0 && !facingRight) Flip();
        else if (moveInput < 0 && facingRight) Flip();

        // --- Dash ---
        if (Input.GetKeyDown(KeyCode.F) && canDash)
        {
            StartCoroutine(DoDash());
        }

        // --- Wall Stuck Fix ---
        CheckWallStuck(moveInput);

        // --- Animaciones ---
        animator.SetFloat("Speed", Mathf.Abs(moveInput));
        animator.SetBool("isJumping", !groundCheck.isGrounded);
        animator.SetBool("isGrounded", groundCheck.isGrounded);
        animator.SetBool("isDashing", isDashing);
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }

    void CambiarEscena()
    {
        SceneManager.LoadScene(nombreEscenaDestino);
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

    private void CheckWallStuck(float moveInput)
    {
        Vector2 direction = facingRight ? Vector2.right : Vector2.left;
        RaycastHit2D hitWall = Physics2D.Raycast(transform.position, direction, wallCheckDistance, wallMask);

        if (hitWall.collider != null && Mathf.Abs(moveInput) > 0.1f)
        {
            rb.AddForce(-direction * wallPushForce, ForceMode2D.Force);
            jugadorCerca = false;
            if (mensajeInteractuar != null)
                mensajeInteractuar.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
            if (mensajeInteractuar != null)
                mensajeInteractuar.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
            if (mensajeInteractuar != null)
                mensajeInteractuar.SetActive(false);
        }
    }
}
