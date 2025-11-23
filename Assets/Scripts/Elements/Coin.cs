using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Coin : MonoBehaviour
{
    [Header("Configuración")]
    [Min(1)] public int valor = 1;
    [Tooltip("Si está activo, la moneda se destruye al recogerla. Si no, simplemente se desactiva.")]
    public bool destruirAlRecoger = true;
    [Min(0f)] public float retardoDestruccion = 0.05f;

    [Header("Efectos opcionales")]
    public GameObject efectoRecogida;
    public AudioClip sonidoRecogida;

    bool recogida;

    void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col != null)
            col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (recogida || !other.CompareTag("Player")) return;

        recogida = true;

        if (CoinCounter.Instance != null)
        {
            CoinCounter.Instance.AgregarMonedas(Mathf.Max(1, valor));
        }
        else
        {
            Debug.LogWarning("CoinCounter no encontrado en la escena. Añade uno para contar monedas.");
        }

        if (efectoRecogida != null)
            Instantiate(efectoRecogida, transform.position, Quaternion.identity);

        if (sonidoRecogida != null)
            AudioSource.PlayClipAtPoint(sonidoRecogida, transform.position);

        if (destruirAlRecoger)
            Destroy(gameObject, retardoDestruccion);
        else
            gameObject.SetActive(false);
    }
}