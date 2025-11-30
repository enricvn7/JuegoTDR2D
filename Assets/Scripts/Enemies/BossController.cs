using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EnemyHealth))]
public class BossController : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;
    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sr;
    EnemyHealth health;
    Vida playerVida;

    [Header("Arena")]
    public BossArenaWalls arenaWalls;   // <--- NUEVO

    [Header("Fases")]
    [Range(0.1f, 1f)] public float porcentajeFase2 = 0.5f;
    [Tooltip("Factor que reduce los tiempos cuando entra a fase 2 (menos es más rápido).")]
    public float multiplicadorFase2 = 0.75f;
    bool enFase2;

    [Header("Ritmo de combate")]
    public float tiempoEntrePatrones = 1.25f;
    public float distanciaMaximaPersecucion = 12f;

    [Header("Knockback")]
    public float knockbackFuerza = 6f;
    public float knockbackProyectil = 6f;

    [Header("Daño por contacto")]
    public int danoContacto = 1;
    public float radioContacto = 0.7f;
    public LayerMask capasDanio = ~0;

    [Header("Ataque: Espada")]
    public Transform puntoGolpeEspada;
    public float radioGolpeEspada = 0.8f;
    public int danoEspada = 1;

    [Header("Ataque: Dash")]
    public float tiempoTelegraphDash = 0.35f;
    public float duracionDash = 0.45f;
    public float velocidadDash = 9f;

    [Header("Ataque: Salto con onda expansiva")]
    public float duracionSalto = 0.9f;
    public float alturaSalto = 2.5f;
    public float radioGolpeSalto = 1.2f;
    public int danoSalto = 2;

    [Header("Ataque: Ráfaga de proyectiles")]
    public BossProjectile proyectilPrefab;
    public Transform puntoDisparo;
    public int proyectilesPorRafaga = 5;
    public float anguloDispersion = 25f;
    public float tiempoEntreProyectiles = 0.15f;
    public float velocidadProyectil = 7f;
    public int danoProyectil = 1;

    bool ejecutandoPatron;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        health = GetComponent<EnemyHealth>();

        // Intentar coger automáticamente el componente de arena si está en el mismo GameObject
        if (arenaWalls == null)
            arenaWalls = GetComponent<BossArenaWalls>();

        // 1) Si no hay player asignado en el inspector, lo buscamos por tag
        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        // 2) Si hemos encontrado player, guardamos su Vida
        if (player != null)
        {
            playerVida = player.GetComponent<Vida>();
        }
    }

    void Start()
    {
        StartCoroutine(BossLoop());
    }

    void Update()
    {
        if (player != null && sr != null)
            sr.flipX = player.position.x < transform.position.x;

        ActualizarFase();
    }

    IEnumerator BossLoop()
    {
        yield return new WaitForSeconds(0.5f); // respiro al iniciar

        while (health == null || health.vidaActual > 0)
        {
            if (ejecutandoPatron)
            {
                yield return null;
                continue;
            }

            if (player == null || Vector2.Distance(rb.position, player.position) > distanciaMaximaPersecucion)
            {
                yield return null;
                continue;
            }

            float espera = tiempoEntrePatrones * (enFase2 ? multiplicadorFase2 : 1f);
            yield return new WaitForSeconds(espera);

            yield return EjecutarPatronAleatorio();
        }
    }

    IEnumerator EjecutarPatronAleatorio()
    {
        ejecutandoPatron = true;

        float tirada = Random.value;
        if (tirada < 0.4f)
            yield return PatronDash();
        else if (tirada < 0.75f)
            yield return PatronProyectiles();
        else
            yield return PatronSalto();

        ejecutandoPatron = false;
    }

    IEnumerator PatronDash()
    {
        if (anim) anim.SetTrigger("Dash");
        yield return new WaitForSeconds(tiempoTelegraphDash * (enFase2 ? multiplicadorFase2 : 1f));

        Vector2 objetivo = player != null
            ? (Vector2)player.position
            : rb.position + Vector2.right * (sr != null && sr.flipX ? -1f : 1f);

        Vector2 direccion = (objetivo - rb.position).normalized;
        if (direccion == Vector2.zero)
            direccion = Vector2.right;

        float fin = Time.time + duracionDash;

        while (Time.time < fin)
        {
            rb.MovePosition(rb.position + direccion * velocidadDash * Time.fixedDeltaTime);
            IntentarDaniarJugador(danoContacto, radioContacto, rb.position);
            yield return new WaitForFixedUpdate();
        }

        rb.linearVelocity = Vector2.zero;
    }

    IEnumerator PatronSalto()
    {
        if (anim) anim.SetTrigger("Jump");

        Vector2 origen = rb.position;
        Vector2 destino = player != null ? (Vector2)player.position : origen;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duracionSalto;
            float altura = Mathf.Sin(Mathf.PI * t) * alturaSalto;
            Vector2 pos = Vector2.Lerp(origen, destino, t) + Vector2.up * altura;
            rb.MovePosition(pos);
            yield return null;
        }

        
    }

    IEnumerator PatronProyectiles()
    {
        if (proyectilPrefab == null)
        {
            ejecutandoPatron = false;
            yield break;
        }

        if (anim) anim.SetTrigger("Shoot");

        Vector2 origen = puntoDisparo != null ? (Vector2)puntoDisparo.position : rb.position;
        Vector2 direccionBase = player != null
            ? ((Vector2)player.position - origen).normalized
            : Vector2.right * (sr != null && sr.flipX ? -1f : 1f);

        for (int i = 0; i < proyectilesPorRafaga; i++)
        {
            float progreso = proyectilesPorRafaga > 1 ? (float)i / (proyectilesPorRafaga - 1) : 0.5f;
            float angulo = Mathf.Lerp(-anguloDispersion, anguloDispersion, progreso);
            Vector2 dir = Quaternion.Euler(0f, 0f, angulo) * direccionBase;

            LanzarProyectil(origen, dir);
            yield return new WaitForSeconds(tiempoEntreProyectiles * (enFase2 ? multiplicadorFase2 : 1f));
        }
    }

    void LanzarProyectil(Vector2 origen, Vector2 direccion)
    {
        var proyectil = Instantiate(proyectilPrefab, origen, Quaternion.identity);
        proyectil.Lanzar(direccion.normalized, velocidadProyectil, danoProyectil, knockbackProyectil, capasDanio);
    }

    // Evento desde la animación Attack
    public void GolpeEspadaAnimEvent()
    {
        Vector2 centro = puntoGolpeEspada != null ? (Vector2)puntoGolpeEspada.position : rb.position;
        IntentarDaniarJugador(danoEspada, radioGolpeEspada, centro);
    }

    void IntentarDaniarJugador(int danio, float radio, Vector2 centro)
    {
        if (Time.timeScale == 0f) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(centro, radio, capasDanio);
        if (hits == null || hits.Length == 0) return;

        foreach (var hit in hits)
        {
            if (hit == null) continue;

            var vida = ObtenerVida(hit);
            if (vida != null && vida == playerVida)
            {
                Vector2 knockDir = player != null
                    ? ((Vector2)player.position - centro).normalized
                    : ((Vector2)hit.transform.position - centro).normalized;

                if (knockDir == Vector2.zero)
                    knockDir = Vector2.up;

                vida.RecibirDanio(danio, knockDir, knockbackFuerza);
                break;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision) => DanarPorContacto(collision.collider, ObtenerPuntoContacto(collision));

    void OnCollisionStay2D(Collision2D collision) => DanarPorContacto(collision.collider, ObtenerPuntoContacto(collision));

    void OnTriggerEnter2D(Collider2D other) => DanarPorContacto(other, other.transform.position);

    void OnTriggerStay2D(Collider2D other) => DanarPorContacto(other, other.transform.position);

    void DanarPorContacto(Collider2D objetivo, Vector2 centroDanio)
    {
        if (Time.timeScale == 0f || objetivo == null)
            return;

        if ((capasDanio.value & (1 << objetivo.gameObject.layer)) == 0)
            return;

        var vida = ObtenerVida(objetivo);
        if (vida == null || vida != playerVida)
            return;

        Vector2 knockDir = ((Vector2)objetivo.transform.position - centroDanio).normalized;
        if (knockDir == Vector2.zero)
            knockDir = Vector2.up;

        vida.RecibirDanio(danoContacto, knockDir, knockbackFuerza);
    }

    static Vector2 ObtenerPuntoContacto(Collision2D collision)
    {
        if (collision == null || collision.contactCount == 0)
            return collision != null ? (Vector2)collision.transform.position : Vector2.zero;

        return collision.GetContact(0).point;
    }

    Vida ObtenerVida(Collider2D col)
    {
        if (col == null) return null;

        var vida = col.GetComponentInParent<Vida>();
        if (vida == null && col.attachedRigidbody != null)
            vida = col.attachedRigidbody.GetComponent<Vida>();

        return vida;
    }

    void ActualizarFase()
    {
        if (health == null || enFase2) return;

        if (health.vidaActual <= health.vidaMax * porcentajeFase2)
        {
            enFase2 = true;
            if (anim) anim.SetTrigger("Phase2");
        }
    }

    public void Morir()
    {
        // <<< NUEVO: abrir la arena al morir >>>
        if (arenaWalls != null)
        {
            arenaWalls.LiberarArena();
        }

        // Lanzar animación de muerte
        if (anim != null)
            anim.SetTrigger("Death");   // Debes tener un Trigger "Death" en el Animator

        // Detener IA del boss
        StopAllCoroutines();
        this.enabled = false;

        // Opcional: parar movimiento físico
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        // Opcional: destruir al cabo de un rato (por ejemplo 3 segundos)
        Destroy(gameObject, 3f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radioContacto);

        Gizmos.color = new Color(1f, 0.5f, 0f);
        Gizmos.DrawWireSphere(transform.position, radioGolpeSalto);

        if (puntoGolpeEspada != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(puntoGolpeEspada.position, radioGolpeEspada);
        }
    }
}
