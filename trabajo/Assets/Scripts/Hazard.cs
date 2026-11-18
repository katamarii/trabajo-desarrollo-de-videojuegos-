using UnityEngine;

public class Hazard : MonoBehaviour
{
    [Header("Configuración de Daño")]
    [SerializeField] private int damageAmount = 1;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController2D player = collision.gameObject.GetComponent<PlayerController2D>();
            if (player != null)
            {
                player.TakeDamage(damageAmount);
            }
        }
    }

    // Por si usas los pinchos como Trigger (Collider configurado como Is Trigger)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController2D player = collision.GetComponent<PlayerController2D>();
            if (player != null)
            {
                player.TakeDamage(damageAmount);
            }
        }
    }
}