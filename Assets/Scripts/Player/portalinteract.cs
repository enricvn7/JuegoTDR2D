using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Si usas TextMeshPro

public class Portal : MonoBehaviour
{
    [Header("Configuración del portal")]
    public string nombreEscenaDestino = "Game"; // Nombre de la escena a cargar
    public GameObject mensajeInteractuar;       // Texto "Presiona X para jugar"

    private bool jugadorCerca = false;

    void Start()
    {
        if (mensajeInteractuar != null)
            mensajeInteractuar.SetActive(false);
    }

    void Update()
    {
        if (jugadorCerca && Input.GetKeyDown(KeyCode.X))
        {
            CambiarEscena();
        }
    }

    void CambiarEscena()
    {
        // Carga la nueva escena
        SceneManager.LoadScene(nombreEscenaDestino);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
            if (mensajeInteractuar != null)
                mensajeInteractuar.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
            if (mensajeInteractuar != null)
                mensajeInteractuar.SetActive(false);
        }
    }
}
