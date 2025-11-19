using UnityEngine;

public class PlayerAttackHitbox : MonoBehaviour
{
    public int damage = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("[Hitbox] Colisión con: " + other.name);

        if (!other.CompareTag("Enemy")) return;

        Debug.Log("[Hitbox] Es Enemy, intento hacer daño");

        EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
        if (enemyHealth == null)
        {
            Debug.LogWarning("[Hitbox] Enemy SIN EnemyHealth: " + other.name);
            return;
        }

        enemyHealth.TakeDamage(damage);
    }
}
