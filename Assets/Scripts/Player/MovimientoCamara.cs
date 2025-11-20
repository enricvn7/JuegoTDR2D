using UnityEngine;

public class MovimientoCamara : MonoBehaviour
{
    public Transform player;        // Referencia al jugador
    public Vector3 offset;          // Offset normal de la cámara

    public float downOffset = 2f;   // Cuánto baja la cámara al presionar S
    public float upOffset = 2f;     // Cuánto sube la cámara al presionar W

    public float smoothSpeed = 5f;  // Velocidad de movimiento suave

    private Vector3 targetOffset;

    void Start()
    {
        targetOffset = offset;
    }

    void LateUpdate()
    {
        // Detectar si se presiona W o S
        if (Input.GetKey(KeyCode.S))
        {
            targetOffset = offset + Vector3.down * downOffset;
        }
        else if (Input.GetKey(KeyCode.W))
        {
            targetOffset = offset + Vector3.up * upOffset;
        }
        else
        {
            targetOffset = offset;
        }

        // Calcular la posición deseada
        Vector3 desiredPosition = player.position + targetOffset;

        // Movimiento suave siguiendo al jugador
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }
}
