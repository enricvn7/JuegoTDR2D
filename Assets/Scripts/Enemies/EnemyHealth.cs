using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int vidaMax = 10;
    public int vidaActual;

    public Rigidbody2D rb; // Rigidbody del enemigo
    public float knockbackForce = 5f;

    void Awake()
    {
        vidaActual = vidaMax;
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(int damage, Vector2 knockbackDir)
    {
        // Restar vida
        vidaActual -= damage;

        // Revisar muerte
        if (vidaActual <= 0)
        {
            Destroy(gameObject); // o cualquier lógica de muerte
        }

        // Aplicar knockback
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero; // resetear velocidad antes del golpe
            rb.AddForce(knockbackDir.normalized * knockbackForce, ForceMode2D.Impulse);
        }
    }
}
