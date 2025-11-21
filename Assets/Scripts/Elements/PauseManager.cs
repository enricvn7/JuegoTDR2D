using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject pausePanel;           // Panel del menú de pausa (desactivado en inicio)
    public Button firstSelectedButton;      // Botón que se selecciona al pausar (opcional)

    bool isPaused = false;

    void Start()
    {
        // Aseguramos estado inicial
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }

    public void TogglePause()
    {
        if (!isPaused)
            PauseGame();
        else
            ResumeGame();
    }

    public void PauseGame()
    {
        isPaused = true;

        // Activar UI primero para que esté visible
        if (pausePanel != null)
            pausePanel.SetActive(true);

        // Seleccionar un botón para navegación con teclado/gamepad
        if (firstSelectedButton != null && EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null); // reset
            EventSystem.current.SetSelectedGameObject(firstSelectedButton.gameObject);
        }

        // Finalmente pausar el juego
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isPaused = false;

        // Reactivar tiempo primero
        Time.timeScale = 1f;

        // Desactivar panel
        if (pausePanel != null)
            pausePanel.SetActive(false);

        // Deseleccionar para evitar inputs residuales
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }

    // Métodos públicos para los botones (deben ser públicos)
    public void ResumeButton() => ResumeGame();

    public void LoadMenu(string sceneName)
    {
        // Asegúrate de que el tiempo está activo antes de cambiar de escena
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
