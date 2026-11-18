using UnityEngine;

public class EnemyWalker : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float walkDistance = 3f;
    [SerializeField] private int damageAmount = 1;
    [SerializeField] private int scoreValue = 100;
    [SerializeField] private float bounceForce = 10f; // Fuerza de rebote estándar

    private Vector2 startPos;
    private int direction = 1;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        startPos = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        transform.Translate(Vector2.right * speed * direction * Time.deltaTime);

        float distanceMoved = transform.position.x - startPos.x;
        
        if (distanceMoved > walkDistance && direction == 1)
        {
            direction = -1;
            if (spriteRenderer != null) spriteRenderer.flipX = true;
        }
        else if (distanceMoved < -walkDistance && direction == -1)
        {
            direction = 1;
            if (spriteRenderer != null) spriteRenderer.flipX = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Evita múltiples ejecuciones si ya fue destruido
            if (this == null) return;

            Vector2 normal = collision.GetContact(0).normal;

            if (normal.y <= -0.5f) // El jugador cayó encima
            {
                // 1. Hacer rebotar al jugador
                PlayerController2D player = collision.gameObject.GetComponent<PlayerController2D>();
                if (player != null)
                {
                    player.Bounce(bounceForce);
                }

                // 2. Sumar puntos
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.AddScore(scoreValue);
                }

                // 3. Desactivar componentes visuales y físicos antes de destruir
                GetComponent<Collider2D>().enabled = false;
                if (GetComponent<SpriteRenderer>() != null)
                {
                    GetComponent<SpriteRenderer>().enabled = false;
                }

                // 4. Destruir con un mínimo retraso
                Destroy(gameObject, 0.05f);
            }
            else // Chocó de lado
            {
                // Buscamos al jugador y aplicamos su sistema de daño con invulnerabilidad
                PlayerController2D player = collision.gameObject.GetComponent<PlayerController2D>();
                if (player != null)
                {
                    player.TakeDamage(damageAmount);
                }
            }
        }
    }
}