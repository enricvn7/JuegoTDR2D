using UnityEngine;
using UnityEngine.UI;

public class UIVidas : MonoBehaviour
{
    [Header("Referencias UI")]
    public Image corazonesImagen;  // Imagen del canvas
    public Sprite[] spritesCorazones; // 0 = 3 corazones, 1 = 2, 2 = 1, 3 = 0 (opcional)

    // Este método será llamado automáticamente por el evento del script Vida
    public void ActualizarVidas(int vidaActual, int vidaMax)
    {
        int index = Mathf.Clamp(vidaMax - vidaActual, 0, spritesCorazones.Length - 1);
        corazonesImagen.sprite = spritesCorazones[index];
    }
}
