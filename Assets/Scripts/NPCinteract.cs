using UnityEngine;
using TMPro; // solo si usas TextMeshPro

public class NPCDialogo : MonoBehaviour
{
    [Header("Diálogo del NPC")]
    [TextArea(2, 5)]
    public string[] lineasDialogo;

    [Header("Referencias UI")]
    public GameObject mensajeInteractuar; // “Presiona X para hablar”
    public TMP_Text textoDialogo;         // Texto donde se muestra el diálogo

    private bool jugadorCerca = false;
    private bool enDialogo = false;
    private int indiceLinea = 0;

    void Start()
    {
        if (mensajeInteractuar != null)
            mensajeInteractuar.SetActive(false);

        if (textoDialogo != null)
            textoDialogo.gameObject.SetActive(false);
    }

    void Update()
    {
        if (jugadorCerca && Input.GetKeyDown(KeyCode.X))
        {
            if (!enDialogo)
                IniciarDialogo();
            else
                MostrarSiguienteLinea();
        }
    }

    void IniciarDialogo()
    {
        enDialogo = true;
        indiceLinea = 0;

        mensajeInteractuar.SetActive(false);
        textoDialogo.gameObject.SetActive(true);
        textoDialogo.text = lineasDialogo[indiceLinea];
    }

    void MostrarSiguienteLinea()
    {
        indiceLinea++;

        if (indiceLinea < lineasDialogo.Length)
        {
            textoDialogo.text = lineasDialogo[indiceLinea];
        }
        else
        {
            TerminarDialogo();
        }
    }

    void TerminarDialogo()
    {
        enDialogo = false;
        textoDialogo.gameObject.SetActive(false);
        mensajeInteractuar.SetActive(true); // Vuelve a aparecer el mensaje para hablar
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
            mensajeInteractuar.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
            mensajeInteractuar.SetActive(false);
            textoDialogo.gameObject.SetActive(false);
            enDialogo = false;
        }
    }
}
