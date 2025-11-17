using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BeeAI : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;
    Vida playerVida;                 
    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sr;

    [Header("Movimiento")]
    public float patrolSpeed = 1.5f;
    public float chaseSpeed = 3f;
    public float patrolRadius = 3f;          
    public float waypointChangeTime = 2.5f;  

    [Header("Detección")]
    public float detectionRadius = 5f;       
    public LayerMask lineOfSightMask;       

    [Header("Ataque")]
    public int damage = 1;
    public float attackCooldown = 0.75f;
    public float contactDamageRadius = 0.4f; 
    float nextAttackTime = 0f;

    Vector2 startPos;
    Vector2 patrolTarget;
    float nextWaypointTime = 0f;

    public float knockbackForce = 5f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr  = GetComponent<SpriteRenderer>();
        startPos = transform.position;

        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        if (player != null)
            playerVida = player.GetComponent<Vida>();

        ChooseNewPatrolPoint();
    }

    void Update()
    {
        if (rb.linearVelocity.x != 0) sr.flipX = rb.linearVelocity.x < 0;

        if (anim) anim.SetFloat("Speed", rb.linearVelocity.magnitude);
    }

    void FixedUpdate()
    {
        if (player == null)
        {
            Patrol();
            return;
        }

        bool canSeePlayer = InDetectionRange() && HasLineOfSight();

        if (canSeePlayer) Chase();
        else Patrol();

        TryDamagePlayer();
    }

    void Patrol()
    {
        if (Time.time >= nextWaypointTime || Vector2.Distance(transform.position, patrolTarget) < 0.2f)
            ChooseNewPatrolPoint();

        MoveTowards(patrolTarget, patrolSpeed);
    }

    void ChooseNewPatrolPoint()
    {
        nextWaypointTime = Time.time + waypointChangeTime;
        var offset = Random.insideUnitCircle * patrolRadius;
        patrolTarget = startPos + offset;
    }

    void Chase()
    {
        MoveTowards(player.position, chaseSpeed);
    }

    void MoveTowards(Vector2 target, float speed)
    {
        Vector2 pos = rb.position;
        Vector2 dir = (target - pos).normalized;
        rb.MovePosition(pos + dir * speed * Time.fixedDeltaTime);
    }

    bool InDetectionRange()
    {
        return Vector2.Distance(transform.position, player.position) <= detectionRadius;
    }

    bool HasLineOfSight()
    {
        if (lineOfSightMask == 0) return true; 
        Vector2 dir = (player.position - transform.position);
        var hit = Physics2D.Raycast(transform.position, dir.normalized, dir.magnitude, lineOfSightMask);
        return hit.collider == null;
    }

    void TryDamagePlayer()
    {
        if (Time.time < nextAttackTime || player == null) return;

        Collider2D hit = Physics2D.OverlapCircle(transform.position, contactDamageRadius,
            LayerMask.GetMask("Default", "Player")); 

        if (hit != null)
        {
            var vida = GetVidaFromCollider(hit);
            if (vida != null && vida == playerVida)
            {
                nextAttackTime = Time.time + attackCooldown;
                vida.RecibirDanio(damage);
                // KNOCKBACK
                var pm = player.GetComponent<PlayerMovement>();
                if (pm != null)
                {
                Vector2 knockDir = (player.position - transform.position).normalized;
                pm.ApplyKnockback(knockDir, knockbackForce);
                }

            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, contactDamageRadius);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(Application.isPlaying ? (Vector3)startPos : transform.position, patrolRadius);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (Time.time < nextAttackTime) return;

        var vida = GetVidaFromCollider(other);
        if (vida != null && vida == playerVida)
        {
            vida.RecibirDanio(damage);
            nextAttackTime = Time.time + attackCooldown;
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

        if (playerVida == null && vida != null && vida.transform == player)
            playerVida = vida;

        return vida;
    }
    
}
