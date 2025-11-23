using UnityEngine;
using UnityEngine.Events;

public class CoinCounter : MonoBehaviour
{
    public static CoinCounter Instance { get; private set; }

    [Header("Monedas")]
    [Min(0)] public int monedasIniciales = 0;
    [Min(0)] public int monedasActuales;

    [Header("Eventos")]
    public UnityEvent<int> onMonedasCambiaron;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        monedasActuales = Mathf.Max(monedasIniciales, 0);
        NotificarCambio();
    }

    public void AgregarMonedas(int cantidad)
    {
        if (cantidad <= 0) return;

        monedasActuales += cantidad;
        NotificarCambio();
    }

    public bool GastarMonedas(int cantidad)
    {
        if (cantidad <= 0) return true;
        if (monedasActuales < cantidad) return false;

        monedasActuales -= cantidad;
        NotificarCambio();
        return true;
    }

    public void ReiniciarMonedas(int valor = 0)
    {
        monedasActuales = Mathf.Max(valor, 0);
        NotificarCambio();
    }

    public int ObtenerMonedas() => monedasActuales;

    void NotificarCambio() => onMonedasCambiaron?.Invoke(monedasActuales);
}