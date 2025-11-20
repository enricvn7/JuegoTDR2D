using UnityEngine;

[RequireComponent(typeof(Vida))]
public class PlayerRespawn : MonoBehaviour
{
    [Header("Punto de respawn")]
    public Transform respawnPoint;   // Asigna este desde el inspector

    [Header("Efectos opcionales")]
    public float delayRespawn = 1.5f; // Tiempo que tarda en reaparecer (opcional)
    public GameObject deathEffect;   // Efecto visual al morir (opcional)

    Vida vida;
    Rigidbody2D rb;
    Collider2D col;
    SpriteRenderer sr;

    void Awake()
    {
        vida = GetComponent<Vida>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();

        // Suscribirse al evento de muerte del script Vida
        vida.onMuerte.AddListener(OnPlayerDeath);
    }

    void OnPlayerDeath()
    {
        // Efecto visual opcional
        if (deathEffect != null)
            Instantiate(deathEffect, transform.position, Quaternion.identity);

        // Desactivar movimiento y colisión temporalmente
        if (rb != null) rb.simulated = false;
        if (col != null) col.enabled = false;
        if (sr != null) sr.enabled = false;

        // Esperar y luego reaparecer
        Invoke(nameof(Respawn), delayRespawn);
    }

    void Respawn()
    {
        if (respawnPoint == null)
        {
            Debug.LogWarning("No se ha asignado un punto de respawn al jugador.");
            return;
        }

        // Reubicar al jugador
        transform.position = respawnPoint.position;

        // Revivir al jugador
        vida.Revivir(); // Esto lo tienes ya en tu script Vida

        // Reactivar todo
        if (rb != null) rb.simulated = true;
        if (col != null) col.enabled = true;
        if (sr != null) sr.enabled = true;
    }
}
