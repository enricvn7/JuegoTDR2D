using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class BoarAI : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;          // Se puede auto-buscar por tag "Player"
    Vida playerVida;
    Rigidbody2D playerRb;
    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sr;

    [Header("Movimiento")]
    public float walkSpeed = 1.5f;    // Velocidad patrulla (Walk)
    public float runSpeed  = 4f;      // Velocidad persecución (Run)
    public float patrolDistance = 3f; // Distancia desde el punto inicial hacia cada lado

    [Header("Detección")]
    public float detectionRadius = 5f;
    public LayerMask lineOfSightMask; // Capas que bloquean visión (paredes, suelo...)

    [Header("Ataque")]
    public int damage = 1;
    public float attackCooldown = 0.75f;
    public float contactDamageRadius = 0.4f; // radio alrededor del boar para golpear
    float nextAttackTime = 0f;

    [Header("Empuje al impactar")]
    public float knockbackForceToPlayer = 6f;
    public float knockbackForceToBoar = 2f;

    // Estado interno
    Vector2 startPos;
    bool movingRight = true;
    bool isChasing = false;

    void Awake()
    {
        rb   = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr   = GetComponent<SpriteRenderer>();

        startPos = rb.position;

        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        if (player != null)
            CachePlayerComponents();
    }

    void FixedUpdate()
    {
        float speedAbs = 0f;

        if (player == null)
        {
            isChasing = false;
            Patrol();
            speedAbs = Mathf.Abs(rb.linearVelocity.x);
            UpdateAnimator(speedAbs);
            return;
        }

        bool canSeePlayer = InDetectionRange() && HasLineOfSight();
        isChasing = canSeePlayer;

        if (isChasing)
            ChasePlayer();
        else
            Patrol();

        // Intentar hacer daño si está cerca
        TryDamagePlayer();

        speedAbs = Mathf.Abs(rb.linearVelocity.x);
        UpdateAnimator(speedAbs);
    }

    // ------------ MOVIMIENTO ------------

    void Patrol()
    {
        float targetX = movingRight
            ? startPos.x + patrolDistance
            : startPos.x - patrolDistance;

        if (movingRight && rb.position.x >= targetX)
            movingRight = false;
        else if (!movingRight && rb.position.x <= targetX)
            movingRight = true;

        float dirX = movingRight ? 1f : -1f;
        MoveHorizontal(dirX, walkSpeed);
    }

    void ChasePlayer()
    {
        float dirX = Mathf.Sign(player.position.x - rb.position.x);
        if (dirX == 0) dirX = 1f;

        MoveHorizontal(dirX, runSpeed);
    }

    void MoveHorizontal(float dirX, float speed)
    {
        rb.linearVelocity = new Vector2(dirX * speed, rb.linearVelocity.y);

        if (sr != null)
            sr.flipX = (dirX < 0f);
    }

    // ------------ DETECCIÓN ------------

    bool InDetectionRange()
    {
        return Vector2.Distance(transform.position, player.position) <= detectionRadius;
    }

    bool HasLineOfSight()
    {
        if (lineOfSightMask == 0) return true; // si no se configuran capas, siempre ve

        Vector2 origin = transform.position;
        Vector2 dir    = (Vector2)player.position - origin;
        float dist     = dir.magnitude;

        var hit = Physics2D.Raycast(origin, dir.normalized, dist, lineOfSightMask);
        return hit.collider == null;
    }

    // ------------ DAÑO Y KNOCKBACK ------------

    void TryDamagePlayer()
    {
        if (Time.time < nextAttackTime || player == null) return;

        // Buscar un collider del Player cerca del Boar
        Collider2D hit = Physics2D.OverlapCircle(
            transform.position,
            contactDamageRadius,
            LayerMask.GetMask("Player", "Default") // ajusta si usas otra layer
        );

        if (hit != null)
        {
            var vida = GetVidaFromCollider(hit);
            if (vida != null && vida == playerVida)
            {
                nextAttackTime = Time.time + attackCooldown;
                vida.RecibirDanio(damage);
            }
        }
    }

    // Si prefieres usar Trigger, puedes añadir esto
    void OnTriggerStay2D(Collider2D other)
    {
        if (Time.time < nextAttackTime) return;

        var vida = GetVidaFromCollider(other);
        if (vida != null && vida == playerVida)
        {
            nextAttackTime = Time.time + attackCooldown;
            vida.RecibirDanio(damage);
        }
    }

    Vida GetVidaFromCollider(Collider2D col)
    {
        if (col == null) return null;

        var vida = col.GetComponentInParent<Vida>();

        if (vida == null && col.attachedRigidbody != null)
            vida = col.attachedRigidbody.GetComponent<Vida>();

        if (vida == null && player != null && col.transform == player)
            vida = player.GetComponent<Vida>();

        if (vida != null && vida.transform == player)
        {
            if (playerVida == null)
                playerVida = vida;

            if (playerRb == null)
                playerRb = player.GetComponent<Rigidbody2D>();
        }

        return vida;
    }

    void CachePlayerComponents()
    {
        if (player == null) return;

        playerVida = player.GetComponent<Vida>();
        playerRb   = player.GetComponent<Rigidbody2D>();
    }

    
    // ------------ ANIMACIÓN ------------

    void UpdateAnimator(float speedAbs)
    {
        if (anim == null) return;

        anim.SetFloat("Speed", speedAbs);
        anim.SetBool("IsChasing", isChasing);
    }

    // ------------ GIZMOS ------------

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, contactDamageRadius);

        if (Application.isPlaying)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(
                new Vector3(startPos.x - patrolDistance, startPos.y, 0f),
                new Vector3(startPos.x + patrolDistance, startPos.y, 0f)
            );
        }
    }
}
