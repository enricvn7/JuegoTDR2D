using UnityEngine;

public class PauseToggle : MonoBehaviour
{
    public GameObject pausePanel; // Tu panel del Canvas

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Si está activo lo desactiva, si está inactivo lo activa
            pausePanel.SetActive(!pausePanel.activeSelf);
        }
    }
}
