using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite spriteActivo;
    public Sprite spriteDesactivado;

    [Header("Estado")]
    public bool isActive = false;

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        ActualizarSprite();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerRespawn pr = collision.GetComponent<PlayerRespawn>();

            if (pr != null)
            {
                pr.SetRespawnPoint(transform);
                ActivarCheckpoint();
            }
        }
    }

    public void ActivarCheckpoint()
    {
        // Desactivar todos los demás checkpoints
        foreach (var cp in FindObjectsOfType<Checkpoint>())
        {
            cp.isActive = false;
            cp.ActualizarSprite();
        }

        // Activar este
        isActive = true;
        ActualizarSprite();
    }

    private void ActualizarSprite()
    {
        if (sr != null)
        {
            sr.sprite = isActive ? spriteActivo : spriteDesactivado;
        }
    }
}
