using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private int puntosAOtorgar = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica si quien toca la moneda es el Jugador
        if (collision.CompareTag("Player"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(puntosAOtorgar); // Suma puntos y actualiza el HUD
            }
            
            // Destruye la moneda de la escena
            Destroy(gameObject);
        }
    }
}