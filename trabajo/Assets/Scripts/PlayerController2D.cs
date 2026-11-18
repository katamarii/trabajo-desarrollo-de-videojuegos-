using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float runSpeed = 11f;
    [SerializeField] private float jumpForce = 12f;

    [Header("Detección de Suelo")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float checkRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Invulnerabilidad y Retroceso por Daño")]
    [SerializeField] private float invulnerabilityDuration = 1.5f; // Segundos de invulnerabilidad
    [SerializeField] private float knockbackForceX = 6f;          // Fuerza de retroceso horizontal
    [SerializeField] private float knockbackForceY = 6f;          // Impulso vertical al recibir daño
    private bool isInvulnerable = false;
    private float invulnerabilityTimer;
    private float knockbackEndTime; // Control para evitar que el movimiento anule el retroceso

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private float horizontalInput;
    private bool isGrounded;
    private float currentSpeed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    public void Bounce(float bounceForce)
    {
        if (rb != null)
        {
            rb.velocity = new Vector2(rb.velocity.x, bounceForce);
        }
    }

    // Método que gestiona el daño, la invulnerabilidad y el salto hacia atrás
    public void TakeDamage(int damage)
    {
        // Si ya es invulnerable, ignoramos el golpe
        if (isInvulnerable) return;

        // Activamos la invulnerabilidad y el tiempo de bloqueo de input para el retroceso
        isInvulnerable = true;
        invulnerabilityTimer = invulnerabilityDuration;
        knockbackEndTime = Time.time + 0.25f; // 0.25 segundos donde el impulso manda sobre el teclado

        // Aplicamos el impulso físico de retroceso
        if (rb != null)
        {
            // Si el sprite mira a la derecha (flipX = false), el retroceso lo empuja a la izquierda (-1) y viceversa
            float knockbackDir = spriteRenderer.flipX ? 1f : -1f;
            
            // Asignación directa de velocidad para garantizar el salto hacia atrás
            rb.velocity = new Vector2(knockbackDir * knockbackForceX, knockbackForceY);
        }

        // Restamos la vida a través del GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TakeDamage(damage);
        }
    }
    
    private void Update()
    {
        // 1. Manejo del temporizador de invulnerabilidad y parpadeo visual
        if (isInvulnerable)
        {
            invulnerabilityTimer -= Time.deltaTime;

            // Efecto visual de parpadeo (cambia la opacidad alfa rápidamente)
            if (spriteRenderer != null)
            {
                Color color = spriteRenderer.color;
                color.a = Mathf.Sin(Time.time * 25f) > 0 ? 1f : 0.3f;
                spriteRenderer.color = color;
            }

            // Cuando el tiempo se agota, restauramos el estado normal
            if (invulnerabilityTimer <= 0f)
            {
                isInvulnerable = false;
                if (spriteRenderer != null)
                {
                    Color color = spriteRenderer.color;
                    color.a = 1f; // Restaurar opacidad completa
                    spriteRenderer.color = color;
                }
            }
        }

        // 2. Entrada horizontal
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // 3. Detección de suelo
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        }

        // 4. Sistema de Correr (Mantener Shift Izquierdo)
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = runSpeed;
        }
        else
        {
            currentSpeed = moveSpeed;
        }

        // 5. Salto
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        // 6. Control de Animaciones
        if (anim != null)
        {
            float animationMultiplier = (currentSpeed > moveSpeed) ? 1.6f : 1.0f;
            anim.SetFloat("Speed", Mathf.Abs(horizontalInput) * animationMultiplier);
            anim.SetBool("IsJumping", !isGrounded);
        }

        // 7. Giro del Sprite (Izquierda / Derecha)
        if (horizontalInput > 0)
        {
            spriteRenderer.flipX = false; // Mira a la derecha
        }
        else if (horizontalInput < 0)
        {
            spriteRenderer.flipX = true;  // Mira a la izquierda
        }
    }

    private void FixedUpdate()
    {
        // Si estamos dentro del tiempo de retroceso por daño, no sobrescribimos la velocidad con el input
        if (Time.time < knockbackEndTime)
        {
            return;
        }

        // Movimiento horizontal normal del jugador
        rb.velocity = new Vector2(horizontalInput * currentSpeed, rb.velocity.y);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        }
    }
}