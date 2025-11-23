using TMPro;
using UnityEngine;

public class CoinUI : MonoBehaviour
{
    [Header("Referencias")]
    public TMP_Text textoMonedas;

    void Awake()
    {
        // Si no se asigna desde el inspector, intenta coger el TMP_Text del mismo objeto
        if (textoMonedas == null)
            textoMonedas = GetComponent<TMP_Text>();
    }

    void Update()
    {
        if (textoMonedas == null) return;

        if (CoinCounter.Instance != null)
        {
            textoMonedas.text = CoinCounter.Instance.monedasActuales.ToString();
            // O: CoinCounter.Instance.ObtenerMonedas().ToString();
        }
        else
        {
            textoMonedas.text = "0";
        }
    }
}
