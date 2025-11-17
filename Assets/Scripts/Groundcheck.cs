using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [Header("Detección de suelo")]
    public LayerMask groundLayer;
    public bool isGrounded { get; private set; } = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            isGrounded = false;
        }
    }
}

