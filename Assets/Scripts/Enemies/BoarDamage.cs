using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class BoarDamage : MonoBehaviour
{
    [Header("Daño al jugador")]
    public int damage = 1;
    public float attackCooldown = 0.8f;
    public LayerMask targetLayers;

    float nextAllowedDamageTime;

    void Awake()
    {
        if (targetLayers == 0)
            targetLayers = LayerMask.GetMask("Player");
    }

    void OnTriggerEnter2D(Collider2D other) => TryDamage(other);
    void OnTriggerStay2D(Collider2D other) => TryDamage(other);
    void OnCollisionEnter2D(Collision2D collision) => TryDamage(collision.collider);
    void OnCollisionStay2D(Collision2D collision) => TryDamage(collision.collider);

    void TryDamage(Collider2D other)
    {
        if (other == null || Time.time < nextAllowedDamageTime)
            return;

        if ((targetLayers.value & (1 << other.gameObject.layer)) == 0)
            return;

        var vida = GetVidaFromCollider(other);
        if (vida == null)
            return;

        vida.RecibirDanio(damage);
        nextAllowedDamageTime = Time.time + attackCooldown;
    }

    static Vida GetVidaFromCollider(Collider2D other)
    {
        if (other == null)
            return null;

        var vida = other.GetComponent<Vida>();
        if (vida != null)
            return vida;

        vida = other.GetComponentInParent<Vida>();
        if (vida != null)
            return vida;

        var rb = other.attachedRigidbody;
        return rb != null ? rb.GetComponent<Vida>() : null;
    }

    
}