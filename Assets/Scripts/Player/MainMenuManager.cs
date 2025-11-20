using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject optionsMenu;

    void Start()
    {
        // Asegurar que al inicio el MainMenu esté activo y OptionsMenu oculto
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }

    public void OpenOptions()
    {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void BackToMain()
    {
        optionsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game!"); // En el editor no se cerrará, solo en el build
        Application.Quit();
    }
    public void StartGame(string Tutorial)
{
    SceneManager.LoadScene(Tutorial);
}
}
