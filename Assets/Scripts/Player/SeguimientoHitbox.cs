using UnityEngine;

public class FollowPlayerDirection : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(1f, 0f, 0f); // valor base

    private PlayerMovement playerMovement; // ejemplo: tu script que controla la dirección

    void Start()
    {
        playerMovement = player.GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (player != null)
        {
            // Si el Player mira derecha, usamos offset normal
            // Si mira izquierda, invertimos X
            float dir = playerMovement.facingRight ? 1f : -1f;
            transform.position = player.position + new Vector3(offset.x * dir, offset.y, offset.z);
        }
    }
}
