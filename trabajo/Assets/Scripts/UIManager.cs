using UnityEngine;
using UnityEngine.UI; // Asegúrate de tener este *using*

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Paneles de Estado")]
    [SerializeField] private GameObject panelHUD;
    [SerializeField] private GameObject panelGameOver;
    [SerializeField] private GameObject panelVictory;

    [Header("Textos de Información")]
    [SerializeField] private Text textScore;   // Cambiado a Text normal
    [SerializeField] private Text textHealth;  // Cambiado a Text normal

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateScore(int currentScore)
    {
        if (textScore != null)
        {
            textScore.text = "Puntos: " + currentScore;
        }
    }

    public void UpdateHealth(int currentHealth)
    {
        if (textHealth != null)
        {
            textHealth.text = "Vida: " + currentHealth;
        }
    }

    public void ShowGameOver()
    {
        if (panelHUD != null) panelHUD.SetActive(false);
        if (panelGameOver != null) panelGameOver.SetActive(true);
    }

    public void ShowVictory()
    {
        if (panelHUD != null) panelHUD.SetActive(false);
        if (panelVictory != null) panelVictory.SetActive(true);
    }
}