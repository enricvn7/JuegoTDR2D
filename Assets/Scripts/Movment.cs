using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;   // Velocidad de movimiento horizontal
    public float jumpForce = 7f;   // Fuerza del salto
    private Rigidbody2D rb;
    private bool isGrounded = false;
    private bool facingRight = true; //saber hacia donde mira el jugador

    [Header("Animacion")]
    private Animator animator;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        
        // --- Movimiento horizontal con A y D ---
        float moveInput = 0f;
        if (Input.GetKey(KeyCode.A)) moveInput = -1f;
        if (Input.GetKey(KeyCode.D)) moveInput = 1f;

        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // --- Salto con espacio ---
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        // --- Flip del sprite ---
        if (moveInput > 0 && !facingRight)
        {
            Flip();
        }
        else if (moveInput < 0 && facingRight)
        {
            Flip();
        }

        //Animacion
        float speed = Mathf.Abs(moveInput);
        animator.SetFloat("Speed", speed); // correr o idle

        animator.SetBool("isJumping", !isGrounded); // saltando o en el suelo
        animator.SetBool("isGrounded", isGrounded);


    }

    // Dibuja el área de detección en el editor
    void Flip()
    {
        facingRight = !facingRight;
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }
        
    // Detectar si toca el suelo
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            isGrounded = false;
        }
    }
    

    
}
