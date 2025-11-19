using System.Collections;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public KeyCode attackKey = KeyCode.Return;
    public float attackDuration = 0.2f;
    public float attackCooldown = 0.4f;

    [SerializeField] private GameObject attackHitbox;

    private bool canAttack = true;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        Debug.Log("[PlayerCombat] Awake. Animator = " + animator);
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
            attackHitbox.SetActive(true);
            Debug.Log("[PlayerCombat] Hitbox ACTIVADA");
        }
        else
        {
            Debug.LogWarning("[PlayerCombat] attackHitbox es NULL en el inspector");
        }

        yield return new WaitForSeconds(attackDuration);

        if (attackHitbox != null)
        {
            attackHitbox.SetActive(false);
            Debug.Log("[PlayerCombat] Hitbox DESACTIVADA");
        }

        if (animator != null)
        {
            animator.SetBool("isAttacking", false);
            Debug.Log("[PlayerCombat] isAttacking = false");
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
        Debug.Log("[PlayerCombat] Fin de ataque, listo para otro");
    }
}
