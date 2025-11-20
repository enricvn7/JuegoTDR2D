using System.Collections;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Inputs & timings")]
    public KeyCode attackKey = KeyCode.Return;
    public float attackDuration = 0.2f;   // tiempo que el hitbox permanece activo
    public float attackCooldown = 0.4f;   // tiempo entre ataques

    [Header("References")]
    [SerializeField] private Collider2D attackHitbox; // asignar el collider del hitbox

    private bool canAttack = true;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (attackHitbox != null)
            attackHitbox.enabled = false; // empezar desactivado
    }

    private void Update()
    {
        if (Input.GetKeyDown(attackKey))
        {
            Debug.Log("[PlayerCombat] Tecla de ataque pulsada");

            if (canAttack)
            {
                StartCoroutine(AttackCoroutine());
            }
            else
            {
                Debug.Log("[PlayerCombat] No puedo atacar, en cooldown");
            }
        }
    }

    private IEnumerator AttackCoroutine()
    {
        canAttack = false;
        Debug.Log("[PlayerCombat] Comienza ataque");

        if (animator != null)
        {
            animator.SetBool("isAttacking", true);
            Debug.Log("[PlayerCombat] isAttacking = true");
        }
        else
        {
            Debug.LogWarning("[PlayerCombat] NO hay Animator en el Player");
        }

        if (attackHitbox != null)
        {
            attackHitbox.enabled = true;
            Debug.Log("[PlayerCombat] Hitbox ACTIVADA");
        }
        else
        {
            Debug.LogWarning("[PlayerCombat] attackHitbox es NULL en el inspector");
        }

        yield return new WaitForSeconds(attackDuration);

        if (attackHitbox != null)
        {
            attackHitbox.enabled = false;
            Debug.Log("[PlayerCombat] Hitbox DESACTIVADA");
        }

        if (animator != null)
        {
            animator.SetBool("isAttacking", false);
            Debug.Log("[PlayerCombat] isAttacking = false");
        }

        // cooldown
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
        Debug.Log("[PlayerCombat] Fin de ataque, listo para otro");
    }

    // Opcional: métodos para usar con Animation Events (ver sección más abajo)
    public void EnableHitbox() {
        if (attackHitbox != null) attackHitbox.enabled = true;
    }

    public void DisableHitbox() {
        if (attackHitbox != null) attackHitbox.enabled = false;
    }
}
