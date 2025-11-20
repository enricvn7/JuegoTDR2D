using UnityEngine;

public class HitboxDamage : MonoBehaviour
{
    public int damage = 20;
    public string enemyTag = "Enemy"; // ajustar si usas Tag o Layer

    private Transform playerTransform;

    private void Start()
    {
        // asumimos que el objeto padre es el jugador; ajusta si no es así
        playerTransform = transform.root;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Debug para ver qué colisiona
        Debug.Log("[HitboxDamage] Colision: " + other.name);

        if (!other.CompareTag(enemyTag)) return;

        // Intentar encontrar componente en el objeto o en un padre
        var enemy = other.GetComponent<EnemyHealth>() ?? other.GetComponentInParent<EnemyHealth>();
        if (enemy == null)
        {
            Debug.LogWarning("[HitboxDamage] No se encontró EnemyHealth en " + other.name);
            return;
        }

        // Calculamos la dirección del knockback: desde el player hacia el enemigo
        Vector2 knockbackDir = (other.transform.position - playerTransform.position).normalized;

        enemy.TakeDamage(damage, knockbackDir);
        Debug.Log("[HitboxDamage] Aplicado daño a " + other.name);
    }
}

