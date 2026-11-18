using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class EnemyJumper : MonoBehaviour
{
    [Header("Salto del Enemigo")]
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float jumpInterval = 2f;
    [SerializeField] private int damageAmount = 1;
    [SerializeField] private int scoreValue = 150;
    [SerializeField] private float highBounceForce = 14f; // Rebote más alto para este enemigo

    private Rigidbody2D rb;
    private Animator anim;
    private float jumpTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        jumpTimer += Time.deltaTime;

        if (jumpTimer >= jumpInterval)
        {
            Jump();
            jumpTimer = 0f;
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);

        if (anim != null)
        {
            anim.SetBool("IsJumping", true);
        }

        Invoke("ResetJumpAnimation", 0.2f);
    }

    private void ResetJumpAnimation()
    {
        if (anim != null)
        {
            anim.SetBool("IsJumping", false);
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
                // 1. Hacer rebotar al jugador más alto
                PlayerController2D player = collision.gameObject.GetComponent<PlayerController2D>();
                if (player != null)
                {
                    player.Bounce(highBounceForce);
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

                // 4. Destruir con un mínimo retraso para estabilizar la UI y físicas
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