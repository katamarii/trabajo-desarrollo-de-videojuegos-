using UnityEngine;
using UnityEngine.SceneManagement; // <-- IMPORTANTE: Necesario para reiniciar escenas

public enum GameState
{
    MainMenu,
    Playing,
    Pause,
    GameOver,
    Victory
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState CurrentState { get; private set; }

    [Header("Puntuación")]
    [SerializeField] private int score;
    public int Score => score;

    [Header("Vida del Jugador")]
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;

    public int CurrentHealth => currentHealth;

    private void Awake()
    {
        // Implementación del Patrón Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Inicializamos la vida al arrancar el nivel
        currentHealth = maxHealth;
        ChangeState(GameState.Playing);
    }

    public void AddScore(int amount)
    {
        score += amount;
        
        // Actualiza el texto en la interfaz si el UIManager existe
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateScore(score);
        }
    }

    // Método para recibir daño
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        
        // Actualiza el texto de salud en el UIManager si existe
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHealth(currentHealth);
        }

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            ChangeState(GameState.GameOver);
        }
    }

    public void ChangeState(GameState newState)
    {
        CurrentState = newState;

        switch (CurrentState)
        {
            case GameState.MainMenu:
                Time.timeScale = 1f;
                break;

            case GameState.Playing:
                Time.timeScale = 1f;
                break;

            case GameState.Pause:
                Time.timeScale = 0f;
                break;

            case GameState.GameOver:
                Time.timeScale = 0f; // Pausa el juego al perder
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.ShowGameOver();
                }
                break;

            case GameState.Victory:
                Time.timeScale = 0f;
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.ShowVictory();
                }
                break;
        }
    }

    // NUEVO: Método para reiniciar el nivel desde el botón de Game Over
    public void RestartGame()
    {
        // Restaurar el tiempo a la normalidad antes de recargar
        Time.timeScale = 1f; 

        // Recarga la escena actual activa
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}