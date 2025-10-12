using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Si usas TextMeshPro

public class PortalInteract : MonoBehaviour
{
    public string sceneToLoad;          // Nombre de la escena a cargar
    public GameObject portalPrompt;     // Referencia al texto UI
    private bool playerInRange = false; // Si el jugador está cerca del portal

    void Update()
    {
        // Mostrar el mensaje si el jugador está cerca
        if (playerInRange)
        {
            portalPrompt.SetActive(true);

            // Cambiar de escena al pulsar X
            if (Input.GetKeyDown(KeyCode.X))
            {
                SceneManager.LoadScene(sceneToLoad);
            }
        }
        else
        {
            portalPrompt.SetActive(false); // Oculta el mensaje si no está cerca
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}