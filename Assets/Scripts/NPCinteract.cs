using UnityEngine;
using TMPro; 

public class NPCinteracts : MonoBehaviour
{
    public GameObject textoUI; 

    private void Start()
    {
        textoUI.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            textoUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            textoUI.SetActive(false);
        }
    }
}
