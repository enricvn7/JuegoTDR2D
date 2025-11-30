using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BossArenaWalls : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;
    Rigidbody2D rb;

    [Header("Arena")]
    [Tooltip("Genera paredes alrededor de la arena cuando el jugador entra en rango del boss.")]
    public bool bloquearArenaAlEntrar = true;
    public float distanciaMaximaPersecucion = 12f;
    public Vector2 tamanoArena = new Vector2(30f, 16f);
    public float grosorMuros = 1f;
    [Tooltip("Capa que tendrán los muros (configúrala en el Inspector).")]
    public int capaMuros = 7;

    bool arenaBloqueada;
    readonly List<BoxCollider2D> murosArena = new();

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Si no hay player asignado en el inspector, lo buscamos por tag
        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    void Update()
    {
        VerificarArena();
    }

    void VerificarArena()
    {
        if (!bloquearArenaAlEntrar || arenaBloqueada || player == null)
            return;

        if (Vector2.Distance(rb.position, player.position) <= distanciaMaximaPersecucion)
            BloquearArena();
    }

    public void BloquearArena()
    {
        if (arenaBloqueada)
            return;

        arenaBloqueada = true;

        Vector2 centro = rb.position;
        float medioAncho = tamanoArena.x * 0.5f;
        float medioAlto = tamanoArena.y * 0.5f;
        float mitadGrosor = grosorMuros * 0.5f;

        // Arriba y abajo
        CrearMuro(new Vector2(centro.x, centro.y + medioAlto + mitadGrosor), new Vector2(tamanoArena.x + grosorMuros * 2f, grosorMuros));
        CrearMuro(new Vector2(centro.x, centro.y - medioAlto - mitadGrosor), new Vector2(tamanoArena.x + grosorMuros * 2f, grosorMuros));

        // Izquierda y derecha
        CrearMuro(new Vector2(centro.x - medioAncho - mitadGrosor, centro.y), new Vector2(grosorMuros, tamanoArena.y + grosorMuros * 2f));
        CrearMuro(new Vector2(centro.x + medioAncho + mitadGrosor, centro.y), new Vector2(grosorMuros, tamanoArena.y + grosorMuros * 2f));
    }

    public void LiberarArena()
    {
        if (murosArena.Count == 0)
            return;

        foreach (var muro in murosArena)
        {
            if (muro != null)
                Destroy(muro.gameObject);
        }

        murosArena.Clear();
        arenaBloqueada = false;
    }

    void OnDestroy()
    {
        // Por si el boss se destruye por código
        LiberarArena();
    }

    void CrearMuro(Vector2 posicion, Vector2 tamano)
    {
        var muroObj = new GameObject("BossArenaWall");
        // No lo parentamos al boss para que los colliders no se unan a su Rigidbody2D.
        // Así evitamos que el boss quede bloqueado dentro de su propia "pared".
        muroObj.transform.SetParent(null);
        muroObj.transform.position = posicion;

        var collider = muroObj.AddComponent<BoxCollider2D>();
        collider.size = tamano;
        collider.isTrigger = false;

        if (capaMuros >= 0 && capaMuros < 32)
            muroObj.layer = capaMuros;

        murosArena.Add(collider);
    }

    void OnDrawGizmosSelected()
    {
        // Dibuja el tamaño de la arena en la escena
        Gizmos.color = Color.cyan;
        Vector2 centro = Application.isPlaying && rb != null ? rb.position : (Vector2)transform.position;
        Gizmos.DrawWireCube(centro, tamanoArena);
    }
}
