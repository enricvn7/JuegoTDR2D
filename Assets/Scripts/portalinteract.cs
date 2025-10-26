using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;


public class Portal : MonoBehaviour
{
    [Header("Escena destino")]
    public string nombreEscena; // Nombre de la escena a cargar

    [Header("Texto opcional")]
    public GameObject textoUI; // (opcional) Texto "Presiona X para entrar"

    private bool jugadorCerca = false;

    void Start()
    {
        if (textoUI != null)
            textoUI.SetActive(false);
    }

    void Update()
    {
        if (jugadorCerca && Input.GetKeyDown(KeyCode.X))
    StartCoroutine(CambiarEscena());

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
            if (textoUI != null)
                textoUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
            if (textoUI != null)
                textoUI.SetActive(false);
        }
    }

    IEnumerator CambiarEscena()
{
    // Aquí podrías poner animación o fade
    yield return new WaitForSeconds(0.5f);
    SceneManager.LoadScene(nombreEscena);
}

}
